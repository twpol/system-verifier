using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JGR.SystemVerifier.Core;
using JGR.SystemVerifier.Plugins;
using System.Reflection;

namespace JGR.SystemVerifier
{
	public partial class VerifierGUI : Form
	{
		public VerifierGUI() {
			InitializeComponent();
		}

		PluginFinder pfinder;
		Dictionary<string, IPlugin> plugins;
		Scanner scanner;

		private void VerifierGUI_Load(object sender, EventArgs e) {
			pfinder = new PluginFinder();
			pfinder.OnStart += new EventHandler(pfinder_OnStart);
			pfinder.OnIsPluginTrusted += new PluginFinder.IsPluginTrustedEventHandler(pfinder_OnIsPluginTrusted);
			pfinder.OnPluginFound += new PluginFinder.PluginFoundEventHandler(pfinder_OnPluginFound);
			pfinder.OnStop += new EventHandler(pfinder_OnStop);
			plugins = new Dictionary<string, IPlugin>();

			pfinder.Start();
		}

		void pfinder_OnStart(object sender, EventArgs e) {
			if (lblStatus.InvokeRequired) {
				lblStatus.Invoke(new EventHandler(pfinder_OnStart), new object[] { sender, e });
			} else {
				lblStatus.Visible = true;
				lblStatus.Text = "Looking for plugins...";
			}
		}

		bool pfinder_OnIsPluginTrusted(object sender, PluginFinder.IsPluginTrustedEventArgs e) {
			if (lblStatus.InvokeRequired) {
				return (bool)lblStatus.Invoke(new PluginFinder.IsPluginTrustedEventHandler(pfinder_OnIsPluginTrusted), new object[] { sender, e });
			} else {
				//PluginTrust trustWin = new PluginTrust();
				//trustWin.txtFilename.Text = e.Filename;
				//trustWin.txtFullName.Text = e.Fullname;
				//DialogResult rv = trustWin.ShowDialog();
				//return (rv == DialogResult.Yes);
				return true;
			}
		}

		void pfinder_OnPluginFound(object sender, PluginFinder.PluginFoundEventArgs e) {
			if (lblStatus.InvokeRequired) {
				lblStatus.Invoke(new PluginFinder.PluginFoundEventHandler(pfinder_OnPluginFound), new object[] { sender, e });
			} else {
				ConstructorInfo pluginCtor = e.type.GetConstructor(new Type[] { });
				IPlugin plugin = (IPlugin)pluginCtor.Invoke(new object[] { });
				plugins.Add(e.type.AssemblyQualifiedName, plugin);
				TreeNode node = trePlugins.Nodes.Add(plugin.Name);
				node.Tag = e.type.AssemblyQualifiedName;
				node.Checked = true;
				node.Expand();

				if (trePlugins.SelectedNode == null) trePlugins.SelectedNode = node;
			}
		}

		void pfinder_OnStop(object sender, EventArgs e) {
			if (lblStatus.InvokeRequired) {
				lblStatus.Invoke(new EventHandler(pfinder_OnStop), new object[] { sender, e });
			} else {
				lblStatus.Visible = false;
				//if (pfinder.Plugins.Count > 0) {
				btnStart.Enabled = true;
				//}
			}
		}

		private void trePlugins_AfterSelect(object sender, TreeViewEventArgs e) {
			IPlugin plugin = plugins[(string)e.Node.Tag];
			lblPluginName.Text = plugin.Name;
			lblPluginDesc.Text = plugin.Description;
			string authors = "";
			foreach (string author in plugin.Authors) {
				if (authors != "") authors += ", ";
				authors += author;
			}
			lblPluginAuthors.Text = authors;
		}

		private void btnClose_Click(object sender, EventArgs e) {
			if (btnStop.Enabled) {
				btnStop_Click(sender, e);
			}
			Close();
		}

		private void btnStart_Click(object sender, EventArgs e) {
			proStatus.Minimum = 0;
			proStatus.Maximum = 1;
			proStatus.Value = 0;
			proStatus.Visible = true;
			tabs.SelectedTab = tabResults;
			btnStart.Enabled = false;
			btnStop.Enabled = true;

			lstResults.Items.Clear();

			scanner = new Scanner();
			scanner.OnProgress += new Scanner.ProgressEventHandler(scanner_Progress);
			scanner.OnOutput += new Scanner.OutputEventHandler(scanner_Output);
			scanner.OnComplete += new EventHandler(scanner_Complete);
			//scanner.Modules.Add(new Test());
			scanner.Start();
		}

		void scanner_Progress(object sender, Scanner.ProgressEventArgs e) {
			if (proStatus.InvokeRequired) {
				proStatus.Invoke(new Scanner.ProgressEventHandler(scanner_Progress), new object[] { sender, e });
			} else {
				proStatus.Maximum = (int)e.Maximum;
				proStatus.Value = (int)e.Current;
			}
		}

		void scanner_Output(object sender, Scanner.OutputEventArgs e) {
			if (lstResults.InvokeRequired) {
				lstResults.Invoke(new Scanner.OutputEventHandler(scanner_Output), new object[] { sender, e });
			} else {
				lstResults.Items.Add(new ListViewItem(new string[] { e.Item.Name, e.Item.Description, e.Item.Severity.ToString() }));
			}
		}

		void scanner_Complete(object sender, EventArgs e) {
			if (proStatus.InvokeRequired) {
				proStatus.Invoke(new EventHandler(scanner_Complete), new object[] { sender, e });
			} else {
				scanner = null;

				proStatus.Visible = false;
				btnStop.Enabled = false;
				btnStart.Enabled = true;
			}
		}

		private void btnStop_Click(object sender, EventArgs e) {
			scanner.Stop();
		}

		private void lstResults_SelectedIndexChanged(object sender, EventArgs e) {
			if (lstResults.SelectedItems.Count > 0) {
				lblResultsName.Text = lstResults.SelectedItems[0].SubItems[0].Text + " (" + lstResults.SelectedItems[0].SubItems[2].Text + ")";
				lblResultsDescription.Text = lstResults.SelectedItems[0].SubItems[1].Text;
			} else {
				lblResultsName.Text = "";
				lblResultsDescription.Text = "";
			}
		}
	}
}