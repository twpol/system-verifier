using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JGR.SystemVerifier.Plugins;

namespace JGR.SystemVerifier.Core
{
	class Scanner
	{
		Thread thread;
		List<IPlugin> plugins;

		public Scanner() {
			thread = new Thread(this.ThreadProc);
			plugins = new List<IPlugin>();
		}

		public void Start() {
			thread.Start();
		}

		public void Stop() {
			thread.Abort();
			if (OnComplete != null) OnComplete(this, new EventArgs());
		}

		public List<IPlugin> Plugins {
			get {
				return plugins;
			}
		}

		void ThreadProc() {
			// This is the scanner's thread. It performs all the work.

			// Step 1: Collect the scanners from the plugins.
			List<IScanner> scanners = new List<IScanner>();
			foreach (IPlugin plugin in plugins) {
				if (plugin is IScanner) {
					scanners.Add(plugin as IScanner);
				}
			}

			// Step 2: Collect the displayers from the plugins.
			List<IDisplay> displays = new List<IDisplay>();
			foreach (IPlugin plugin in plugins) {
				if (plugin is IDisplay) {
					displays.Add(plugin as IDisplay);
				}
			}

			// Step 3: Set up initial progress values.
			// Add 1 to the maximum returned from each scanner so we can do
			// a final step for each, and in case the scanner really doesn't
			// know how many steps it'll take yet.
			long maximum = 0;
			foreach (IScanner scanner in scanners) {
				maximum += scanner.Maximum + 1;
			}

			long current = 0;
			if (OnProgress != null) OnProgress(this, new ProgressEventArgs(maximum, current));

			foreach (IScanner scanner in scanners) {
				while (scanner.Current <= scanner.Maximum) {
					//OnOutput(this, new OutputEventArgs(new DisplayItem(DisplayItemSeverity.Verbose, "Process Before", "Current: " + current + "  Maximum: " + maximum)));
					maximum -= scanner.Maximum;
					current -= scanner.Current;
					List<IScanItem> items = scanner.Process();
					maximum += scanner.Maximum;
					current += scanner.Current;
					//OnOutput(this, new OutputEventArgs(new DisplayItem(DisplayItemSeverity.Verbose, "Process After", "Current: " + current + "  Maximum: " + maximum)));

					// Process each scan item into a display item using the first display
					// module which accepts it.
					if ((items != null) && (OnOutput != null)) {
						foreach (IScanItem item in items) {
							foreach (IDisplay display in displays) {
								if (display.Accepts(item)) {
									OnOutput(this, new OutputEventArgs(display.Process(item)));
									break;
								}
							}
						}
					}

					if (OnProgress != null) OnProgress(this, new ProgressEventArgs(maximum, current));
				}
			}

			if (OnProgress != null) OnProgress(this, new ProgressEventArgs(maximum, current));
			if (OnComplete != null) OnComplete(this, new EventArgs());
		}

		public class ProgressEventArgs
		{
			public long Maximum;
			public long Current;

			public ProgressEventArgs(long maximum, long current) {
				Maximum = maximum;
				Current = current;
			}
		}

		public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
		public event ProgressEventHandler OnProgress;

		public class OutputEventArgs
		{
			public IDisplayItem Item;

			public OutputEventArgs(IDisplayItem item) {
				Item = item;
			}
		}

		public delegate void OutputEventHandler(object sender, OutputEventArgs e);
		public event OutputEventHandler OnOutput;

		public event EventHandler OnComplete;
	}
}
