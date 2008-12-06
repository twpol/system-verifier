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
	public partial class VerifierGUI : Form, IPluginHost
	{
		public VerifierGUI() {
			InitializeComponent();
		}

		PluginFinder pfinder;
		Dictionary<string, KeyValuePair<Type, IPlugin>> plugins;
		Scanner scanner;

		private void VerifierGUI_Load(object sender, EventArgs e) {
			pfinder = new PluginFinder();
			pfinder.OnStart += new EventHandler(pfinder_OnStart);
			pfinder.OnIsPluginTrusted += new PluginFinder.IsPluginTrustedEventHandler(pfinder_OnIsPluginTrusted);
			pfinder.OnPluginFound += new PluginFinder.PluginFoundEventHandler(pfinder_OnPluginFound);
			pfinder.OnStop += new EventHandler(pfinder_OnStop);
			plugins = new Dictionary<string, KeyValuePair<Type, IPlugin>>();

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
				if (plugin is IPluginWithHost) {
					(plugin as IPluginWithHost).Init(this);
				}
				plugins.Add(e.type.AssemblyQualifiedName, new KeyValuePair<Type, IPlugin>(e.type, plugin));
				
				TreeNode node = trePlugins.Nodes.Add(plugin.Name);
				node.Tag = e.type.AssemblyQualifiedName;
				node.Checked = true;

				if (plugin is IPluginWithSections) {
					List<KeyValuePair<string, long>> sections = (plugin as IPluginWithSections).Sections;
					foreach (KeyValuePair<string, long> section in sections) {
						ForceTreeEntry(node, section.Key, section.Value);
					}
				}

				if (trePlugins.SelectedNode == null) trePlugins.SelectedNode = node;
			}
		}

		void ForceTreeEntry(TreeNode root, string path, object tag) {
			string[] pathLevels = path.Split('\\');

			TreeNode node = null;
			foreach (TreeNode child in root.Nodes) {
				if (child.Text == pathLevels[0]) {
					if (pathLevels.Length == 1) {
						return;
					}
					node = child;
					break;
				}
			}

			if (node == null) {
				node = root.Nodes.Add(pathLevels[0]);
				if (pathLevels.Length == 1) {
					node.Tag = tag;
				}
				node.Checked = true;
			}

			if (pathLevels.Length > 1) {
				ForceTreeEntry(node, String.Join("\\", pathLevels, 1, pathLevels.Length - 1), tag);
			}
		}

		void pfinder_OnStop(object sender, EventArgs e) {
			if (lblStatus.InvokeRequired) {
				lblStatus.Invoke(new EventHandler(pfinder_OnStop), new object[] { sender, e });
			} else {
				lblStatus.Visible = false;
				trePlugins.ExpandAll();
				//if (pfinder.Plugins.Count > 0) {
				btnStart.Enabled = true;
				//}
			}
		}

		private void trePlugins_AfterSelect(object sender, TreeViewEventArgs e) {
			// Find the "root" node so that we know which plugin it is for, as
			// other nodes are tagged with section IDs.
			TreeNode rootNode = e.Node;
			while (rootNode.Parent != null) {
				rootNode = rootNode.Parent;
			}

			// Pull the plugin out of the collection using the Tag (name).
			IPlugin plugin = plugins[(string)rootNode.Tag].Value;

			lblPluginName.Text = plugin.Name;
			lblPluginDesc.Text = plugin.Description;
			lblPluginAuthors.Text = String.Join(", ", plugin.Authors);
		}

		private void trePlugins_AfterCheck(object sender, TreeViewEventArgs e) {
			foreach (TreeNode child in e.Node.Nodes) {
				child.Checked = e.Node.Checked;
			}
		}

		private void lstResults_KeyDown(object sender, KeyEventArgs e) {
			if ((e.Control && (e.KeyCode == Keys.C)) || (e.Control && (e.KeyCode == Keys.Insert))) {
				List<string> dataCSV = new List<string>();
				List<string> dataText = new List<string>();
				{
					List<string> row = new List<string>();
					foreach (ColumnHeader col in lstResults.Columns) {
						row.Add(col.Text);
					}
					dataCSV.Add(String.Join(",", row.ToArray()));
				}
				foreach (ListViewItem item in lstResults.Items) {
					if (item.Selected) {
						List<string> row = new List<string>();
						foreach (ListViewItem.ListViewSubItem cell in item.SubItems) {
							row.Add(cell.Text);
						}
						dataCSV.Add(String.Join(",", row.ToArray()));
						dataText.Add(String.Join("\t", row.ToArray()));
					}
				}

				Clipboard.Clear();
				Clipboard.SetText(String.Join("\n", dataCSV.ToArray()), TextDataFormat.CommaSeparatedValue);
				Clipboard.SetText(String.Join("\n", dataText.ToArray()), TextDataFormat.UnicodeText);
			}
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
			lblResultsName.Text = "";
			lblResultsDescription.Text = "";

			scanner = new Scanner();
			scanner.OnProgress += new Scanner.ProgressEventHandler(scanner_Progress);
			scanner.OnOutput += new Scanner.OutputEventHandler(scanner_Output);
			scanner.OnComplete += new EventHandler(scanner_Complete);
			foreach (TreeNode node in trePlugins.Nodes) {
				if (node.Checked) {
					ConstructorInfo pluginCtor = plugins[(string)node.Tag].Key.GetConstructor(new Type[] { });
					IPlugin plugin = (IPlugin)pluginCtor.Invoke(new object[] { });
					if (plugin is IPluginWithHost) {
						(plugin as IPluginWithHost).Init(this);
					}
					if (plugin is IPluginWithSections) {
						List<long> sections = new List<long>();
						CollectSections(ref sections, node);
						(plugin as IPluginWithSections).SetSections(sections);
					}
					scanner.Plugins.Add(plugin);
				}
			}
			scanner.Start();
		}

		void CollectSections(ref List<long> sections, TreeNode root) {
			foreach (TreeNode child in root.Nodes) {
				if (child.Checked) {
					sections.Add((long)child.Tag);
				}
				CollectSections(ref sections, child);
			}
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

		#region IPluginHost Members

		public string OS {
			get {
				return "Windows";
			}
		}

		public long OSMinor {
			get {
				return Environment.OSVersion.Version.Minor;
			}
		}

		public long OSMajor {
			get {
				return Environment.OSVersion.Version.Major;
			}
		}

		public long Bitness {
			get {
				return 8 * IntPtr.Size;
			}
		}

		#endregion
	}
}