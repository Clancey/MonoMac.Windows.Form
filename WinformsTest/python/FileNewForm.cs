using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RhinoDLR_Python
{
  partial class FileNewForm : Form
  {
    public static void ShowFileNewDialog( object sender, ScriptEditor.Model.NewScriptUserInterfaceArgs args )
    {
      // provide for new command scripts or standard python scripts
      FileNewForm frm = new FileNewForm();
      if ( frm.ShowDialog() == DialogResult.OK )
      {
        args.CreateScript = true;
        args.FileName = frm.GetFilePath();
        args.Script = frm.GetScriptString();
      }
    }

    enum ScriptType : int
    {
      CommandScript = 0,
      EmptyScript = 1
    }

    class PlugInPath
    {
      public PlugInPath(string path)
      {
        m_directory_path = path;
      }
      string m_directory_path;
      public override string ToString()
      {
        return ShortName;
      }
      public string ShortName
      {
        get
        {
          int index = m_directory_path.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
          string pluginname = m_directory_path.Substring(index);
          return pluginname;
        }
      }
      public string FullPath
      {
        get { return m_directory_path; }
      }
    }

    FileNewForm()
    {
      InitializeComponent();
      m_listNewType.SelectedIndex = 0;

      // Build list of plug-ins that are installed on this computer (and are dev plug-ins)
      string plugin_dir = IronPythonPlugIn.thePlugIn.PlugInScriptsDirectory();
      if (System.IO.Directory.Exists(plugin_dir))
      {
        string[] plug_ins = System.IO.Directory.GetDirectories(plugin_dir);
        if (null != plug_ins)
        {
          for (int i = 0; i < plug_ins.Length; i++)
          {
            string dev = System.IO.Path.Combine(plug_ins[i], "dev");
            if (System.IO.Directory.Exists(dev))
            {
              m_combo_plugin.Items.Add(new PlugInPath(plug_ins[i]));
            }
          }

          if (m_combo_plugin.Items.Count > 0)
          {
            // look in the MRU list to see which plug-in should be default
            List<System.IO.FileInfo> mrulist = Utility.MRUFileServer.GetMRURecords();
            int index = -1;
            if (null != mrulist)
            {
              string baseplug_in_dir = IronPythonPlugIn.thePlugIn.PlugInScriptsDirectory();
              for (int i = 0; i < mrulist.Count; i++)
              {
                string path = mrulist[i].DirectoryName;
                if (!path.StartsWith(baseplug_in_dir, StringComparison.OrdinalIgnoreCase))
                  continue;
                path = path.Substring(baseplug_in_dir.Length + 1);
                path = path.Substring(0, path.IndexOf(System.IO.Path.DirectorySeparatorChar));

                for (int j = 0; j < m_combo_plugin.Items.Count; j++)
                {
                  string item = m_combo_plugin.Items[j].ToString();
                  if (item.Equals(path, StringComparison.OrdinalIgnoreCase))
                  {
                    index = j;
                    break;
                  }
                }
                if (index != -1)
                  break;
              }
            }
            if (index == -1)
              index = 0;

            m_combo_plugin.SelectedIndex = index;
          }
        }
      }
    }

    ScriptType SelectedScriptType
    {
      get
      {
        int index = m_listNewType.SelectedIndex;
        return (ScriptType)index;
      }
    }

    private void OnNewTypeSelectionChanged(object sender, EventArgs e)
    {
      bool visible = (SelectedScriptType == ScriptType.CommandScript);
      m_label_cmdname.Visible = visible;
      m_txtCommandName.Visible = visible;
      m_label_plugin.Visible = visible;
      m_combo_plugin.Visible = visible;

      if (m_listNewType.SelectedIndex == (int)ScriptType.CommandScript)
      {
        m_groupDescription.Text = "Rhino Command";
        m_lblDescription.Text = "Command scripts act as standard Rhino commands and are bundled in script Plug-Ins.";
        m_txtCommandName.Focus();
      }
      else if (m_listNewType.SelectedIndex == (int)ScriptType.EmptyScript)
      {
        m_groupDescription.Text = "Empty Script";
        m_lblDescription.Text = "An empty file for writing scripts that can be run from the Python dialog.";
      }
    }

    public string GetFilePath()
    {
      int index = m_listNewType.SelectedIndex;
      if ( ScriptType.CommandScript == SelectedScriptType )
      {
        string filename = m_txtCommandName.Text + "_cmd.py";
        PlugInPath p = m_combo_plugin.SelectedItem as PlugInPath;
        if (null == p)
        {
          // This is a "new" IronPython plug-in. Use the Text in the combo box
          // to define a new plug-in
          Guid plugin_id = Guid.NewGuid();
          string shortname = m_combo_plugin.Text + " {" + plugin_id.ToString() + "}";
          string fullpath = IronPythonPlugIn.thePlugIn.PlugInScriptsDirectory();
          fullpath = System.IO.Path.Combine(fullpath, shortname);
          p = new PlugInPath(fullpath);
        }
        string path = p.FullPath;
        path = System.IO.Path.Combine(path, "dev");
        path = System.IO.Path.Combine(path, filename);
        return path;
      }
      return null;
    }

    public string GetScriptString()
    {
      int index = m_listNewType.SelectedIndex;

      if ((int)ScriptType.CommandScript == index)
      {
        System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
        string[] names = a.GetManifestResourceNames();
        if (null == names)
          return "";
        string name = null;
        for (int i = 0; i < names.Length; i++)
        {
          if (names[i].EndsWith("rhcommand.template", StringComparison.OrdinalIgnoreCase))
          {
            name = names[i];
            break;
          }
        }
        string template = "";
        if (!String.IsNullOrEmpty(name))
        {
          System.IO.Stream resourceStream = a.GetManifestResourceStream(name);
          System.IO.StreamReader stream = new System.IO.StreamReader(resourceStream);
          template = stream.ReadToEnd();
          stream.Close();
        }
        if (!String.IsNullOrEmpty(template))
        {
          template = template.Replace("[COMMANDNAME]", m_txtCommandName.Text);
        }
        return template;
      }
      return "";
    }

    private void OnNewButtonClickEvent(object sender, EventArgs e)
    {
      // Command scripts must have a valid
      // Command Name and Plug-In Name
      if (SelectedScriptType == ScriptType.CommandScript)
      {
        string commandName = m_txtCommandName.Text;
        string error_message=null;
        if( string.IsNullOrEmpty(commandName) )
        {
          error_message = "Command scripts must have an associated command name";
        }
        else if (!Rhino.Commands.Command.IsValidCommandName(commandName))
        {
          error_message = "Invalid command name";
        }
        else if (Rhino.Commands.Command.IsCommand(commandName))
        {
          error_message = "Command name already exists";
        }

        if (!string.IsNullOrEmpty(error_message))
        {
          Rhino.UI.Dialogs.ShowMessageBox(error_message, "Command Name Error");
          m_txtCommandName.Focus();
          return;
        }

        // We have a valid command name. Make sure the plug-in name is also ok
        string plugin_name = m_combo_plugin.Text.Trim();
        if (string.IsNullOrEmpty(plugin_name))
        {
          error_message = "Command scripts must be associated with a plug-in";
        }
        else
        {
          char[] bad_chars = System.IO.Path.GetInvalidPathChars();
          if (plugin_name.IndexOfAny(bad_chars) >= 0)
            error_message = "Invalid plug-in name";
        }

        if (!string.IsNullOrEmpty(error_message))
        {
          Rhino.UI.Dialogs.ShowMessageBox(error_message, "Plug-In Name Error");
          m_combo_plugin.Focus();
          return;
        }

        // make sure this command file doesn't already exist
        string filename = GetFilePath();
        if (System.IO.File.Exists(filename))
        {
          error_message = string.Format("'{0}.{1}' file already exists", plugin_name, commandName);
          Rhino.UI.Dialogs.ShowMessageBox(error_message, "File Exists");
          m_txtCommandName.Focus();
          return;
        }
      }
      this.DialogResult = DialogResult.OK;
      Close();
    }

    private void OnFormShownEvent(object sender, EventArgs e)
    {
      m_txtCommandName.Focus();
    }
  }
}

