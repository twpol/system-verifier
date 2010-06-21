//------------------------------------------------------------------------------
// DirectShow plug-in, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using JGR.SystemVerifier.Plugins;
using System.IO;

namespace DirectShow {
	public class DirectShowFilters : IPlugin, IPluginWithHost, IPluginWithSections, IScanner, IDisplay {
		#region IPlugin Members

		public string Name {
			get { return "DirectShow Filters"; }
		}

		public string Description {
			get { return "Validates registration and settings for DirectShow filters."; }
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
		#region IPluginWithSections Members

		enum SectionsEnum : long {
			ExtensionRegistration
		}

		Dictionary<long, bool> sectionsEnabled = new Dictionary<long, bool>(Enum.GetNames(typeof(SectionsEnum)).Length);

		public List<KeyValuePair<string, long>> Sections {
			get {
				List<KeyValuePair<string, long>> rv = new List<KeyValuePair<string, long>>();
				rv.Add(new KeyValuePair<string, long>(@"Extension Registration", (long)SectionsEnum.ExtensionRegistration));
				return rv;
			}
		}

		public void SetSections(List<long> sections) {
			foreach (long section in Enum.GetValues(typeof(SectionsEnum))) {
				sectionsEnabled[section] = sections.Contains(section);
			}
		}

		#endregion
		#region IScanner Members

		private long current = 0;
		private long maximum = 0;
		private Queue<DirectShowAction> actions;
		private Dictionary<long, string> prefixes;

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
			actions = new Queue<DirectShowAction>();

			prefixes = new Dictionary<long, string>();
			prefixes.Add(host.Bitness, "");
			if (host.Bitness > 32) {
				prefixes.Add(32, @"Wow6432Node\");
			}

			foreach (var prefix in prefixes) {
				// File Extension -> Source Filter registrations
				if (sectionsEnabled[(long)SectionsEnum.ExtensionRegistration]) {
					using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"Media Type\Extensions")) {
						foreach (var extensionName in key.GetSubKeyNames()) {
							using (var extensionKey = key.OpenSubKey(extensionName)) {
								var sourceFilter = extensionKey.GetValue("Source Filter", null);
								if (sourceFilter != null) {
									actions.Enqueue(new DirectShowExtension(extensionName, sourceFilter.ToString()));
								}
							}
						}
					}
				}
				// DirectShow Filters
				using (var catKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"CLSID\{DA4E3DA0-D07D-11d0-BD50-00A0C911CE86}\Instance")) {
					foreach (var category in catKey.GetSubKeyNames()) {
						using (var catSubKey = catKey.OpenSubKey(category)) {
							var catName = catSubKey.GetValue("FriendlyName", null);
							var catClassId = catSubKey.GetValue("CLSID", null);
							using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"CLSID\" + category + @"\Instance")) {
								if (key != null) {
									foreach (var extensionName in key.GetSubKeyNames()) {
										using (var extensionKey = key.OpenSubKey(extensionName)) {
											var name = extensionKey.GetValue("FriendlyName", null);
											var classId = extensionKey.GetValue("CLSID", null);
											if (classId != null) {
												actions.Enqueue(new DirectShowCategoryCodec(catName == null ? catClassId.ToString() : catName.ToString(), catClassId.ToString(), name == null ? classId.ToString() : name.ToString(), classId.ToString()));
											}
										}
									}
								}
							}
						}
					}
				}
				// Direct Show media categories
				using (var catKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"DirectShow\MediaObjects\Categories")) {
					foreach (var category in catKey.GetSubKeyNames()) {
						using (var catSubKey = catKey.OpenSubKey(category)) {
							var catName = catSubKey.GetValue(null, null);
							foreach (var classid in catSubKey.GetSubKeyNames()) {
								using (var objKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"DirectShow\MediaObjects\" + classid)) {
									if (objKey != null) {
										var name = objKey.GetValue(null, null);
										actions.Enqueue(new DirectShowCategoryCodec(catName.ToString(), "{" + category + "}", name.ToString(), "{" + classid + "}"));
									}
								}
							}
						}
					}
				}
				// Direct Show preferred filters
				using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\" + prefix.Value + @"Microsoft\DirectShow\Preferred")) {
					foreach (var name in key.GetValueNames()) {
						var classId = key.GetValue(name, null);
						if (classId != null) {
							actions.Enqueue(new DirectShowPreferredCodec(name, classId.ToString()));
						}
					}
				}
				// Media Foundation preferred sources
				using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"MediaFoundation\MediaSources\Preferred")) {
					foreach (var name in key.GetValueNames()) {
						var classId = key.GetValue(name, null);
						if (classId != null) {
							actions.Enqueue(new DirectShowPreferredCodec(name, classId.ToString()));
						}
					}
				}
				// Media Foundation preferred transformations
				//using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prefix.Value + @"MediaFoundation\Transforms\Preferred")) {
				//    foreach (var name in key.GetValueNames()) {
				//        var classId = key.GetValue(name, null);
				//        if (classId != null) {
				//            actions.Enqueue(new DirectShowPreferrerCodec(name, classId.ToString()));
				//        }
				//    }
				//}
			}

			maximum = actions.Count;
			current = 0;
		}

		public List<IScanItem> Process() {
			var rv = new List<IScanItem>();
			var action = actions.Dequeue();
			var extension = action as DirectShowExtension;
			var codec = action as DirectShowCodec;
			DefaultScanItem item;
			current++;

			if (extension != null) {
				if (extension.Extension.StartsWith(".")) {
					using (var key = Microsoft.Win32.Registry.ClassesRoot) {
						foreach (var prefix in prefixes) {
							using (var filterKey = key.OpenSubKey(prefix.Value + @"CLSID\" + extension.SourceFilter)) {
								if (filterKey == null) {
									item = new DefaultScanItem(extension.Extension);
									item.Properties["Severity"] = "Warning";
									item.Properties["Description"] = "Associated DirectShow source filter " + FormatClassID(extension.SourceFilter) + " is not registered for " + prefix.Key + "bit applications.";
									rv.Add(item);
								}
							}
						}
					}
				} else {
					item = new DefaultScanItem(extension.Extension);
					item.Properties["Severity"] = "Warning";
					item.Properties["Description"] = "Association with DirectShow source filter " + FormatClassID(extension.SourceFilter) + @" is incorrect. The registration at <HKCR\Media Type\Extensions> has no preceeding '.'.";
					rv.Add(item);
				}
			}
			if (codec != null) {
				using (var key = Microsoft.Win32.Registry.ClassesRoot) {
					foreach (var prefix in prefixes) {
						using (var filterKey = key.OpenSubKey(prefix.Value + @"CLSID\" + codec.ClassId)) {
							item = new DefaultScanItem("DirectShowFilter");
							item.Properties["Name"] = codec.Name;
							item.Properties["ClassID"] = codec.ClassId;
							var categoryCodec = codec as DirectShowCategoryCodec;
							if (categoryCodec != null) {
								item.Properties["CategoryName"] = categoryCodec.CategoryName;
								item.Properties["CategoryClassID"] = categoryCodec.CategoryClassId;
							}
							var preferredCodec = codec as DirectShowPreferredCodec;
							if (preferredCodec != null) {
								item.Properties["Type"] = preferredCodec.Type;
							}
							item.Properties["Bitness"] = prefix.Key + "bit";

							if (filterKey == null) {
								item.Properties["ItemIssue"] = "Unregistered";
								rv.Add(item);
							} else {
								using (var filterInprocKey = filterKey.OpenSubKey("InprocServer32")) {
									var path = filterInprocKey.GetValue(null, null);
									if (path == null) {
										item.Properties["ItemIssue"] = "RegisteredWithoutServer";
										rv.Add(item);
										continue;
									}
									if (!File.Exists(path.ToString())) {
										item.Properties["ItemIssue"] = "RegisteredServerNotOnDisk";
										item.Properties["ItemPath"] = path.ToString();
										rv.Add(item);
										continue;
									}
								}
							}
						}
					}
				}
			}

			return rv;
		}

		public void PostProcess() {
		}

		#endregion
		#region IDisplay Members

		public bool Accepts(IScanItem item) {
			return (item.Type == "DirectShowFilter");
		}

		public IDisplayItem Process(IScanItem item) {
			DisplayItemSeverity severity = DisplayItemSeverity.Information;
			string name = item.Type;
			string description = "";

			switch (item.Type) {
				case "DirectShowFilter":
					var issue = "";
					switch ((string)item.Properties["ItemIssue"]) {
						case "Unregistered":
							issue = "is not registered";
							break;
						case "RegisteredWithoutServer":
							issue = "has no registered file path";
							break;
						case "RegisteredServerNotOnDisk":
							issue = "uses non-existant file '" + item.Properties["ItemPath"] + "'";
							break;
					}
					severity = DisplayItemSeverity.Warning;
					if (item.Properties.ContainsKey("CategoryName")) {
						name = (string)item.Properties["Name"];
						description = "DirectShow '" + item.Properties["CategoryName"] + "' instance " + issue + " for " + item.Properties["Bitness"] + " applications (" + item.Properties["CategoryClassID"] + "/" + item.Properties["ClassID"] + "). You may not be able to play some files in " + item.Properties["Bitness"] + " applications.";
					} else if (item.Properties.ContainsKey("Type")) {
						name = (string)item.Properties["Type"];
						description = "DirectShow preferred filter " + FormatClassID((string)item.Properties["ClassID"]) + " " + issue + " for " + item.Properties["Bitness"] + " applications. You may not be able to play some files in " + item.Properties["Bitness"] + " applications.";
					} else {
						name = (string)item.Properties["Name"];
						description = "DirectShow filter " + issue + " for " + item.Properties["Bitness"] + " applications (" + FormatClassID((string)item.Properties["ClassID"]) + "). You may not be able to play some files in " + item.Properties["Bitness"] + " applications.";
					}
					break;
			}

			return new DefaultDisplayItem(severity, name, description);
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

		#endregion
	}

	class DirectShowAction {
	}

	class DirectShowExtension : DirectShowAction {
		public string Extension { get; private set; }
		public string SourceFilter { get; private set; }

		public DirectShowExtension(string extension, string sourceFilter) {
			Extension = extension;
			SourceFilter = sourceFilter;
		}
	}

	class DirectShowCodec : DirectShowAction {
		public string Name { get; private set; }
		public string ClassId { get; private set; }

		public DirectShowCodec(string name, string classid) {
			Name = name;
			ClassId = classid;
		}
	}

	class DirectShowCategoryCodec : DirectShowCodec {
		public string CategoryName { get; private set; }
		public string CategoryClassId { get; private set; }

		public DirectShowCategoryCodec(string categoryName, string categoryClassId, string name, string classid)
			: base(name, classid) {
			CategoryName = categoryName;
			CategoryClassId = categoryClassId;
		}
	}

	class DirectShowPreferredCodec : DirectShowCodec {
		public string Type { get; private set; }

		public DirectShowPreferredCodec(string type, string classid)
			: base("", classid) {
			Type = type;
		}
	}
}
