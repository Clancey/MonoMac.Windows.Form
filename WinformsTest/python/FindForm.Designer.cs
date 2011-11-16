namespace ScriptEditor.Forms
{
  partial class FindForm
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
      System.Windows.Forms.Label m_lblFindWhat;
      System.Windows.Forms.Button m_btnFindNext;
      System.Windows.Forms.Button m_btnCancel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindForm));
      this.m_txtFindString = new System.Windows.Forms.TextBox();
      this.m_chkMatchCase = new System.Windows.Forms.CheckBox();
      m_lblFindWhat = new System.Windows.Forms.Label();
      m_btnFindNext = new System.Windows.Forms.Button();
      m_btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // m_lblFindWhat
      // 
      m_lblFindWhat.AutoSize = true;
      m_lblFindWhat.Location = new System.Drawing.Point(10, 16);
      m_lblFindWhat.Name = "m_lblFindWhat";
      m_lblFindWhat.Size = new System.Drawing.Size(53, 13);
      m_lblFindWhat.TabIndex = 0;
      m_lblFindWhat.Text = "Fi&nd what";
      // 
      // m_btnFindNext
      // 
      m_btnFindNext.DialogResult = System.Windows.Forms.DialogResult.OK;
      m_btnFindNext.Location = new System.Drawing.Point(261, 10);
      m_btnFindNext.Name = "m_btnFindNext";
      m_btnFindNext.Size = new System.Drawing.Size(75, 23);
      m_btnFindNext.TabIndex = 2;
      m_btnFindNext.Text = "&Find Next";
      m_btnFindNext.UseVisualStyleBackColor = true;
      m_btnFindNext.Click += new System.EventHandler(this.OnFindNext);
      // 
      // m_btnCancel
      // 
      m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      m_btnCancel.Location = new System.Drawing.Point(261, 39);
      m_btnCancel.Name = "m_btnCancel";
      m_btnCancel.Size = new System.Drawing.Size(75, 23);
      m_btnCancel.TabIndex = 3;
      m_btnCancel.Text = "Cancel";
      m_btnCancel.UseVisualStyleBackColor = true;
      m_btnCancel.Click += new System.EventHandler(this.OnCancel);
      // 
      // m_txtFindString
      // 
      this.m_txtFindString.Location = new System.Drawing.Point(69, 12);
      this.m_txtFindString.Name = "m_txtFindString";
      this.m_txtFindString.Size = new System.Drawing.Size(186, 20);
      this.m_txtFindString.TabIndex = 1;
      // 
      // m_chkMatchCase
      // 
      this.m_chkMatchCase.AutoSize = true;
      this.m_chkMatchCase.Location = new System.Drawing.Point(12, 43);
      this.m_chkMatchCase.Name = "m_chkMatchCase";
      this.m_chkMatchCase.Size = new System.Drawing.Size(82, 17);
      this.m_chkMatchCase.TabIndex = 4;
      this.m_chkMatchCase.Text = "Match &case";
      this.m_chkMatchCase.UseVisualStyleBackColor = true;
      // 
      // FindForm
      // 
      this.AcceptButton = m_btnFindNext;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = m_btnCancel;
      this.ClientSize = new System.Drawing.Size(348, 75);
      this.Controls.Add(this.m_chkMatchCase);
      this.Controls.Add(m_btnCancel);
      this.Controls.Add(m_btnFindNext);
      this.Controls.Add(this.m_txtFindString);
      this.Controls.Add(m_lblFindWhat);
      //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FindForm";
      //this.ShowInTaskbar = false;
      this.Text = "Find";
      //this.Shown += new System.EventHandler(this.OnShown);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox m_txtFindString;
    private System.Windows.Forms.CheckBox m_chkMatchCase;
  }
}