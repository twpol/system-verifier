using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using Microsoft.Win32;
using JGR.SystemVerifier.Plugins;

namespace WindowsSearch
{
	public class WindowsSearchService : IPlugin, IPluginWithHost, IScanner
	{
		#region IPlugin Members

		public string Name {
			get { return "Windows Search Service"; }
		}

		public string Description {
			get { return "Validates registration for protocol handlers, persistent handlers and filters."; }
		}

		public string[] Authors {
			get { return new string[] { "James Ross" }; }
		}

		#endregion
		#region IPluginWithHost Members

		private IPluginHost host;

		public void Init(IPluginHost host) {
			this.host = host;
		}

		#endregion
		#region IScanner Members

		private long current = 0;
		private long maximum = 0;
		private Queue<string> extensions;
		private List<string> disabledExtensions;

		public long Current {
			get {
				return current;
			}
		}

		public long Maximum {
			get {
				return maximum;
			}
		}

		public void PreProcess() {
			extensions = new Queue<string>();

			// Scan for all the extensions and populate our queue.
			using (RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes")) {
				foreach (string extensionName in key.GetSubKeyNames()) {
					if (extensionName.StartsWith(".")) {
						extensions.Enqueue(extensionName);
					}
				}
			}

			disabledExtensions = new List<string>();

			try {
				using (RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Search\Gather\Windows\SystemIndex\Extensions\ExtensionList")) {
					foreach (string extensionIndex in key.GetValueNames()) {
						disabledExtensions.Add("." + (string)key.GetValue(extensionIndex));
					}
				}
			} catch (SecurityException) {
				disabledExtensions = null;
			}

			maximum = extensions.Count;
			current = 0;
		}

		public List<IScanItem> Process() {
			List<IScanItem> rv = new List<IScanItem>();
			string extension = extensions.Dequeue();
			DefaultScanItem item;
			current++;

			if ((current == 1) && (disabledExtensions == null)) {
				item = new DefaultScanItem("Windows Search Service");
				item.Properties["Severity"] = "Warning";
				item.Properties["Description"] = "Administrator rights are required to read the enabled extension list. All extensions will be assumed to be enabled.";
				rv.Add(item);
			}

			ProcessExtensionFor(extension, rv);

			return rv;
		}

		void ProcessExtensionFor(string extension, List<IScanItem> rv) {
			DefaultScanItem item;
			//string prefix = "";
			//if (disabledExtensions != null) {
			//	if (disabledExtensions.Contains(extension)) {
			//		prefix = "[Off] ";
			//	} else {
			//		prefix = "[On] ";
			//	}
			//}

			while (true) {
				{
					string persistHandler = GetPersistentHandler(extension);
					string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
					if (iFilterClass != "") {
						rv.AddRange(VerifyFilterClassID(extension, iFilterClass));
						break;
					}
				}
				{
					string contentType = GetValueFromKey(@"SOFTWARE\Classes\" + extension, "Content Type");
					string classID = GetValueFromKey(@"SOFTWARE\Classes\MIME\Database\Content Type\" + contentType, "CLSID");
					string persistHandler = GetPersistentHandler(classID);
					string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
					if (iFilterClass != "") {
						rv.AddRange(VerifyFilterClassID(extension, iFilterClass));
						break;
					}
				}
				{
					string handlerName = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + extension);
					string classID = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + handlerName + @"\CLSID");
					string persistHandler = GetPersistentHandler(classID);
					string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
					if (iFilterClass != "") {
						rv.AddRange(VerifyFilterClassID(extension, iFilterClass));
						break;
					}
				}

				break;
			}

			if (host.Bitness < 64) {
				string propertyHandler32 = GetDefaultValueFromKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers\" + extension);
				if (propertyHandler32 != "") {
					rv.AddRange(VerifyPropertyClassID(extension, propertyHandler32, 32));
				}
			} else {
				string propertyHandler64 = GetDefaultValueFromKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers\" + extension);
				string propertyHandler32 = GetDefaultValueFromKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers\" + extension);
				if ((propertyHandler64 != "") && (propertyHandler32 != "") && (propertyHandler64 != propertyHandler32)) {
					item = new DefaultScanItem(extension);
					item.Properties["Severity"] = "Warning";
					item.Properties["Description"] = "Property handle for 64bit (" + FormatClassID(propertyHandler64) + ") is different from 32bit (" + FormatClassID(propertyHandler32) + ").";
					rv.Add(item);
				}
				if (propertyHandler64 != "") {
					rv.AddRange(VerifyPropertyClassID(extension, propertyHandler64, 64));
				}
				if (propertyHandler32 != "") {
					rv.AddRange(VerifyPropertyClassID(extension, propertyHandler32, 32));
				}
			}
		}

