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
        TabControl tab = new TabControl ();
		tab.Alignment = TabAlignment.Top;
		tab.Dock = DockStyle.Fill;
		// tab.Appearance = TabAppearance.FlatButtons;
		// tab.Multiline = true;
		
		tab.Controls.Add (CreateTabPage ("Red", Color.FromArgb (255, 255, 0, 0)));
		tab.Controls.Add (CreateTabPage ("Orange", Color.FromArgb (255, 255, 153, 0)));
		tab.Controls.Add (CreateTabPage ("Yellow", Color.FromArgb (255, 255, 255, 0)));
		tab.Controls.Add (CreateTabPage ("Green", Color.FromArgb (255, 0, 153, 0)));
		tab.Controls.Add (CreateTabPage ("Blue", Color.FromArgb (255, 0, 0, 255)));
		tab.Controls.Add (CreateTabPage ("Purple", Color.FromArgb (255, 197, 0, 148)));
		
		tab.Height = 500;
		tab.Width = 500;
		tab.SelectedIndex = 3;
		Controls.Add (tab);
		
		this.ClientSize = new Size(500,500);
	}
	
	private TabPage CreateTabPage (string label, Color c)
	{
		TabPage res = new TabPage (label);
		res.BackColor = c;
		return res;
	}

    static void Main() 
    {
        Application.Run(delegate{return new Form1();});
    }
}
