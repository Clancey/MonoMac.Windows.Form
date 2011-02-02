using System;
using System.Drawing;
using System.Windows.Forms;

namespace ListboxSample
{
	class MainClass : Form
	{
		
  		public ListBox listBox1;
		public MainClass()
		{
			this.listBox1 = new System.Windows.Forms.ListBox();
	        this.SuspendLayout();
	        // 
	        // listBox1
	        // 
	        this.listBox1.FormattingEnabled = true;
	        this.listBox1.HorizontalScrollbar = true;
	        this.listBox1.Items.AddRange(new object[] {
	            "Item 1, column 1",
	            "Item 2, column 1",
	            "Item 3, column 1",
	            "Item 4, column 1",
	            "Item 5, column 1",
	            "Item 1, column 2",
	            "Item 2, column 2",
	            "Item 3, column 2"});
	        this.listBox1.Location = new System.Drawing.Point(0, 0);
	        this.listBox1.MultiColumn = true;
	        this.listBox1.Name = "listBox1";
	        this.listBox1.ScrollAlwaysVisible = true;
	        this.listBox1.Size = new System.Drawing.Size(120, 95);
	        this.listBox1.TabIndex = 0;
	        this.listBox1.ColumnWidth = 85;
	        // 
	        // Form1
	        // 
	        this.ClientSize = new System.Drawing.Size(292, 273);
	        this.Controls.Add(this.listBox1);
	        this.Name = "Form1";
	        this.ResumeLayout(false); 
					
		}
		static void Main (string[] args)
		{
			Application.Run (delegate() { return new MainClass (); });
		}
	}
}

