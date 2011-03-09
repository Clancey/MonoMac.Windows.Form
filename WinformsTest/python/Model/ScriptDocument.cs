using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.Model
{
  /// <summary>
  /// Represents a single script that is loaded in and edited by the script editor.
  /// </summary>
  class ScriptDocument
  {
    //TODO: Need to make this a user defined template -or- embed as a resource
    public static string DefaultPythonTemplateText
    {
      get
      {
        return "# A new blank script";
      }
    }
    public static string TempScriptPath
    {
      get
      {
        string tempPath = System.IO.Path.GetTempPath();
        string path = System.IO.Path.Combine(tempPath, "TempScript.py");
        return path;
      }
    }

    public delegate string GetTextDelegate();
    public delegate void SetTextDelegate(string text);
    public delegate int[] GetBreakPointsDelegate();
    Guid m_id;
    string m_source_code;
    string m_path;
    bool m_modified;

    GetTextDelegate m_GetSourceCode;
    SetTextDelegate m_SetSourceCode;
    GetBreakPointsDelegate m_GetBreakPoints;

    public ScriptDocument(string initialText)
    {
      m_id = Guid.NewGuid();
      m_source_code = initialText;
      m_path = null;
      m_modified = false;
    }

    /// <summary> 
    /// creates a new file which already exists on disk 
    /// </summary> 
    /// <param name="path">Path of file to load</param> 
    public static ScriptDocument Load(string path)
    {
      if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
        return null;

      string text = System.IO.File.ReadAllText(path);

      ScriptDocument doc = new ScriptDocument(text);
      doc.m_path = path;
      return doc;
    }


    public Guid Id
    {
      get { return m_id; }
    }

    public void SetDelegates(GetTextDelegate getFunction, SetTextDelegate setFunction, GetBreakPointsDelegate breakpointFunction)
    {
      m_GetSourceCode = getFunction;
      m_SetSourceCode = setFunction;
      m_GetBreakPoints = breakpointFunction;
    }

    public int[] GetBreakPointLines()
    {
      int[] rc = null;
      if (m_GetBreakPoints != null)
        rc = m_GetBreakPoints();
      return rc;
    }

    public string SourceCode
    {
      get
      {
        if (m_GetSourceCode != null)
          m_source_code = m_GetSourceCode();
        return m_source_code;
      }
      set
      {
        m_source_code = value;
        if (m_SetSourceCode != null)
          m_SetSourceCode(m_source_code);
      }
    }

    public string FullPath
    {
      get { return m_path; }
      set
      {
        m_path = value;
        DocumentEvents.FireDocumentNameChange(m_id);
      }
    }

    public bool Modified
    {
      get { return m_modified; }
      set { m_modified = value; }
    }

    public string DisplayName
    {
      get
      {
        string name = FullPath;
        if (string.IsNullOrEmpty(name))
          name = "{untitled}";
        else
          name = System.IO.Path.GetFileName(name);
        if (Modified)
          name += "*";
        return name;
      }
    }

    public bool IsUntitledAndEmpty
    {
      get
      {
        if (!string.IsNullOrEmpty(FullPath))
          return false;
        // empty includes only having the default template text in the contents
        if (!string.IsNullOrEmpty(SourceCode) && !SourceCode.Equals(DefaultPythonTemplateText, StringComparison.OrdinalIgnoreCase))
          return false;

        return true;
      }
    }

    public bool Save(bool onlyIfModified)
    {
      string script = SourceCode;
      if (string.IsNullOrEmpty(script))
        return false;

      // prompt the user if saving an undefined file
      string path = FullPath;
      if (string.IsNullOrEmpty(path))
        return false;

      string dir = System.IO.Path.GetDirectoryName(path);
      if (!System.IO.Directory.Exists(dir))
        return false;

      if (onlyIfModified && !Modified)
        return false;

      bool rc = false;
      try
      {
        System.IO.File.WriteAllText(path, script);
        rc = true;
      }
      catch (Exception)
      {
      }

      if (rc)
      {
        bool old_modified = Modified;
        if (old_modified == true)
        {
          Modified = false;
          DocumentEvents.FireDocumentModifiedStateChange(Id);
        }
        DocumentEvents.FireSaveDocument(Id);
      }
      return rc;
    }
  }
}