		List<IScanItem> VerifyFilterClassID(string extension, string classID) {
			List<string> interfaces = new List<string>(new string[] {
                "{89BCB740-6119-101A-BCB7-00DD010655AF}" // IFilter
            });
			List<IScanItem> rv = new List<IScanItem>();
			int[] bitnessList = new int[] { 32, 64 };

			foreach (int bitness in bitnessList) {
				Dictionary<string, uint> verify = host.VerifyCOMClassID(classID, interfaces, bitness);
				if (verify["_exitcode"] != 0) {
					IScanItem item = new DefaultScanItem(extension);
					item.Properties["Description"] = "There was an error checking filter class " + FormatClassID(classID) + ". Error code: " + verify["_exitcode"].ToString("x");
					rv.Add(item);
				} else {
					foreach (string intf in interfaces) {
						if (!verify.ContainsKey(intf)) {
							IScanItem item = new DefaultScanItem(extension);
							item.Properties["Description"] = "There was an unexpected error checking filter class " + FormatClassID(classID) + " for interface " + FormatClassID(intf) + " for " + bitness + "bit applications.";
							rv.Add(item);
						} else if (verify[intf] != 0) {
							IScanItem item = new DefaultScanItem(extension);
							item.Properties["Description"] = "Filter class " + FormatClassID(classID) + " is not correctly registered for " + bitness + "bit applications. Error code for " + FormatClassID(intf) + ": " + verify[intf].ToString("x");
							if (verify[intf] == 0x80070057) {
								item.Properties["Description"] += " (E_INVALIDARG)";
							} else if (verify[intf] == 0x80004002) {
								item.Properties["Description"] += " (E_NOINTERFACE)";
							} else if (verify[intf] == 0x80040154) {
								item.Properties["Description"] += " (REGDB_E_CLASSNOTREG)";
							} else if (verify[intf] == 0x80040155) {
								item.Properties["Description"] += " (REGDB_E_IIDNOTREG)";
							}
							rv.Add(item);
						}
					}
				}
			}

			return rv;
		}

        List<IScanItem> VerifyPropertyClassID(string extension, string classID, long bitness) {
            Dictionary<string, string> interfaces = new Dictionary<string, string>();
            interfaces.Add("IPropertyStore", "{886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99}"); // Windows Vista
            //interfaces.Add("IPropertyStoreCapabilities", "{C8E2D566-186E-4D49-BF41-6909EAD56ACC}"); // Windows Vista
            //interfaces.Add("IFilter", "{851E9802-B338-4AB3-BB6B-6AA57CC699D0}");
            //interfaces.Add("IPropertyStorage", "{00000138-0000-0000-C000-000000000046}");
            //interfaces.Add("IShellExtInit", "{000214E8-0000-0000-C000-000000000046}"); // Windows XP
            //interfaces.Add("IPersistStream", "{00000109-0000-0000-C000-000000000046}");
            //interfaces.Add("IPersistStorage", "{0000010A-0000-0000-C000-000000000046}");
            //interfaces.Add("IPersistFile", "{0000010B-0000-0000-C000-000000000046}");
            //interfaces.Add("IInitializeWithFileXXX", "{3B362301-E0F3-4049-B0BD-F34F7D3BB9AA}"); // Windows Vista
            interfaces.Add("IInitializeWithFile", "{B7D14566-0509-4CCE-A71F-0A554233BD9B}"); // Windows Vista
            interfaces.Add("IInitializeWithItem", "{7F73BE3F-FB79-493C-A6C7-7EE14E245841}"); // Windows Vista
            interfaces.Add("IInitializeWithStream", "{B824B49D-22AC-4161-AC8A-9916E8FA3F7F}"); // Windows Vista
            List<IScanItem> rv = new List<IScanItem>();
            classID = classID.ToUpper();

            Dictionary<string, uint> verify = host.VerifyCOMClassID(classID, new List<string>(interfaces.Values), bitness);

            // Check for the entire verify failing for some reason (e.g. COM Checker is missing).
            if (verify["_exitcode"] != 0) {
                IScanItem item = new DefaultScanItem(extension);
                item.Properties["Description"] = "There was an error checking property handler class " + FormatClassID(classID) + ". Error code: " + FormatHResult(verify["_exitcode"]);
                rv.Add(item);
                return rv;
            }

            // Map verify data to our list of interfaces.
            interfaces.Add("class", classID);
            Dictionary<string, uint> resultCodes = new Dictionary<string, uint>();
            foreach (KeyValuePair<string, string> intf in interfaces) {
                if (verify.ContainsKey(intf.Value)) {
                    resultCodes[intf.Key] = verify[intf.Value];
                } else {
                    resultCodes[intf.Key] = 0xFFFFFFFF;
                }
            }

            // Check for unregistered classes specially.
            if (resultCodes["class"] == 0x80040154) {
                IScanItem item = new DefaultScanItem(extension);
                item.Properties["Description"] = "Property handler class " + FormatClassID(classID) + " is not registered for " + bitness + "bit applications.";
                rv.Add(item);
                return rv;
            }

            // Other error with instanciating the class.
            if (resultCodes["class"] != 0x00000000) {
                IScanItem item = new DefaultScanItem(extension);
                item.Properties["Description"] = "There was an error checking property handler class " + FormatClassID(classID) + ". Error code: " + FormatHResult(resultCodes["class"]);
                rv.Add(item);
                return rv;
            }

            // Check each interface was successfully checked and report any errors with checking or with QI.
            foreach (KeyValuePair<string, string> intf in interfaces) {
                if (!verify.ContainsKey(intf.Value)) {
                    IScanItem item = new DefaultScanItem(extension);
                    item.Properties["Description"] = "There was an unexpected error checking property handler class " + FormatClassID(classID) + " for interface " + FormatClassID(intf.Value) + " for " + bitness + "bit applications.";
                    rv.Add(item);
                }
                if ((intf.Key != "class") && (resultCodes[intf.Key] != 0x00000000) && (resultCodes[intf.Key] != 0x80004002)) {
                    IScanItem item = new DefaultScanItem(extension);
                    item.Properties["Description"] = "Property handler class " + FormatClassID(classID) + " fails when QIed to '" + intf.Key + "' (" + FormatHResult(resultCodes[intf.Key]) + ") for " + bitness + "bit applications.";
                    rv.Add(item);
                }
            }

            // All the verify work succeeded, check the class implements what it is supposed to.
            if (resultCodes["IPropertyStore"] != 0) {
                IScanItem item = new DefaultScanItem(extension);
                item.Properties["Description"] = "Property handler class " + FormatClassID(classID) + " does not implement 'IPropertyStore' (" + FormatHResult(resultCodes["IPropertyStore"]) + ") for " + bitness + "bit applications.";
                rv.Add(item);
            } else {
                bool initOK = (resultCodes["IInitializeWithFile"] == 0) || (resultCodes["IInitializeWithItem"] == 0) || (resultCodes["IInitializeWithStream"] == 0);
                if (!initOK) {
                    IScanItem item = new DefaultScanItem(extension);
                    item.Properties["Description"] = "Property handler class " + FormatClassID(classID) + " does not implement 'IShellExtInit', 'IInitializeWithFile', 'IInitializeWithItem' or 'IInitializeWithStream' for " + bitness + "bit applications.";
                    rv.Add(item);
                }
            }

            return rv;
        }