// used to keep things working outside of Rhino
class IronPythonPlugIn
{
	static IronPythonPlugIn m_pi = new IronPythonPlugIn();
	public static IronPythonPlugIn thePlugIn	{ get { return m_pi; } }

	System.Reflection.Assembly Assembly
	{
		get{return System.Reflection.Assembly.GetExecutingAssembly();}
	}

	public string PlugInScriptsDirectory()
	{
		string path = System.IO.Path.GetDirectoryName(this.Assembly.Location); //SettingsDirectory;
		System.IO.DirectoryInfo ironpy_dir = System.IO.Directory.GetParent(path);
		System.IO.DirectoryInfo plugins_dir = System.IO.Directory.GetParent(ironpy_dir.FullName);
		return System.IO.Path.Combine(plugins_dir.FullName, "PythonPlugins");
	}

  private List<string> m_paths = new List<string>();
  public string[] SearchPaths
  {
		get
		{
      if (m_paths.Count == 0)
      {
        m_paths.Add(PlugInScriptsDirectory());
        m_paths.Add("some other path");
      }

			return m_paths.ToArray();
		}
    set
		{
			m_paths = new List<string>();
			m_paths.AddRange(value);
		}
	}
}

namespace Utility
{
	static class MRUFileServer
	{
		public static List<System.IO.FileInfo> GetMRURecords() { return null; }
	}
}

namespace Rhino
{
	namespace Commands
	{
		static class Command
		{
			public static bool IsValidCommandName(string commandName){ return !string.IsNullOrEmpty(commandName); }
			public static bool IsCommand(string commandName){ return false; }
		}
	}
	namespace UI
	{
		static class Dialogs
		{
			public static void ShowMessageBox(string text, string caption){	System.Windows.Forms.MessageBox.Show(text, caption); }
		}
	}
}