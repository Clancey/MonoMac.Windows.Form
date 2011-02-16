using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TestLoad;
using System.Reflection;
class MyForm : Form
{

	private System.Windows.Forms.Button button1;
	private System.Windows.Forms.Button button2;
	private System.Windows.Forms.TextBox textBox1;
	private System.Windows.Forms.TextBox textBox2;
	private System.Windows.Forms.CheckBox checkBox1;
	private System.Windows.Forms.ComboBox comboBox1;
	private System.Windows.Forms.ListBox listbox1;
	private System.Windows.Forms.TrackBar trackBar1;
	private System.Windows.Forms.Label label1;
	public MyForm ()
	{
		this.listbox1 = new System.Windows.Forms.ListBox();
		this.button1 = new System.Windows.Forms.Button ();
		this.button2 = new System.Windows.Forms.Button ();
		this.textBox1 = new System.Windows.Forms.TextBox ();
		this.textBox2 = new System.Windows.Forms.TextBox ();
		this.checkBox1 = new System.Windows.Forms.CheckBox ();
		this.comboBox1 = new System.Windows.Forms.ComboBox ();
		this.trackBar1 = new TrackBar();
		this.label1 = new Label();
		
		
		this.SuspendLayout ();
		// 
		// listbox1
		// 
		listbox1.Size = new Size(100,75);
		listbox1.Location = new Point(300,10);		
		listbox1.DataSource = new List<string> { "test", "test2", "test3", "test4", "test5", "test6", "test7" };
		listbox1.BackColor = Color.Green;
		// 
		// trackBar1
		// 
		trackBar1.Location = new Point(300,90);		
        trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
        trackBar1.Maximum = 30;	
        trackBar1.TickFrequency = 5;
        trackBar1.LargeChange = 3;
        trackBar1.SmallChange = 2;
		trackBar1.Value = 20;	
		trackBar1.Minimum = 10;
		// 
		// button1
		// 
		this.button1.Location = new System.Drawing.Point (78, 76);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size (75, 25);
		this.button1.TabIndex = 0;
		this.button1.Text = "Show Modal";
		//this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler (this.button1_Click);
		// 
		// button2
		// 
		this.button2.Location = new System.Drawing.Point (150, 76);
		this.button2.Name = "button1";
		this.button2.Size = new System.Drawing.Size (150, 25);
		this.button2.TabIndex = 0;
		this.button2.Text = "Show Message box";
		//this.button1.UseVisualStyleBackColor = true;
		this.button2.Click += delegate {
			MessageBox.Show("Text","Caption");
		};
		// 
		// textBox1
		// 
		//this.textBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
		this.textBox1.Font = new System.Drawing.Font ("Arial", 7f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		this.textBox1.Location = new System.Drawing.Point (78, 118);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size (100, 25);
		this.textBox1.TabIndex = 1;
		//this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
		// 
		// textBox2
		// 
		this.textBox2.Location = new System.Drawing.Point (78, 156);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size (100, 20);
		this.textBox2.TabIndex = 2;
		// 
		// checkBox1
		// 
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point (78, 41);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size (80, 17);
		this.checkBox1.TabIndex = 3;
		this.checkBox1.Text = "checkBox1";
		this.checkBox1.BackColor = Color.Aqua;
		//this.checkBox1.UseVisualStyleBackColor = true;
		// 
		// comboBox1
		// 
		this.comboBox1.Location = new System.Drawing.Point (78, 200);
		this.comboBox1.Size = new System.Drawing.Size (121, 23);
		this.comboBox1.DataSource = new List<ComboBoxItems>{new ComboBoxItems("test1",1),new ComboBoxItems("test2",2)}; //new List<string> { "test", "test2", "test3" };
		this.comboBox1.DisplayMember = "Display";
		this.comboBox1.ValueMember = "Value";
		this.comboBox1.SelectedValueChanged += delegate
		{
			textBox2.Text = comboBox1.SelectedValue.ToString();	
		};
		///
		///Label1
		///
		this.label1.Size = new Size(100,25);
		this.label1.Location = new Point(78,250);
		this.label1.Text = "Label!";
		
		// 
		// Form1
		// 
		//this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size (500, 400);
		this.Controls.Add(listbox1);
		this.Controls.Add (this.comboBox1);
		this.Controls.Add (this.checkBox1);
		this.Controls.Add (this.textBox2);
		this.Controls.Add (this.textBox1);
		this.Controls.Add (this.button1);
		this.Controls.Add (this.button2);
		this.Controls.Add(this.trackBar1);
		this.Controls.Add(this.label1);
		this.Name = "Form1";
		this.Text = "Form1";
		this.ResumeLayout (false);
		this.PerformLayout ();		
	}

	private void textbox1_KeyDown (object sender, KeyEventArgs e)
	{
		textBox2.Text = e.KeyValue.ToString ();
		//textBox3.Text = ((Keys)e.KeyValue).ToString()+ (e.Shift ? " + Shift" : "") + (e.Alt ? " + Alt" : "");
	}

	private void button1_Click (object sender, EventArgs e)
	{
		Form testForm = new Form ();
		var btn = new Button();
		btn.Click += delegate{
			testForm.Close();	
		};
		testForm.Controls.Add(btn);
		testForm.ShowDialog ();
		//MessageBox.Show ("I was clicked");
		
	}
	
	private void trackBar1_Scroll(object sender, System.EventArgs e)
    {
        // Display the trackbar value in the text box.
        textBox1.Text = "" + trackBar1.Value;
    }
	public static void Main (string[] args)
	{
		Application.Run (delegate() { return new MyForm (); });
	}
}
