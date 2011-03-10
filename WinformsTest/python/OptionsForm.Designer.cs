namespace RhinoDLR_Python
{
  partial class OptionsForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.Button btnCancel;
      System.Windows.Forms.Button btnOk;
      System.Windows.Forms.TabPage m_pagePaths;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
      this.label4 = new System.Windows.Forms.Label();
      this.m_updownMruFilesInList = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label2 = new System.Windows.Forms.Label();
      this.m_updownMruFilesAtStart = new System.Windows.Forms.NumericUpDown();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.m_btnAddToSearchPath = new System.Windows.Forms.ToolStripButton();
      this.m_btnRemoveFromSearchPath = new System.Windows.Forms.ToolStripButton();
      this.m_btnMoveUpInSearchPath = new System.Windows.Forms.ToolStripButton();
      this.m_btnMoveDownInSearchPath = new System.Windows.Forms.ToolStripButton();
      this.m_btnOpenPath = new System.Windows.Forms.ToolStripButton();
      this.label1 = new System.Windows.Forms.Label();
      this.m_listPaths = new System.Windows.Forms.ListBox();
      this.m_tabOptions = new System.Windows.Forms.TabControl();
      this.m_pageEditor = new System.Windows.Forms.TabPage();
      this.label6 = new System.Windows.Forms.Label();
      this.m_comboFontSizes = new System.Windows.Forms.ComboBox();
      this.m_comboFontNames = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.m_chkConvertTabsToSpaces = new System.Windows.Forms.CheckBox();
      btnCancel = new System.Windows.Forms.Button();
      btnOk = new System.Windows.Forms.Button();
      m_pagePaths = new System.Windows.Forms.TabPage();
      m_pagePaths.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_updownMruFilesInList)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_updownMruFilesAtStart)).BeginInit();
      this.toolStrip1.SuspendLayout();
      this.m_tabOptions.SuspendLayout();
      this.m_pageEditor.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      btnCancel.Location = new System.Drawing.Point(417, 400);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new System.Drawing.Size(75, 23);
      btnCancel.TabIndex = 1;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      btnOk.Location = new System.Drawing.Point(336, 400);
      btnOk.Name = "btnOk";
      btnOk.Size = new System.Drawing.Size(75, 23);
      btnOk.TabIndex = 0;
      btnOk.Text = "OK";
      btnOk.UseVisualStyleBackColor = true;
      // 
      // m_pagePaths
      // 
      m_pagePaths.Controls.Add(this.label4);
      m_pagePaths.Controls.Add(this.m_updownMruFilesInList);
      m_pagePaths.Controls.Add(this.label3);
      m_pagePaths.Controls.Add(this.pictureBox1);
      m_pagePaths.Controls.Add(this.label2);
      m_pagePaths.Controls.Add(this.m_updownMruFilesAtStart);
      m_pagePaths.Controls.Add(this.toolStrip1);
      m_pagePaths.Controls.Add(this.label1);
      m_pagePaths.Controls.Add(this.m_listPaths);
      m_pagePaths.Location = new System.Drawing.Point(4, 22);
      m_pagePaths.Name = "m_pagePaths";
      m_pagePaths.Padding = new System.Windows.Forms.Padding(3);
      m_pagePaths.Size = new System.Drawing.Size(472, 356);
      m_pagePaths.TabIndex = 0;
      m_pagePaths.Text = "Files";
      m_pagePaths.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(83, 243);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(112, 13);
      this.label4.TabIndex = 17;
      this.label4.Text = "Files in most recent list";
      // 
      // m_updownMruFilesInList
      // 
      this.m_updownMruFilesInList.Location = new System.Drawing.Point(33, 241);
      this.m_updownMruFilesInList.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.m_updownMruFilesInList.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.m_updownMruFilesInList.Name = "m_updownMruFilesInList";
      this.m_updownMruFilesInList.Size = new System.Drawing.Size(43, 20);
      this.m_updownMruFilesInList.TabIndex = 3;
      this.m_updownMruFilesInList.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(83, 218);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(102, 13);
      this.label3.TabIndex = 15;
      this.label3.Text = "Files to open at start";
      // 
      // pictureBox1
      // 
      this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox1.Location = new System.Drawing.Point(10, 174);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(453, 2);
      this.pictureBox1.TabIndex = 14;
      this.pictureBox1.TabStop = false;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(7, 188);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(92, 13);
      this.label2.TabIndex = 13;
      this.label2.Text = "Most Recent Files";
      // 
      // m_updownMruFilesAtStart
      // 
      this.m_updownMruFilesAtStart.Location = new System.Drawing.Point(33, 214);
      this.m_updownMruFilesAtStart.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.m_updownMruFilesAtStart.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.m_updownMruFilesAtStart.Name = "m_updownMruFilesAtStart";
      this.m_updownMruFilesAtStart.Size = new System.Drawing.Size(43, 20);
      this.m_updownMruFilesAtStart.TabIndex = 2;
      this.m_updownMruFilesAtStart.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
      // 
      // toolStrip1
      // 
      this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
      //this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnAddToSearchPath,
            this.m_btnRemoveFromSearchPath,
            this.m_btnMoveUpInSearchPath,
            this.m_btnMoveDownInSearchPath,
            this.m_btnOpenPath});
      //this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
      this.toolStrip1.Location = new System.Drawing.Point(353, 121);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(118, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // m_btnAddToSearchPath
      // 
      this.m_btnAddToSearchPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_btnAddToSearchPath.Image = ((System.Drawing.Image)(resources.GetObject("m_btnAddToSearchPath.Image")));
      this.m_btnAddToSearchPath.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_btnAddToSearchPath.Name = "m_btnAddToSearchPath";
      this.m_btnAddToSearchPath.Size = new System.Drawing.Size(23, 22);
      this.m_btnAddToSearchPath.ToolTipText = "Add to search path";
      this.m_btnAddToSearchPath.Click += new System.EventHandler(this.OnAddToSearchPath);
      // 
      // m_btnRemoveFromSearchPath
      // 
      this.m_btnRemoveFromSearchPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_btnRemoveFromSearchPath.Enabled = false;
      this.m_btnRemoveFromSearchPath.Image = ((System.Drawing.Image)(resources.GetObject("m_btnRemoveFromSearchPath.Image")));
      this.m_btnRemoveFromSearchPath.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_btnRemoveFromSearchPath.Name = "m_btnRemoveFromSearchPath";
      this.m_btnRemoveFromSearchPath.Size = new System.Drawing.Size(23, 22);
      this.m_btnRemoveFromSearchPath.ToolTipText = "Remove from search path";
      this.m_btnRemoveFromSearchPath.Click += new System.EventHandler(this.OnRemoveFromSearchPath);
      // 
      // m_btnMoveUpInSearchPath
      // 
      this.m_btnMoveUpInSearchPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_btnMoveUpInSearchPath.Enabled = false;
      this.m_btnMoveUpInSearchPath.Image = ((System.Drawing.Image)(resources.GetObject("m_btnMoveUpInSearchPath.Image")));
      this.m_btnMoveUpInSearchPath.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_btnMoveUpInSearchPath.Name = "m_btnMoveUpInSearchPath";
      this.m_btnMoveUpInSearchPath.Size = new System.Drawing.Size(23, 22);
      this.m_btnMoveUpInSearchPath.ToolTipText = "Move up in search order";
      this.m_btnMoveUpInSearchPath.Click += new System.EventHandler(this.OnMoveUpInSearchPath);
      // 
      // m_btnMoveDownInSearchPath
      // 
      this.m_btnMoveDownInSearchPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_btnMoveDownInSearchPath.Enabled = false;
      this.m_btnMoveDownInSearchPath.Image = ((System.Drawing.Image)(resources.GetObject("m_btnMoveDownInSearchPath.Image")));
      this.m_btnMoveDownInSearchPath.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_btnMoveDownInSearchPath.Name = "m_btnMoveDownInSearchPath";
      this.m_btnMoveDownInSearchPath.Size = new System.Drawing.Size(23, 22);
      this.m_btnMoveDownInSearchPath.ToolTipText = "Move down in search order";
      this.m_btnMoveDownInSearchPath.Click += new System.EventHandler(this.OnMoveDownInSearchPath);
      // 
      // m_btnOpenPath
      // 
      this.m_btnOpenPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_btnOpenPath.Enabled = false;
      this.m_btnOpenPath.Image = ((System.Drawing.Image)(resources.GetObject("m_btnOpenPath.Image")));
      this.m_btnOpenPath.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_btnOpenPath.Name = "m_btnOpenPath";
      this.m_btnOpenPath.Size = new System.Drawing.Size(23, 22);
      this.m_btnOpenPath.ToolTipText = "Open in file explorer";
      this.m_btnOpenPath.Click += new System.EventHandler(this.OnOpenPath);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(7, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(109, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Module Search Paths";
      // 
      // m_listPaths
      // 
      this.m_listPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_listPaths.FormattingEnabled = true;
      this.m_listPaths.HorizontalScrollbar = true;
      this.m_listPaths.Location = new System.Drawing.Point(33, 23);
      this.m_listPaths.Name = "m_listPaths";
      this.m_listPaths.Size = new System.Drawing.Size(433, 95);
      this.m_listPaths.TabIndex = 0;
      this.m_listPaths.SelectedIndexChanged += new System.EventHandler(this.OnSearchPathItemSelect);
      this.m_listPaths.DoubleClick += new System.EventHandler(this.ItemDoubleClick);
      // 
      // m_tabOptions
      // 
      this.m_tabOptions.Controls.Add(m_pagePaths);
      this.m_tabOptions.Controls.Add(this.m_pageEditor);
      this.m_tabOptions.Location = new System.Drawing.Point(12, 12);
      this.m_tabOptions.Name = "m_tabOptions";
      this.m_tabOptions.SelectedIndex = 0;
      this.m_tabOptions.Size = new System.Drawing.Size(480, 382);
      this.m_tabOptions.TabIndex = 10;
      // 
      // m_pageEditor
      // 
      this.m_pageEditor.Controls.Add(this.m_chkConvertTabsToSpaces);
      this.m_pageEditor.Controls.Add(this.label6);
      this.m_pageEditor.Controls.Add(this.m_comboFontSizes);
      this.m_pageEditor.Controls.Add(this.m_comboFontNames);
      this.m_pageEditor.Controls.Add(this.label5);
      this.m_pageEditor.Location = new System.Drawing.Point(4, 22);
      this.m_pageEditor.Name = "m_pageEditor";
      this.m_pageEditor.Padding = new System.Windows.Forms.Padding(3);
      this.m_pageEditor.Size = new System.Drawing.Size(472, 356);
      this.m_pageEditor.TabIndex = 1;
      this.m_pageEditor.Text = "Text Editor";
      this.m_pageEditor.UseVisualStyleBackColor = true;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(368, 26);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(27, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "Size";
      // 
      // m_comboFontSizes
      // 
      this.m_comboFontSizes.FormattingEnabled = true;
      this.m_comboFontSizes.Location = new System.Drawing.Point(401, 23);
      this.m_comboFontSizes.Name = "m_comboFontSizes";
      this.m_comboFontSizes.Size = new System.Drawing.Size(65, 21);
      this.m_comboFontSizes.TabIndex = 3;
      // 
      // m_comboFontNames
      // 
      this.m_comboFontNames.FormattingEnabled = true;
      this.m_comboFontNames.Location = new System.Drawing.Point(33, 23);
      this.m_comboFontNames.Name = "m_comboFontNames";
      this.m_comboFontNames.Size = new System.Drawing.Size(295, 21);
      this.m_comboFontNames.TabIndex = 2;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(7, 7);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(28, 13);
      this.label5.TabIndex = 0;
      this.label5.Text = "Font";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(13, 400);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(102, 23);
      this.button1.TabIndex = 11;
      this.button1.Text = "Restore Defaults";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.OnRestoreDefaults);
      // 
      // m_chkConvertTabsToSpaces
      // 
      this.m_chkConvertTabsToSpaces.AutoSize = true;
      this.m_chkConvertTabsToSpaces.Checked = true;
      this.m_chkConvertTabsToSpaces.CheckState = System.Windows.Forms.CheckState.Checked;
      this.m_chkConvertTabsToSpaces.Location = new System.Drawing.Point(10, 65);
      this.m_chkConvertTabsToSpaces.Name = "m_chkConvertTabsToSpaces";
      this.m_chkConvertTabsToSpaces.Size = new System.Drawing.Size(135, 17);
      this.m_chkConvertTabsToSpaces.TabIndex = 5;
      this.m_chkConvertTabsToSpaces.Text = "Convert tabs to spaces";
      this.m_chkConvertTabsToSpaces.UseVisualStyleBackColor = true;
      // 
      // OptionsForm
      // 
      this.AcceptButton = btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = btnCancel;
      this.ClientSize = new System.Drawing.Size(504, 435);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.m_tabOptions);
      this.Controls.Add(btnOk);
      this.Controls.Add(btnCancel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "OptionsForm";
      this.ShowInTaskbar = false;
      this.Text = "Python Options";
      m_pagePaths.ResumeLayout(false);
      m_pagePaths.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_updownMruFilesInList)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_updownMruFilesAtStart)).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.m_tabOptions.ResumeLayout(false);
      this.m_pageEditor.ResumeLayout(false);
      this.m_pageEditor.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown m_updownMruFilesInList;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown m_updownMruFilesAtStart;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton m_btnAddToSearchPath;
    private System.Windows.Forms.ToolStripButton m_btnRemoveFromSearchPath;
    private System.Windows.Forms.ToolStripButton m_btnMoveUpInSearchPath;
    private System.Windows.Forms.ToolStripButton m_btnMoveDownInSearchPath;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox m_listPaths;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TabControl m_tabOptions;
    private System.Windows.Forms.ToolStripButton m_btnOpenPath;
    private System.Windows.Forms.TabPage m_pageEditor;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ComboBox m_comboFontNames;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ComboBox m_comboFontSizes;
    private System.Windows.Forms.CheckBox m_chkConvertTabsToSpaces;



  }
}