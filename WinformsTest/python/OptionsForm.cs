using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RhinoDLR_Python
{
  partial class OptionsForm : Form
  {
    static int m_prev_selected_tab = 0;
    public static void ShowOptionsDialog(System.Windows.Forms.IWin32Window owner)
    {
      OptionsForm f = new OptionsForm();
      if (f.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK)
      {
        // handle new paths
        List<string> pathItems = new List<string>();
        for (int i = 0; i < f.m_listPaths.Items.Count; i++)
        {
          pathItems.Add(f.m_listPaths.Items[i].ToString());
        }
        IronPythonPlugIn.thePlugIn.SearchPaths = pathItems.ToArray();

        // handle mru options
        //IronPythonPlugIn.thePlugIn.SetMruOptions((int)f.m_updownMruFilesAtStart.Value, (int)f.m_updownMruFilesInList.Value);

        try
        {
          string fontname = (string)f.m_comboFontNames.SelectedItem;
          int fontsize = (int)f.m_comboFontSizes.SelectedItem;
          Font editor_font = new Font(fontname, (float)fontsize);
          //ScriptForm.EditorFont = editor_font;
          //ScriptForm.ConvertTabsToSpaces = f.m_chkConvertTabsToSpaces.Checked;
        }
        catch(Exception)
        {
        }
      }
      m_prev_selected_tab = f.m_tabOptions.SelectedIndex;
    }

    private OptionsForm()
    {
      InitializeComponent();
      
      string[] paths = IronPythonPlugIn.thePlugIn.SearchPaths;
      if (paths != null && paths.Length > 0)
        m_listPaths.Items.AddRange(paths);

      m_updownMruFilesAtStart.Value = 3;// IronPythonPlugIn.thePlugIn.Settings.GetInteger("MRU.FilesAtStart", 3);
      m_updownMruFilesInList.Value = 10;// IronPythonPlugIn.thePlugIn.Settings.GetInteger("MRU.FilesInList", 10);

       // Gets the list of installed fonts.
      FontFamily[] ff = FontFamily.Families;
      for (int i = 0; i < ff.Length; i++)
      {
        if( ff[i].IsStyleAvailable(FontStyle.Regular))
          m_comboFontNames.Items.Add(ff[i].Name);
      }
      for (int i=7; i<20; i++ )
        m_comboFontSizes.Items.Add(i);

      Font editor_font = this.Font;
      if (editor_font != null)
      {
        m_comboFontNames.SelectedItem = editor_font.Name;;
        m_comboFontSizes.SelectedItem = (int)editor_font.Size;
      }

      m_tabOptions.SelectedIndex = m_prev_selected_tab;
      m_chkConvertTabsToSpaces.Checked = true;// ScriptForm.ConvertTabsToSpaces;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog dlg = new FolderBrowserDialog();
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        string path = dlg.SelectedPath;
        bool inList = false;
        for (int i = 0; i < m_listPaths.Items.Count; i++)
        {
          string existingPath = m_listPaths.Items[i].ToString();
          if (string.Compare(path, existingPath, StringComparison.OrdinalIgnoreCase) == 0)
          {
            inList = true;
            break;
          }
        }
        if (!inList)
          m_listPaths.Items.Add(path);
      }
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
      int index = m_listPaths.SelectedIndex;
      if (index >= 0)
        m_listPaths.Items.RemoveAt(index);
    }

    private void OnSearchPathItemSelect(object sender, EventArgs e)
    {
      int index = m_listPaths.SelectedIndex;
      m_btnRemoveFromSearchPath.Enabled = index >= 0;
      m_btnMoveUpInSearchPath.Enabled = index > 0;
      m_btnMoveDownInSearchPath.Enabled = index < (m_listPaths.Items.Count - 1);
      m_btnOpenPath.Enabled = true;
    }

    private void OnAddToSearchPath(object sender, EventArgs e)
    {
      System.Windows.Forms.FolderBrowserDialog dlg = new FolderBrowserDialog();
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        string path = dlg.SelectedPath;
        m_listPaths.Items.Add(path);
        m_listPaths.SelectedItem = path;
      }
    }

    private void OnRemoveFromSearchPath(object sender, EventArgs e)
    {
      m_listPaths.Items.RemoveAt(m_listPaths.SelectedIndex);
    }

    private void OnMoveUpInSearchPath(object sender, EventArgs e)
    {
      int index = m_listPaths.SelectedIndex;
      object temp = m_listPaths.SelectedItem;
      m_listPaths.Items[index] = m_listPaths.Items[index-1];
      m_listPaths.Items[index - 1] = temp;
      m_listPaths.SelectedIndex = index - 1;
    }

    private void OnMoveDownInSearchPath(object sender, EventArgs e)
    {
      int index = m_listPaths.SelectedIndex;
      object temp = m_listPaths.SelectedItem;
      m_listPaths.Items[index] = m_listPaths.Items[index + 1];
      m_listPaths.Items[index + 1] = temp;
      m_listPaths.SelectedIndex = index + 1;
    }

    private void OnRestoreDefaults(object sender, EventArgs e)
    {
      // this needs to be done on a per-tab page basis

      int selected_tab = m_tabOptions.SelectedIndex;
      if (0 == selected_tab)
      {
        m_listPaths.Items.Clear();
        string[] paths = IronPythonPlugIn.thePlugIn.SearchPaths;//.DefaultSearchPaths;
        if (paths != null && paths.Length > 0)
          m_listPaths.Items.AddRange(paths);

        m_updownMruFilesAtStart.Value = 3;
        m_updownMruFilesInList.Value = 10;
      }
      else if (1 == selected_tab)
      {
        //ICSharpCode.TextEditor.Document.DefaultTextEditorProperties def = new ICSharpCode.TextEditor.Document.DefaultTextEditorProperties();
        Font f = this.Font;
        m_comboFontNames.SelectedItem = f.Name;
        m_comboFontSizes.SelectedItem = (int)f.Size;
        m_chkConvertTabsToSpaces.Checked = true;
      }
    }

    private void ItemDoubleClick(object sender, EventArgs e)
    {
      OnOpenPath(sender, e);
    }

    private void OnOpenPath(object sender, EventArgs e)
    {
      string path = (string)m_listPaths.SelectedItem;

      if (!String.IsNullOrEmpty(path))
        System.Diagnostics.Process.Start("explorer.exe", path);
    }
  }
}