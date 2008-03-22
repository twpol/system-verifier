using System;
using System.Collections.Generic;
using System.Text;
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
		private int state = 0;
		private Queue<string> extensions;

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

			maximum = extensions.Count;
			current = 0;
		}

		public List<IScanItem> Process() {
			List<IScanItem> rv = new List<IScanItem>();
			string extension = extensions.Dequeue();
			DefaultScanItem item;
			current++;

			ProcessExtensionFor(extension, rv);

			if (false) {
				item = new DefaultScanItem(extension);
				item.Properties["Description"] = item.Type;
				rv.Add(item);
			}

			return rv;
		}

		void ProcessExtensionFor(string extension, List<IScanItem> rv) {
			DefaultScanItem item;

			{
				string persistHandler = GetPersistentHandler(extension);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				string library = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + iFilterClass + @"\InProcServer32");
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = "PH: " + library + " / " + GetClassIDName(iFilterClass) + " / " + persistHandler;
					rv.Add(item);
					return;
				}
			}
			{
				string contentType = GetValueFromKey(@"SOFTWARE\Classes\" + extension, "Content Type");
				string classID = GetValueFromKey(@"SOFTWARE\Classes\MIME\Database\Content Type\" + contentType, "CLSID");
				string persistHandler = GetPersistentHandler(classID);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				string library = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + iFilterClass + @"\InProcServer32");
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = "CT: " + library + " / " + GetClassIDName(iFilterClass) + " / " + GetClassIDName(persistHandler) + " / " + GetClassIDName(classID) + " / " + contentType;
					rv.Add(item);
					return;
				}
			}
			{
				string handlerName = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + extension);
				string classID = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + handlerName + @"\CLSID");
				string persistHandler = GetPersistentHandler(classID);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				string library = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + iFilterClass + @"\InProcServer32");
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = "DT: " + library + " / " + GetClassIDName(iFilterClass) + " / " + GetClassIDName(persistHandler) + " / " + GetClassIDName(classID) + " / " + handlerName;
					rv.Add(item);
					return;
				}
			}
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

		string GetClassIDName(string classID) {
			string name = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID);
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
