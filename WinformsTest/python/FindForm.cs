using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.Forms
{
  partial class FindForm : Form
  {
    //RhinoDLR_Python.ScriptForm m_parent_form;
    public FindForm()//RhinoDLR_Python.ScriptForm parent)
    {
      InitializeComponent();
      //m_parent_form = parent;
    }

    private void OnFindNext(object sender, EventArgs e)
    {
      //m_parent_form.FindText( m_txtFindString.Text, m_chkMatchCase.Checked, true);
    }

    private void OnCancel(object sender, EventArgs e)
    {
      Close();
    }

    private void OnShown(object sender, EventArgs e)
    {
      m_txtFindString.SelectAll();
    }
  }
}
