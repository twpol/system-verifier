using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using JGR.SystemVerifier.Plugins;

namespace WindowsAssociations
{
    public class WindowsFileAssociations : IPlugin, IPluginWithHost, IPluginWithSections, IScanner
    {
        #region IPlugin Members

        public string Name {
            get { return "Windows File Associations"; }
        }

        public string Description {
            get { return "Validates registrations for Windows Explorer file associations and handlers."; }
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

        enum SectionsEnum : long
        {
            ExtensionHandlerMapping
        }

        Dictionary<long, bool> sectionsEnabled = new Dictionary<long, bool>(Enum.GetNames(typeof(SectionsEnum)).Length);

        public List<KeyValuePair<string, long>> Sections {
            get {
                List<KeyValuePair<string, long>> rv = new List<KeyValuePair<string, long>>();
                rv.Add(new KeyValuePair<string, long>(@"Extension Handler Mapping", (long)SectionsEnum.ExtensionHandlerMapping));
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
        private Queue<KeyValuePair<RegistryKey, Queue<string>>> extensions;

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
            extensions = new Queue<KeyValuePair<RegistryKey,Queue<string>>>();
			maximum = 0;
            current = 0;

            // Scan for all the extensions and populate our queue.
            var rootKeys = new List<KeyValuePair<RegistryKey, string>>();
			rootKeys.Add(new KeyValuePair<RegistryKey, string>(Registry.LocalMachine, @"SOFTWARE\Classes"));
            if (host.Bitness > 32) {
				rootKeys.Add(new KeyValuePair<RegistryKey, string>(Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Classes"));
            }
			rootKeys.Add(new KeyValuePair<RegistryKey, string>(Registry.CurrentUser, @"SOFTWARE\Classes"));
			foreach (KeyValuePair<RegistryKey, string> root in rootKeys) {
				RegistryKey key = root.Key.OpenSubKey(root.Value);
				var queue = new Queue<string>();
				foreach (string extensionName in key.GetSubKeyNames()) {
					if (extensionName.StartsWith(".")) {
						queue.Enqueue(extensionName);
					}
				}
				extensions.Enqueue(new KeyValuePair<RegistryKey, Queue<string>>(key, queue));
				maximum += queue.Count;
			}
        }

        public List<IScanItem> Process() {
            List<IScanItem> rv = new List<IScanItem>();
			while (extensions.Peek().Value.Count == 0) {
				extensions.Peek().Key.Close();
				extensions.Dequeue();
			}
			RegistryKey key = extensions.Peek().Key;
            string extension = extensions.Peek().Value.Dequeue();
            current++;

			CheckFileAssiociation(rv, key, extension);

            return rv;
        }

		void CheckFileAssiociation(List<IScanItem> rv, RegistryKey root, string extension) {
			using (RegistryKey key = root.OpenSubKey(extension)) {
				var mappedName = (string)key.GetValue(null);
				if (mappedName != null) {
					using (RegistryKey mappedKey = root.OpenSubKey(mappedName)) {
						if (mappedKey == null) {
							rv.Add(new DefaultScanItem(extension, DisplayItemSeverity.Error, "File extension handler '" + mappedName + "' is not registered ('" + key.Name + "')."));
						}
					}
				}

				CheckShellHandlers(rv, key, extension);
				CheckShellExHandlers(rv, key, extension);

				if (mappedName != null) {
					using (RegistryKey mappedKey = root.OpenSubKey(mappedName)) {
						if (mappedKey != null) {
							CheckShellHandlers(rv, mappedKey, extension);
							CheckShellExHandlers(rv, mappedKey, extension);
						}
					}
				}
			}
		}

		void CheckShellHandlers(List<IScanItem> rv, RegistryKey root, string extension) {
			using (RegistryKey shellKey = root.OpenSubKey("shell")) {
				if (shellKey == null) return;

				// Check the default handler is valid.
				var defaultHandler = (string)shellKey.GetValue(null);
				if (defaultHandler == null) {
					using (RegistryKey defaultHandlerKey = shellKey.OpenSubKey("open")) {
						if (defaultHandlerKey == null) {
							rv.Add(new DefaultScanItem(extension, DisplayItemSeverity.Warning, "File extension has no default handler ('" + shellKey.Name + "')."));
						}
					}
				} else {
					using (RegistryKey defaultHandlerKey = shellKey.OpenSubKey(defaultHandler)) {
						if (defaultHandlerKey == null) {
							rv.Add(new DefaultScanItem(extension, DisplayItemSeverity.Error, "File extension default handler '" + defaultHandler + "' is not registered ('" + shellKey.Name + "')."));
						}
					}
				}

				foreach (string handlerName in shellKey.GetSubKeyNames()) {
					CheckShellHandler(rv, shellKey, extension, handlerName);
				}
			}
		}

		void CheckShellHandler(List<IScanItem> rv, RegistryKey root, string extension, string handlerName) {
			using (RegistryKey handlerKey = root.OpenSubKey(handlerName)) {
				using (RegistryKey handlerCommandKey = handlerKey.OpenSubKey("command")) {
					if (handlerCommandKey == null) {
						rv.Add(new DefaultScanItem(extension, DisplayItemSeverity.Error, "File extension handler '" + handlerName + "' has no command ('" + handlerKey.Name + "')."));
					} else {
						var execCommand = (string)handlerCommandKey.GetValue(null);
						var execDelegate = (string)handlerCommandKey.GetValue("DelegateExecute");
						if ((execCommand == null) && (execDelegate == null)) {
							rv.Add(new DefaultScanItem(extension, DisplayItemSeverity.Error, "File extension handler '" + handlerName + "' has no command to execute ('" + handlerKey.Name + "')."));
						}
					}
				}
			}
		}

		void CheckShellExHandlers(List<IScanItem> rv, RegistryKey root, string extension) {

		}

		void CheckShellExHandler(List<IScanItem> rv, RegistryKey root, string extension) {

		}

        public void PostProcess() {
        }

        #endregion
    }
}
