namespace JGR.SystemVerifier
{
	partial class VerifierGUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerifierGUI));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.proStatus = new System.Windows.Forms.ProgressBar();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPluginAuthors = new System.Windows.Forms.Label();
            this.lblPluginName = new System.Windows.Forms.Label();
            this.lblPluginDesc = new System.Windows.Forms.Label();
            this.trePlugins = new System.Windows.Forms.TreeView();
            this.tabResults = new System.Windows.Forms.TabPage();
            this.lblResultsName = new System.Windows.Forms.Label();
            this.lblResultsDescription = new System.Windows.Forms.Label();
            this.lstResults = new System.Windows.Forms.ListView();
            this.colResultsName = new System.Windows.Forms.ColumnHeader();
            this.colResultsDesc = new System.Windows.Forms.ColumnHeader();
            this.imlResults = new System.Windows.Forms.ImageList(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.imlPlugins = new System.Windows.Forms.ImageList(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabConfig.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 26);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(22, 13);
            label1.TabIndex = 5;
            label1.Text = "By:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(497, 329);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(335, 329);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // proStatus
            // 
            this.proStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.proStatus.Location = new System.Drawing.Point(12, 329);
            this.proStatus.Name = "proStatus";
            this.proStatus.Size = new System.Drawing.Size(317, 23);
            this.proStatus.TabIndex = 7;
            this.proStatus.Visible = false;
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabConfig);
            this.tabs.Controls.Add(this.tabResults);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(560, 311);
            this.tabs.TabIndex = 8;
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.tableLayoutPanel1);
            this.tabConfig.Controls.Add(this.trePlugins);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfig.Size = new System.Drawing.Size(552, 285);
            this.tabConfig.TabIndex = 0;
            this.tabConfig.Text = "Configuration";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblPluginAuthors, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPluginName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPluginDesc, 0, 1);
            this.tableLayoutPanel1.Controls.Add(label1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(316, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(230, 273);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lblPluginAuthors
            // 
            this.lblPluginAuthors.AutoSize = true;
            this.lblPluginAuthors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPluginAuthors.Location = new System.Drawing.Point(31, 26);
            this.lblPluginAuthors.Name = "lblPluginAuthors";
            this.lblPluginAuthors.Size = new System.Drawing.Size(196, 13);
            this.lblPluginAuthors.TabIndex = 6;
            this.lblPluginAuthors.Text = "<no plugin selected>";
            // 
            // lblPluginName
            // 
            this.lblPluginName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblPluginName, 2);
            this.lblPluginName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPluginName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPluginName.Location = new System.Drawing.Point(3, 0);
            this.lblPluginName.Name = "lblPluginName";
            this.lblPluginName.Size = new System.Drawing.Size(224, 13);
            this.lblPluginName.TabIndex = 3;
            this.lblPluginName.Text = "<no plugin selected>";
            // 
            // lblPluginDesc
            // 
            this.lblPluginDesc.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblPluginDesc, 2);
            this.lblPluginDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPluginDesc.Location = new System.Drawing.Point(3, 13);
            this.lblPluginDesc.Name = "lblPluginDesc";
            this.lblPluginDesc.Size = new System.Drawing.Size(224, 13);
            this.lblPluginDesc.TabIndex = 4;
            this.lblPluginDesc.Text = "<no plugin selected>";
            // 
            // trePlugins
            // 
            this.trePlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trePlugins.CheckBoxes = true;
            this.trePlugins.HideSelection = false;
            this.trePlugins.Location = new System.Drawing.Point(6, 6);
            this.trePlugins.Name = "trePlugins";
            this.trePlugins.ShowLines = false;
            this.trePlugins.ShowPlusMinus = false;
            this.trePlugins.ShowRootLines = false;
            this.trePlugins.Size = new System.Drawing.Size(304, 273);
            this.trePlugins.TabIndex = 0;
            this.trePlugins.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trePlugins_AfterCheck);
            this.trePlugins.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trePlugins_AfterSelect);
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.lblResultsName);
            this.tabResults.Controls.Add(this.lblResultsDescription);
            this.tabResults.Controls.Add(this.lstResults);
            this.tabResults.Location = new System.Drawing.Point(4, 22);
            this.tabResults.Name = "tabResults";
            this.tabResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabResults.Size = new System.Drawing.Size(552, 285);
            this.tabResults.TabIndex = 1;
            this.tabResults.Text = "Results";
            this.tabResults.UseVisualStyleBackColor = true;
            // 
            // lblResultsName
            // 
            this.lblResultsName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultsName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultsName.Location = new System.Drawing.Point(6, 220);
            this.lblResultsName.Name = "lblResultsName";
            this.lblResultsName.Size = new System.Drawing.Size(540, 16);
            this.lblResultsName.TabIndex = 2;
            // 
            // lblResultsDescription
            // 
            this.lblResultsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultsDescription.Location = new System.Drawing.Point(6, 236);
            this.lblResultsDescription.Name = "lblResultsDescription";
            this.lblResultsDescription.Size = new System.Drawing.Size(540, 46);
            this.lblResultsDescription.TabIndex = 1;
            // 
            // lstResults
            // 
            this.lstResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colResultsName,
            this.colResultsDesc});
            this.lstResults.FullRowSelect = true;
            this.lstResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstResults.HideSelection = false;
            this.lstResults.Location = new System.Drawing.Point(6, 6);
            this.lstResults.MultiSelect = false;
            this.lstResults.Name = "lstResults";
            this.lstResults.ShowItemToolTips = true;
            this.lstResults.Size = new System.Drawing.Size(540, 211);
            this.lstResults.SmallImageList = this.imlResults;
            this.lstResults.TabIndex = 0;
            this.lstResults.UseCompatibleStateImageBehavior = false;
            this.lstResults.View = System.Windows.Forms.View.Details;
            this.lstResults.SelectedIndexChanged += new System.EventHandler(this.lstResults_SelectedIndexChanged);
            this.lstResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstResults_KeyDown);
            // 
            // colResultsName
            // 
            this.colResultsName.Text = "Name";
            this.colResultsName.Width = 100;
            // 
            // colResultsDesc
            // 
            this.colResultsDesc.Text = "Description";
            this.colResultsDesc.Width = 400;
            // 
            // imlResults
            // 
            this.imlResults.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlResults.ImageStream")));
            this.imlResults.TransparentColor = System.Drawing.Color.Transparent;
            this.imlResults.Images.SetKeyName(0, "Severity - Information.png");
            this.imlResults.Images.SetKeyName(1, "Severity - Information.png");
            this.imlResults.Images.SetKeyName(2, "Severity - Warning.png");
            this.imlResults.Images.SetKeyName(3, "Severity - Error.png");
            this.imlResults.Images.SetKeyName(4, "Severity - Error.png");
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(416, 329);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // imlPlugins
            // 
            this.imlPlugins.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imlPlugins.ImageSize = new System.Drawing.Size(16, 16);
            this.imlPlugins.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(12, 329);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(317, 23);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Starting...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VerifierGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 364);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.proStatus);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "VerifierGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Verifier";
            this.Load += new System.EventHandler(this.VerifierGUI_Load);
            this.tabs.ResumeLayout(false);
            this.tabConfig.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabResults.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.ProgressBar proStatus;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabConfig;
		private System.Windows.Forms.TabPage tabResults;
		private System.Windows.Forms.ListView lstResults;
		private System.Windows.Forms.ColumnHeader colResultsName;
		private System.Windows.Forms.ColumnHeader colResultsDesc;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Label lblResultsDescription;
		private System.Windows.Forms.Label lblResultsName;
		private System.Windows.Forms.ImageList imlResults;
		private System.Windows.Forms.ImageList imlPlugins;
        private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.TreeView trePlugins;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPluginAuthors;
        private System.Windows.Forms.Label lblPluginName;
        private System.Windows.Forms.Label lblPluginDesc;
	}
}

