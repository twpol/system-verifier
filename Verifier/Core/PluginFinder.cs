//------------------------------------------------------------------------------
// System Verifier GUI, part of System Verifier (http://systemverifier.codeplex.com/).
// License: New BSD License (BSD).
//------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace JGR.SystemVerifier.Core
{
	class PluginFinder
	{
		Thread thread;

		public PluginFinder() {
			thread = new Thread(new ThreadStart(this.ThreadProc));
		}

		public void Start() {
			thread.Start();
		}

		public void Stop() {
			thread.Abort();
			if (OnStop != null) OnStop(this, new EventArgs());
		}

		void ThreadProc() {
			if (OnStart != null) OnStart(this, new EventArgs());
			if ((OnIsPluginTrusted == null) || (OnPluginFound == null)) {
				if (OnStop != null) OnStop(this, new EventArgs());
				return;
			}
			
			// Find out where we're running from, first.
			string pluginPath = Application.ExecutablePath;
			pluginPath = pluginPath.Substring(0, pluginPath.LastIndexOf(@"\")) + @"\Plugins";

			if (Directory.Exists(pluginPath)) {
				foreach (string plugin in Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories)) {
					Assembly reflectionAssembly = Assembly.ReflectionOnlyLoadFrom(plugin);
					if (OnIsPluginTrusted(this, new IsPluginTrustedEventArgs(reflectionAssembly.Location, reflectionAssembly.FullName))) {
						Assembly pluginAssembly = Assembly.Load(reflectionAssembly.GetName());

						foreach (Type type in pluginAssembly.GetExportedTypes()) {
							if (type.GetInterface("JGR.SystemVerifier.Plugins.IPlugin") != null) {
								OnPluginFound(this, new PluginFoundEventArgs(type));
							}
						}
					}
				}
			}
			
			if (OnStop != null) OnStop(this, new EventArgs());
		}

		public event EventHandler OnStart;
		public class IsPluginTrustedEventArgs {
			public string Filename;
			public string Fullname;
			public IsPluginTrustedEventArgs(string filename, string fullname) {
				this.Filename = filename;
				this.Fullname = fullname;
			}
		}
		public delegate bool IsPluginTrustedEventHandler(object sender, IsPluginTrustedEventArgs e);
		public event IsPluginTrustedEventHandler OnIsPluginTrusted;
		public class PluginFoundEventArgs {
			public Type type;
			public PluginFoundEventArgs(Type type) {
				this.type = type;
			}
		}
		public delegate void PluginFoundEventHandler(object sender, PluginFoundEventArgs e);
		public event PluginFoundEventHandler OnPluginFound;
		public event EventHandler OnStop;
	}
}
