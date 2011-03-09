using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RhinoDLR_Python
{
  partial class ExceptionForm : Form
  {
    public ExceptionForm()
    {
      InitializeComponent();
    }

    public static void ShowException(System.Exception ex)
    {
      ExceptionForm f = new ExceptionForm();
      f.m_lblMessage.Text = "Message: " + ex.Message;

			f.m_txtTraceback.Text = ex.StackTrace;// RhinoPython.PythonScriptScope.GetStackTrace(ex);
      f.ShowDialog();
    }
  }
}
