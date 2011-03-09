using System.Drawing;
using System.Windows.Forms;

public class Form1 : Form
{
    private TabControl tabControl1;
    private TabPage tabPage1;
    private TabPage tabPage2;
	private Label label1;

    public Form1()
    {
        this.tabControl1 = new TabControl();
        this.tabPage1 = new TabPage("Page 1");
        this.tabPage2 = new TabPage("Page 2");
		label1 = new Label();
		tabPage1.Controls.Add(label1);
			
		
		///
		///Label1
		///
		this.label1.Size = new Size(100,25);
		this.label1.Location = new Point(0,0);
		this.label1.Text = "Label!";
			
        // Gets the controls collection for tabControl1.
        // Adds the tabPage1 to this collection.
        this.tabControl1.TabPages.Add(tabPage1);
        this.tabControl1.TabPages.Add(tabPage2);

        this.tabControl1.Location = new Point(25, 25);
        this.tabControl1.Size = new Size(250, 250);

        this.ClientSize = new Size(300, 300);
        this.Controls.Add(tabControl1);
		tabControl1.SelectedIndex = 1;
    }

    static void Main() 
    {
        Application.Run(delegate{return new Form1();});
    }
}
