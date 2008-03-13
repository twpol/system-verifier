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

			if (true) {
				item = new DefaultScanItem(extension);
				item.Properties["Description"] = item.Type;
				rv.Add(item);
			}

			return rv;
		}

		public void PostProcess() {
		}

		#endregion
	}
}
