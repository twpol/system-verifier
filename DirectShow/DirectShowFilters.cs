using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using JGR.SystemVerifier.Plugins;

namespace DirectShow
{
	public class DirectShowFilters : IPlugin, IPluginWithHost, IPluginWithSections, IScanner, IDisplay
	{
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

		public List<KeyValuePair<string, long>> Sections {
			get {
				List<KeyValuePair<string, long>> rv = new List<KeyValuePair<string,long >>();
				rv.Add(new KeyValuePair<string, long>(@"Extension Registration", 1));
				return rv;
			}
		}

		public void SetSections(List<long> sections) {
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
		#region IScanner Members

		private long current = 0;
		private long maximum = 0;
		private Queue<DirectShowExtension> extensions;

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
			extensions = new Queue<DirectShowExtension>();

			using (RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"Media Type\Extensions")) {
				foreach (string extensionName in key.GetSubKeyNames()) {
					using (RegistryKey extensionKey = key.OpenSubKey(extensionName)) {
						object sourceFilter = extensionKey.GetValue("Source Filter", null);
						if (sourceFilter != null) {
							extensions.Enqueue(new DirectShowExtension(extensionName, sourceFilter.ToString()));
						}
					}
				}
			}

			maximum = extensions.Count;
			current = 0;
		}

		public List<IScanItem> Process() {
			List<IScanItem> rv = new List<IScanItem>();
			DirectShowExtension extension = extensions.Dequeue();
			DefaultScanItem item;
			current++;

			if (extension.Extension.StartsWith(".")) {
				using (RegistryKey key = Microsoft.Win32.Registry.ClassesRoot) {
					using (RegistryKey filterKey = key.OpenSubKey(@"CLSID\" + extension.SourceFilter)) {
						if (filterKey == null) {
							item = new DefaultScanItem("DirectShowFilter");
							item.Properties["Extension"] = extension.Extension;
							item.Properties["ClassID"] = extension.SourceFilter;
							item.Properties["Bitness"] = host.Bitness + "bit";
							item.Properties["ItemIssue"] = "Unregistered";
							rv.Add(item);
						}
					}
					if (host.Bitness > 32) {
						using (RegistryKey filterKey = key.OpenSubKey(@"Wow6432Node\CLSID\" + extension.SourceFilter)) {
							if (filterKey == null) {
								item = new DefaultScanItem("DirectShowFilter");
								item.Properties["Extension"] = extension.Extension;
								item.Properties["ClassID"] = extension.SourceFilter;
								item.Properties["Bitness"] = "32bit";
								item.Properties["ItemIssue"] = "Unregistered";
								rv.Add(item);
							}
						}
					}
				}
			} else {
				item = new DefaultScanItem("DirectShowFilter");
				item.Properties["Extension"] = extension.Extension;
				item.Properties["ClassID"] = extension.SourceFilter;
				item.Properties["ItemIssue"] = "ExtensionNoDot";
				rv.Add(item);
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
					switch ((string)item.Properties["ItemIssue"]) {
						case "Unregistered":
							severity = DisplayItemSeverity.Warning;
							name = "Missing Filter";
							if (item.Properties["Extension"] != null) {
								description = "The DirectShow filter associated with '" + item.Properties["Extension"] + "' files is not registered for " + item.Properties["Bitness"] + " applications. Class ID '" + item.Properties["ClassID"] + "' is not registered. You may not be able to play '" + item.Properties["Extension"] + "' files in " + item.Properties["Bitness"] + " applications.";
							} else {
								description = "The DirectShow filter '" + item.Properties["ClassID"] + "' is not registered for " + item.Properties["Bitness"] + " applications.";
							}
							break;
						case "ExtensionNoDot":
							severity = DisplayItemSeverity.Warning;
							name = "Registration";
							description = "The DirectShow filter '" + item.Properties["ClassID"] + "' is incorrectly registered for '." + item.Properties["Extension"] + @"' files. The extension registration at <HKCR\Media Type\Extensions> has no preceeding '.'. You may not be able to play '." + item.Properties["Extension"] + "' files.";
							break;
					}
					break;
			}

			return new DefaultDisplayItem(severity, name, description);
		}

		#endregion
	}

	class DirectShowExtension
	{
		public DirectShowExtension(string extension, string sourceFilter) {
			this.extension = extension;
			this.sourceFilter = sourceFilter;
		}

		private string extension;
		public string Extension {
			get { return extension; }
		}

		private string sourceFilter;
		public string SourceFilter {
			get { return sourceFilter; }
		}
	}
}
