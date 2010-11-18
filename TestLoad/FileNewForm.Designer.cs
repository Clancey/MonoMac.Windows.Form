using System.Windows.Forms;
namespace RhinoDLR_Python
{
  partial class FileNewForm : Form
  {
	public FileNewForm()
		{
			InitializeComponent();
		}
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>


    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      //this.m_groupDescription = new System.Windows.Forms.GroupBox();
      this.m_lblDescription = new System.Windows.Forms.Label();
      //this.m_listNewType = new System.Windows.Forms.ListBox();
      this.m_btnNew = new System.Windows.Forms.Button();
      this.m_btnCancel = new System.Windows.Forms.Button();
      this.m_txtCommandName = new System.Windows.Forms.TextBox();
      this.m_label_cmdname = new System.Windows.Forms.Label();
      this.m_label_plugin = new System.Windows.Forms.Label();
      this.m_combo_plugin = new System.Windows.Forms.ComboBox();
      //this.m_groupDescription.SuspendLayout();
      //this.SuspendLayout();
      // 
      // m_groupDescription
      // 
			/*
      this.m_groupDescription.Controls.Add(this.m_lblDescription);
      this.m_groupDescription.Location = new System.Drawing.Point(138, 12);
      this.m_groupDescription.Name = "m_groupDescription";
      this.m_groupDescription.Size = new System.Drawing.Size(278, 130);
      this.m_groupDescription.TabIndex = 3;
      this.m_groupDescription.TabStop = false;
      this.m_groupDescription.Text = "Description";
      */
      // 
      // m_lblDescription
      // 
      this.m_lblDescription.Location = new System.Drawing.Point(9, 16);
      this.m_lblDescription.Name = "m_lblDescription";
      this.m_lblDescription.Size = new System.Drawing.Size(261, 107);
      this.m_lblDescription.TabIndex = 2;
      this.m_lblDescription.Text = "description";
      // 
      // m_listNewType
      //
			/*
      this.m_listNewType.FormattingEnabled = true;
      this.m_listNewType.Items.AddRange(new object[] {
            "Command",
            "Empty Script"});
      this.m_listNewType.Location = new System.Drawing.Point(12, 12);
      this.m_listNewType.Name = "m_listNewType";
      this.m_listNewType.Size = new System.Drawing.Size(120, 212);
      this.m_listNewType.TabIndex = 0;
      this.m_listNewType.SelectedIndexChanged += new System.EventHandler(this.OnNewTypeSelectionChanged);
      */
      // 
      // m_btnNew
      // 
      this.m_btnNew.Location = new System.Drawing.Point(260, 200);
      this.m_btnNew.Name = "m_btnNew";
      this.m_btnNew.Size = new System.Drawing.Size(75, 23);
      this.m_btnNew.TabIndex = 4;
      this.m_btnNew.Text = "New";
      //this.m_btnNew.UseVisualStyleBackColor = true;
     // this.m_btnNew.Click += new System.EventHandler(this.OnNewButtonClickEvent);
      // 
      // m_btnCancel
      // 
      this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_btnCancel.Location = new System.Drawing.Point(341, 200);
      this.m_btnCancel.Name = "m_btnCancel";
      this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
      this.m_btnCancel.TabIndex = 5;
      this.m_btnCancel.Text = "Cancel";
     // this.m_btnCancel.UseVisualStyleBackColor = true;
      // 
      // m_txtCommandName
      // 
      this.m_txtCommandName.Location = new System.Drawing.Point(226, 148);
     // this.m_txtCommandName.MaxLength = 50;
      this.m_txtCommandName.Name = "m_txtCommandName";
      this.m_txtCommandName.Size = new System.Drawing.Size(190, 20);
      this.m_txtCommandName.TabIndex = 6;
      // 
      // m_label_cmdname
      // 
      this.m_label_cmdname.AutoSize = true;
      this.m_label_cmdname.Location = new System.Drawing.Point(135, 151);
      this.m_label_cmdname.Name = "m_label_cmdname";
      this.m_label_cmdname.Size = new System.Drawing.Size(85, 13);
      this.m_label_cmdname.TabIndex = 7;
      this.m_label_cmdname.Text = "Command Name";
      // 
      // m_label_plugin
      // 
      this.m_label_plugin.AutoSize = true;
      this.m_label_plugin.Location = new System.Drawing.Point(180, 177);
      this.m_label_plugin.Name = "m_label_plugin";
      this.m_label_plugin.Size = new System.Drawing.Size(40, 13);
      this.m_label_plugin.TabIndex = 8;
      this.m_label_plugin.Text = "Plug-In";
      // 
      // m_combo_plugin
      // 
     // this.m_combo_plugin.FormattingEnabled = true;
      this.m_combo_plugin.Location = new System.Drawing.Point(227, 175);
     // this.m_combo_plugin.Name = "m_combo_plugin";
      this.m_combo_plugin.Size = new System.Drawing.Size(189, 21);
      //this.m_combo_plugin.TabIndex = 9;
      // 
      // FileNewForm
      // 
      //this.AcceptButton = this.m_btnNew;
      //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      //this.CancelButton = this.m_btnCancel;
      this.ClientSize = new System.Drawing.Size(424, 237);
      this.Controls.Add(this.m_combo_plugin);
      this.Controls.Add(this.m_label_plugin);
      this.Controls.Add(this.m_label_cmdname);
      this.Controls.Add(this.m_txtCommandName);
      this.Controls.Add(this.m_btnCancel);
      this.Controls.Add(this.m_btnNew);
      //this.Controls.Add(this.m_groupDescription);
      //this.Controls.Add(this.m_listNewType);
     // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      //this.MaximizeBox = false;
      //this.MinimizeBox = false;
      this.Name = "FileNewForm";
     // this.ShowInTaskbar = false;
      this.Text = "New";
      //this.Shown += new System.EventHandler(this.OnFormShownEvent);
      //this.m_groupDescription.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    //private System.Windows.Forms.ListBox m_listNewType;
    private System.Windows.Forms.Label m_lblDescription;
    private System.Windows.Forms.Button m_btnNew;
    private System.Windows.Forms.Button m_btnCancel;
    private System.Windows.Forms.TextBox m_txtCommandName;
    private System.Windows.Forms.Label m_label_cmdname;
   // private System.Windows.Forms.GroupBox m_groupDescription;
    private System.Windows.Forms.Label m_label_plugin;
    private System.Windows.Forms.ComboBox m_combo_plugin;
  }
}