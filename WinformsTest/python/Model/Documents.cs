using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScriptEditor.Model
{
  static class Documents
  {
    private static readonly List<ScriptDocument> m_docs = new List<ScriptDocument>();
    private static Guid m_active_doc_id = Guid.Empty;
    private static string m_save_directory;
    public static void Initialize(string initialSaveDirectory)
    {
      m_save_directory = initialSaveDirectory;
    }


    public static Guid NewDocument(EventHandler<NewScriptUserInterfaceArgs> userInterfaceCallback)
    {
      if ( userInterfaceCallback != null)
      {
        NewScriptUserInterfaceArgs args = new NewScriptUserInterfaceArgs();
        userInterfaceCallback(null, args);
        if (args.CreateScript)
        {
          string filename = args.FileName;
          string script = args.Script;
          Guid rc = Guid.Empty;
          if (string.IsNullOrEmpty(filename))
          {
            rc = NewDocument(script, true);
          }
          else
          {
            // create the file and perform an Open
            string directory = System.IO.Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory))
            {
              if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
              System.IO.File.WriteAllText(filename, script);
              rc = OpenDocument(filename, true);
            }
          }
          return rc;
        }
      }
      return Guid.Empty;
    }

    /// <summary>Create a new script file </summary>
    /// <param name="initialText"></param>
    /// <param name="setAsActive">make the new script the "active" document</param>
    /// <returns>id of the new script on success</returns>
    public static Guid NewDocument(string initialText, bool setAsActive)
    {
      Guid doc_to_close = Guid.Empty;
      // Look at the first file to see if it is a completely empty "untitled" script.
      // If so, close this file
      if (m_docs.Count == 1)
      {
        ScriptDocument old_doc = m_docs[0];
        if (old_doc.IsUntitledAndEmpty)
          doc_to_close = old_doc.Id;
      }


      ScriptDocument doc = new ScriptDocument(initialText);
      // Insert the document at the beginning of the list instead of
      // appending it to the end. This will cause the file to "look" like 
      // the topmost document in the script editor
      m_docs.Insert(0, doc);

      DocumentEvents.FireNewDocument(doc.Id);
      if (setAsActive)
        SetActiveDoc(doc.Id);

      if( doc_to_close != Guid.Empty )
        CloseDocument(doc_to_close);

      return doc.Id;
    }

    public static Guid OpenDocument(System.Windows.Forms.Form parentForm)
    {
      Guid rc = Guid.Empty;
      System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
      fd.Title = "Open an existing script file";
      fd.Filter = "python scripts (*.py)|*.py|All files (*.*)|*.*";

      if (fd.ShowDialog(parentForm) == System.Windows.Forms.DialogResult.OK)
      {
        rc = OpenDocument(fd.FileName, true);
      }
      return rc;
    }

    public static Guid OpenDocument(string path, bool setActive)
    {
      if (string.IsNullOrEmpty(path))
        return Guid.Empty;

      // See if the file is already opened. If it is then just make is active
      for (int i = 0; i < m_docs.Count; i++)
      {
        if (string.Compare(m_docs[i].FullPath, path, StringComparison.InvariantCultureIgnoreCase) == 0)
        {
          Guid rc = m_docs[i].Id;
          SetActiveDoc(rc);
          return rc;
        }
      }

      ScriptDocument doc = ScriptDocument.Load(path);
      if( null==doc )
        return Guid.Empty;

      // If the active file is an "unnamed" document that has not been modified,
      // close this file
      Guid scriptToClose = Guid.Empty;
      if( m_docs.Count == 1 && m_docs[0].IsUntitledAndEmpty )
      {
        scriptToClose = m_docs[0].Id;
      }

      // Insert the document at the beginning of the list instead of
      // appending it to the end. This will cause the file to "look" like 
      // the topmost document in the script editor
      m_docs.Insert(0, doc);
      DocumentEvents.FireOpenDocument(doc.Id);

      if (scriptToClose != Guid.Empty)
        CloseDocument(scriptToClose);

      if (setActive)
        SetActiveDoc(doc.Id);

      return doc.Id;
    }

    /// <summary>
    /// This function does NOT save the script if it has been modified.
    /// </summary>
    /// <param name="scriptId"></param>
    public static void CloseDocument(Guid scriptId)
    {
      int index = IndexFromId(scriptId);
      if (index < 0)
        return;
      ScriptDocument doc = m_docs[index];
      if (null == doc)
        return;

      m_docs.RemoveAt(index);
      DocumentEvents.FireCloseDocument(scriptId);
      
      if (m_active_doc_id == doc.Id)
      {
        m_active_doc_id = Guid.Empty;
        if (m_docs.Count == 0)
        {
          NewDocument("", false);
        }
        if (m_docs.Count > 0)
          SetActiveDoc( m_docs[0].Id );
      }
    }


    public static void ScriptCloseEventHelper(Guid scriptId)
    {
      ScriptDocument doc = DocumentFromId(scriptId);
      if( null==doc )
        return;

      if (doc.Modified)
      {
        string question = "Save changes to " + doc.DisplayName + "?";
        DialogResult rc = MessageBox.Show( question, "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (rc == DialogResult.Cancel)
          return;
        if (rc == DialogResult.Yes)
          SaveDocument(scriptId, false);
      }
      CloseDocument(scriptId);
    }

    public static void SaveDocument(Guid scriptId, bool showSaveDialog)
    {
      ScriptDocument doc = DocumentFromId(scriptId);
      if (null==doc)
        return;

      string path = doc.FullPath;
      if (string.IsNullOrEmpty(path))
        showSaveDialog = true;

      if (showSaveDialog)
      {
        System.Windows.Forms.SaveFileDialog fd = new System.Windows.Forms.SaveFileDialog();
        fd.Title = "Save Script";
        fd.Filter = "python scripts (*.py)|*.py|" +
                    "All files (*.*)|*.*";

        if (!string.IsNullOrEmpty(m_save_directory))
          fd.InitialDirectory = m_save_directory;

        // 01 June 2010 S. Baer - RR64956
        // Use the path to the existing script if it already exists
        if (!string.IsNullOrEmpty(path))
        {
          string dir = System.IO.Path.GetDirectoryName(path);
          if (System.IO.Directory.Exists(dir))
            fd.InitialDirectory = dir;
          string file = System.IO.Path.GetFileName(path);
          if (!string.IsNullOrEmpty(file))
            fd.FileName = file;
        }

        if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          return;
        path = fd.FileName;
      }

      if (string.IsNullOrEmpty(path))
        return;

      doc.FullPath = path;
      doc.Save(false);

      // 01 June 2010 S. Baer RR67329
      // Update the save directory so the SaveAs dialog will start in the last saved location
      string saveAsDir = System.IO.Path.GetDirectoryName(path);
      if (!string.IsNullOrEmpty(saveAsDir) && System.IO.Directory.Exists(saveAsDir))
        m_save_directory = saveAsDir;

    }

    public static void SetDocumentModified(Guid document_id, bool modified)
    {
      for (int i = 0; i < m_docs.Count; i++)
      {
        ScriptDocument doc = m_docs[i];
        if (doc.Id == document_id)
        {
          if (doc.Modified != modified)
          {
            doc.Modified = modified;
            DocumentEvents.FireDocumentModifiedStateChange(document_id);
          }
          break;
        }
      }
    }

    public static Guid FindScript(string fullpath)
    {
      for( int i=0; i<m_docs.Count; i++ )
      {
        string path = m_docs[i].FullPath;
        if (string.IsNullOrEmpty(path))
          continue;

        if (string.Compare(path, fullpath, StringComparison.InvariantCultureIgnoreCase) == 0)
          return m_docs[i].Id;
      }

      return Guid.Empty;
    }

    public static ScriptDocument DocumentFromId( Guid id )
    {
      int index = IndexFromId(id);
      if( index<0 )
        return null;
      return m_docs[index];
    }

    private static int IndexFromId(Guid id)
    {
      for (int i = 0; i < m_docs.Count; i++)
      {
        if (m_docs[i].Id == id)
          return i;
      }
      return -1;
    }

    public static int DocumentCount { get { return m_docs.Count; } }
    public static List<ScriptDocument> OpenDocuments { get { return m_docs; } }

    public static ScriptDocument ActiveDocument
    {
      get { return DocumentFromId(m_active_doc_id); }
    }
    public static Guid ActiveDocumentId
    {
      get { return m_active_doc_id; }
    }

    public static void SetActiveDoc(Guid id)
    {
      Guid old_active_id = m_active_doc_id;
      // Guid.Empty sets active file to nothing
      ScriptDocument oldActive = DocumentFromId(old_active_id);
      ScriptDocument newActive = null;
      for (int i = 0; i < m_docs.Count; i++)
      {
        if (m_docs[i].Id == id)
        {
          newActive = m_docs[i];
          break;
        }
      }
      if( null==newActive )
        return;
      m_active_doc_id = id;
      if (old_active_id != m_active_doc_id)
      {
        DocumentEvents.FireSetActiveDocument(m_active_doc_id);
      }
    }

    public static void SetActiveFileToNext()
    {
      ScriptDocument doc = ActiveDocument;
      if (null == doc)
      {
        if (m_docs.Count > 0)
          SetActiveDoc(m_docs[0].Id);
        return;
      }

      for (int i = 0; i < m_docs.Count; i++)
      {
        if (m_docs[i].Id == m_active_doc_id)
        {
          Guid id = Guid.Empty;
          if( i < (m_docs.Count-1))
            id = m_docs[i+1].Id;
          else
            id = m_docs[0].Id;
          if( id != Guid.Empty )
            SetActiveDoc(id);
          break;
        }
      }
    }

    public static void ClearDocumentList()
    {
      m_docs.Clear();
      m_active_doc_id = Guid.Empty;
    }

    public static int SaveAll(bool onlyIfModified)
    {
      int rc=0;
      for (int i = 0; i < m_docs.Count; i++)
      {
        if (m_docs[i].Save(onlyIfModified))
          rc++;
      }
      return rc;
    }


  }

  static class DocumentEvents
  {
    public delegate void DocumentEventHandler(Guid script_id);

    public static event DocumentEventHandler OnSetActiveDocument;
    public static event DocumentEventHandler OnDocumentModifiedStateChange;
    public static event DocumentEventHandler OnCloseDocument;
    public static event DocumentEventHandler OnOpenDocument;
    public static event DocumentEventHandler OnSaveDocument;
    public static event DocumentEventHandler OnNewDocument;
    public static event DocumentEventHandler OnDocumentNameChange;

    public static void FireSetActiveDocument(Guid scriptId)
    {
      if (OnSetActiveDocument != null)
        OnSetActiveDocument(scriptId);
    }
    public static void FireDocumentModifiedStateChange(Guid scriptId)
    {
      if (OnDocumentModifiedStateChange != null)
        OnDocumentModifiedStateChange(scriptId);
    }
    public static void FireCloseDocument(Guid scriptId)
    {
      if (OnCloseDocument != null)
        OnCloseDocument(scriptId);
    }
    public static void FireOpenDocument(Guid scriptId)
    {
      if (OnOpenDocument != null)
        OnOpenDocument(scriptId);
    }
    public static void FireSaveDocument(Guid scriptId)
    {
      if (OnOpenDocument != null)
        OnSaveDocument(scriptId);
    }
    public static void FireNewDocument(Guid scriptId)
    {
      if (OnNewDocument != null)
        OnNewDocument(scriptId);
    }
    public static void FireDocumentNameChange(Guid scriptId)
    {
      if (OnDocumentNameChange != null)
        OnDocumentNameChange(scriptId);
    }
  }

  public class NewScriptUserInterfaceArgs : EventArgs
  {
    bool m_create_script;
    string m_filename;
    string m_script;

    public bool CreateScript
    {
      get { return m_create_script; }
      set { m_create_script = value; }
    }

    public string FileName
    {
      get
      {
        return m_filename;
      }
      set
      {
        m_filename = value;
      }
    }

    public string Script
    {
      get
      {
        return m_script;
      }
      set
      {
        m_script = value;
      }
    }
  }


}
