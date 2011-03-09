using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsTests
{
	public partial class TesterForm : Form
	{
		public TesterForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				ExceptionThrower();
			}
			catch (Exception ex)
			{
				RhinoDLR_Python.ExceptionForm.ShowException(ex);
			}
		}
		void ExceptionThrower()
		{
			throw new NotImplementedException("A test exception");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ScriptEditor.Model.NewScriptUserInterfaceArgs args = new ScriptEditor.Model.NewScriptUserInterfaceArgs();
			RhinoDLR_Python.FileNewForm.ShowFileNewDialog(sender, args);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ScriptEditor.Forms.FindForm frm = new ScriptEditor.Forms.FindForm();
			frm.ShowDialog();
		}

    private void button4_Click(object sender, EventArgs e)
    {
      RhinoDLR_Python.OptionsForm.ShowOptionsDialog(this);
    }
	}
}
