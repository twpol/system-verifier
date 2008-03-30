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
				item.Properties["Description"] = "Administrator rights are required to read the enabled extension list.";
				rv.Add(item);
			}

			ProcessExtensionFor(extension, rv);

			return rv;
		}

		void ProcessExtensionFor(string extension, List<IScanItem> rv) {
			DefaultScanItem item;
			string prefix = "";
			if (disabledExtensions != null) {
				if (disabledExtensions.Contains(extension)) {
					prefix = "[Off] ";
				} else {
					prefix = "[On] ";
				}
			}

			{
				string persistHandler = GetPersistentHandler(extension);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = prefix + GetNameFromClassID(iFilterClass) + " (Persistent Handler)";
					rv.Add(item);
					return;
				}
			}
			{
				string contentType = GetValueFromKey(@"SOFTWARE\Classes\" + extension, "Content Type");
				string classID = GetValueFromKey(@"SOFTWARE\Classes\MIME\Database\Content Type\" + contentType, "CLSID");
				string persistHandler = GetPersistentHandler(classID);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = prefix + GetNameFromClassID(iFilterClass) + " (Content Type)";
					rv.Add(item);
					return;
				}
			}
			{
				string handlerName = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + extension);
				string classID = GetDefaultValueFromKey(@"SOFTWARE\Classes\" + handlerName + @"\CLSID");
				string persistHandler = GetPersistentHandler(classID);
				string iFilterClass = GetIFilterFromPersistentHandler(persistHandler);
				if (iFilterClass != "") {
					item = new DefaultScanItem(extension);
					item.Properties["Description"] = prefix + GetNameFromClassID(iFilterClass) +" (Document Type)";
					rv.Add(item);
					return;
				}
			}

			item = new DefaultScanItem(extension);
			item.Properties["Description"] = prefix + "File Properties Filter" + " (Default)";
			rv.Add(item);
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

		string GetNameFromClassID(string classID) {
			string name = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID);
			if (name == "") {
				name = GetDefaultValueFromKey(@"SOFTWARE\Classes\CLSID\" + classID + @"\InProcServer32");
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
