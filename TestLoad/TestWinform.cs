using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TestLoad;
using MonoMac.AppKit;
class MyForm : Form
{

	private System.Windows.Forms.Button button1;
	private System.Windows.Forms.TextBox textBox1;
	private System.Windows.Forms.TextBox textBox2;
	private System.Windows.Forms.CheckBox checkBox1;
	private System.Windows.Forms.ComboBox comboBox1;
	ListBox lbox;
	private TrackBar trackBar1;
	private RoundButton btn;
	public MyForm ()
	{
		this.button1 = new System.Windows.Forms.Button ();
		this.textBox1 = new System.Windows.Forms.TextBox ();
		this.textBox2 = new System.Windows.Forms.TextBox ();
		this.checkBox1 = new System.Windows.Forms.CheckBox ();
		this.comboBox1 = new System.Windows.Forms.ComboBox ();
		lbox = new ListBox();
		lbox.Size = new SizeF(100,75);
		lbox.Location = new PointF(300,10);		
		lbox.DataSource = new List<string> { "test", "test2", "test3", "test4", "test5", "test6", "test7" };
		lbox.BackColor = Color.Green;
		
		trackBar1 = new TrackBar();
		trackBar1.Location = new PointF(300,90);
		
        trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
		  // The Maximum property sets the value of the track bar when
        // the slider is all the way to the right.
        trackBar1.Maximum = 30;	

        // The TickFrequency property establishes how many positions
        // are between each tick-mark.
        trackBar1.TickFrequency = 5;

        // The LargeChange property sets how many positions to move
        // if the bar is clicked on either side of the slider.
        trackBar1.LargeChange = 3;

        // The SmallChange property sets how many positions to move
        // if the keyboard arrows are used to move the slider.
        trackBar1.SmallChange = 2;
		trackBar1.Value = 20;	
		trackBar1.Minimum = 10;
		
		btn = new RoundButton();		
		btn.Size = new SizeF(100,30);
		btn.Location = new PointF(75,10);
		btn.Text = "test button";
		
		this.SuspendLayout ();
		// 
		// button1
		// 
		this.button1.Location = new System.Drawing.Point (78, 76);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size (75, 25);
		this.button1.TabIndex = 0;
		this.button1.Text = "button1";
		//this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler (this.button1_Click);
		// 
		// textBox1
		// 
		//this.textBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
		this.textBox1.Font = new System.Drawing.Font ("Arial", 7f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		this.textBox1.Location = new System.Drawing.Point (78, 118);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size (100, 26);
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
		//this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point (78, 200);
		//this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size (121, 25);
		//this.comboBox1.TabIndex = 4;
		
		comboBox1.DataSource = new List<ComboBoxItems>{new ComboBoxItems("test1",1),new ComboBoxItems("test2",2)}; //new List<string> { "test", "test2", "test3" };
		comboBox1.DisplayMember = "Display";
		comboBox1.ValueMember = "Value";
		comboBox1.SelectedValueChanged += delegate
		{
			textBox2.Text = comboBox1.SelectedValue.ToString();	
		};
		// 
		// Form1
		// 
		//this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size (500, 400);
		this.Controls.Add(lbox);
		this.Controls.Add (this.comboBox1);
		this.Controls.Add (this.checkBox1);
		this.Controls.Add (this.textBox2);
		this.Controls.Add (this.textBox1);
		this.Controls.Add (this.button1);
		this.Controls.Add (this.btn);
		this.Controls.Add(this.trackBar1);
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
		Application.Run (delegate() { 
			return new MyForm (); });
	}
}