		string GetDefaultValueFromKey(string keyName) {
			RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyName);
			if (key == null) {
				return "";
			}
			using (key) {
				return "" + key.GetValue("");
			}
		}

		string GetValueFromKey(string keyName, string valueName) {
			RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyName);
			if (key == null) {
				return "";
			}
			using (key) {
				return "" + key.GetValue(valueName);
			}
		}

		string FormatClassID(string classID) {
			string name = GetNameFromClassID(classID);
			if (name == classID) {
				return classID;
			}
			return "'" + name + "' " + classID;
		}

		string FormatHResult(uint result) {
			if (result == 0x80070057) {
				return result.ToString("x") + " (E_INVALIDARG)";
			}
			if (result == 0x80004002) {
				return result.ToString("x") + " (E_NOINTERFACE)";
			}
			if (result == 0x80040154) {
				return result.ToString("x") + " (REGDB_E_CLASSNOTREG)";
			}
			if (result == 0x80040155) {
				return result.ToString("x") + " (REGDB_E_IIDNOTREG)";
			}
			return result.ToString("x");
		}

		string GetNameFromClassID(string classID) {
			string name = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID);
			if (name == "") {
				name = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID + @"\InProcServer32");
			}
			if (name == "") {
				name = GetDefaultValueFromKey(@"SOFTWARE\Classes\Interface\" + classID);
			}
			if (name == "") {
				name = GetDefaultValueFromKey(@"SOFTWARE\Classes\Interface\" + classID + @"\InProcServer32");
			}
			if (name == "") {
				return classID;
			}
			return name;
		}

		string GetPersistentHandler(string extensionOrClassID) {
			if (extensionOrClassID.StartsWith(".")) {
				return GetDefaultValueFromKey(@"SOFTWARE\Classes\" + extensionOrClassID + @"\PersistentHandler").ToUpper();
			}
			if (extensionOrClassID.StartsWith("{")) {
				return GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + extensionOrClassID + @"\PersistentHandler").ToUpper();
			}
			return "";
		}

		string GetIFilterFromPersistentHandler(string classID) {
			return GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID + @"\PersistentAddinsRegistered\{89BCB740-6119-101A-BCB7-00DD010655AF}").ToUpper();
		}

		public void PostProcess() {
		}

		#endregion
	}
}
