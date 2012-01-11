using System;
using System.Collections.Generic;
using System.Text;
using ScriptEditor.Model;
using ICSharpCode.TextEditor.Gui.CompletionWindow;


namespace Ripe.SharpDev
{
  class EditorTabPage : ICSharpCode.TextEditor.TextEditorControl
  {
    // Each tab refers to a single document in the "Documents" list
    // The the document Id to track this document
    Guid m_document_id = Guid.Empty;
    System.Windows.Forms.Timer m_timer;
    System.Windows.Forms.Form m_mainform;
    ICSharpCode.TextEditor.Gui.CompletionWindow.CodeCompletionWindow m_code_complete;
    Action<string> m_help_callback;

    public EditorTabPage(ScriptDocument doc, Action<string> help_cb)
    {
      InitializeComponent();
      m_help_callback = help_cb;
      m_document_id = doc.Id;

      //Set editor defaults
      this.ShowSpaces = true;
      this.TabIndent = 4;
      this.ShowTabs = true;
      this.ConvertTabsToSpaces = true;
      this.SetHighlighting("Python");
      this.Dock = System.Windows.Forms.DockStyle.Fill;
      this.IsIconBarVisible = true;

      if (!string.IsNullOrEmpty(doc.FullPath))
        LoadFile(doc.FullPath, false, true);
      else
        this.Text = doc.SourceCode;

      doc.SetDelegates(this.GetEditorText, this.SetEditorText, this.GetBreakPointLines);

      // test for custom colors
      ICSharpCode.TextEditor.Document.DefaultHighlightingStrategy strat = this.Document.HighlightingStrategy as ICSharpCode.TextEditor.Document.DefaultHighlightingStrategy;
      if (strat != null)
        strat.SetColorFor("SpaceMarkers", new ICSharpCode.TextEditor.Document.HighlightColor(System.Drawing.Color.BurlyWood, true, false)); 
      this.Document.FoldingManager.FoldingStrategy = new PythonFoldingStrategy();
      this.Document.FormattingStrategy = new PythonFormattingStrategy();
      this.Document.FoldingManager.UpdateFoldings(null, null);

      // Wire up events for this TextEditor
      this.Document.LineCountChanged += new EventHandler<ICSharpCode.TextEditor.Document.LineCountChangeEventArgs>(Document_LineCountChanged);
      this.Document.DocumentChanged += new ICSharpCode.TextEditor.Document.DocumentEventHandler(OnDocumentChanged);
      this.ActiveTextAreaControl.TextArea.IconBarMargin.MouseDown += new ICSharpCode.TextEditor.MarginMouseEventHandler(IconBarMargin_MouseDown);

      //this.ActiveTextAreaControl.TextArea.DoProcessDialogKey += new ICSharpCode.TextEditor.DialogKeyProcessor(TextArea_DoProcessDialogKey);

      this.ActiveTextAreaControl.TextArea.KeyEventHandler += TextAreaKeyEventHandler;
      this.Disposed += CloseCodeCompletionWindow;
      this.ActiveTextAreaControl.TextArea.IconBarMargin.Painted += new ICSharpCode.TextEditor.MarginPaintEventHandler(IconBarMargin_Painted);
    }

    protected override void OnParentChanged(EventArgs e)
    {
      if (m_mainform != ParentForm)
      {
        m_mainform = ParentForm;
        if (m_mainform != null)
          m_mainform.FormClosing += CloseCodeCompletionWindow;
      }
      base.OnParentChanged(e);
    }

    #region properties
    public Guid DocumentId
    {
      get { return m_document_id; }
    }
    #endregion

    #region intelliclunk
    RhinoDLR_Python.Intellisense m_intellisense;
    public void SetIntellisense(RhinoDLR_Python.Intellisense isense)
    {
      m_intellisense = isense;
    }
    ModuleNameCompletionProvider m_modulename_provider;
    public ICompletionDataProvider ModuleNameProvider
    {
      get { return m_modulename_provider ?? (m_modulename_provider = new ModuleNameCompletionProvider(m_intellisense)); }
    }

    ModuleItemCompletionProvider m_moduleitem_provider;
    public ICompletionDataProvider ModuleItemProvider
    {
      get { return m_moduleitem_provider ?? (m_moduleitem_provider = new ModuleItemCompletionProvider(m_intellisense)); }
    }

    DotCompletionProvider m_dot_completion;
    public ICompletionDataProvider DotProvider
    {
      get { return m_dot_completion ?? (m_dot_completion = new DotCompletionProvider(m_intellisense)); }
    }

    #endregion

    // Hack used to deal with the issue that DialogKeys don't appear to get processed
    // correctly while running as a Rhino plug-in
    public void ProcessKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
      if (ActiveTextAreaControl.TextArea.Focused)
      {
        ActiveTextAreaControl.TextArea.ExecuteDialogKey(e.KeyData);
      }
    }

    // Use the LineCountChanged event to trigger recomputing of folding information
    void Document_LineCountChanged(object sender, ICSharpCode.TextEditor.Document.LineCountChangeEventArgs e)
    {
      if (null == m_timer)
      {
        m_timer = new System.Windows.Forms.Timer();
        // wait 3 seconds to update all of the folding information
        m_timer.Interval = 3000;
        m_timer.Tick += new EventHandler(OnTimerTick);
        m_timer.Start();
      }
      m_timer.Enabled = true;
    }

    void IconBarMargin_MouseDown(ICSharpCode.TextEditor.AbstractMargin sender, System.Drawing.Point mousepos, System.Windows.Forms.MouseButtons mouseButtons)
    {
      // Determine if bookmarks or breakpoints need to be added or removed
      ICSharpCode.TextEditor.IconBarMargin margin = sender as ICSharpCode.TextEditor.IconBarMargin;
      if (null == margin || System.Windows.Forms.MouseButtons.Left != mouseButtons)
        return;

      System.Drawing.Rectangle viewRect = margin.TextArea.TextView.DrawingPosition;
      int line = margin.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top).Line;
      ToggleBreakPoint(line);
    }

    private void RepaintBookmarks()
    {
      ICSharpCode.TextEditor.TextArea textArea = ActiveTextAreaControl.TextArea;
      if (null != textArea)
      {
        ICSharpCode.TextEditor.IconBarMargin margin = textArea.IconBarMargin;
        if (null != margin)
        {
          margin.Paint(textArea.CreateGraphics(), textArea.DisplayRectangle);
        }
      }
    }

    public void ClearAllBreakPoints()
    {
      this.Document.BookmarkManager.Clear();
      RepaintBookmarks();
    }

    public void ToggleBreakPoint()
    {
      int line = ActiveTextAreaControl.Caret.Line;
      ToggleBreakPoint(line);
    }

    public void ToggleBreakPoint(int line)
    {
      if (line >= 0 && line < this.Document.TotalNumberOfLines)
      {
        ICSharpCode.TextEditor.Document.LineSegment lineseg = this.Document.GetLineSegment(line);
        // walk through the line and make sure the first word is not a comment
        bool okToAdd = false;
        for (int i = 0; i < lineseg.Words.Count; i++)
        {
          if (lineseg.Words[i].Type == ICSharpCode.TextEditor.Document.TextWordType.Word)
          {
            if (lineseg.Words[i].Color != System.Drawing.Color.Green)
              okToAdd = true;
            break;
          }
        }
        if (!okToAdd)
          return;

        ICSharpCode.TextEditor.Document.Bookmark existing_mark = null;
        foreach (ICSharpCode.TextEditor.Document.Bookmark mark in this.Document.BookmarkManager.Marks)
        {
          if (mark.LineNumber == line)
          {
            existing_mark = mark;
            break;
          }
        }

        if (existing_mark != null)
        {
          this.Document.BookmarkManager.RemoveMark(existing_mark);
        }
        else
        {
          ICSharpCode.TextEditor.Document.Bookmark breakpoint = new BreakPoint(this.Document, new ICSharpCode.TextEditor.TextLocation(0, line));
          this.Document.BookmarkManager.AddMark(breakpoint);
        }

        RepaintBookmarks();
      }
    }

    class BreakPoint : ICSharpCode.TextEditor.Document.Bookmark
    {
      public BreakPoint( ICSharpCode.TextEditor.Document.IDocument document,
        ICSharpCode.TextEditor.TextLocation location)
        : base(document, location)
      {
      }

      public override void Draw(ICSharpCode.TextEditor.IconBarMargin margin, System.Drawing.Graphics g, System.Drawing.Point p)
      {
        margin.DrawBreakpoint(g, p.Y, true, true);
      }
    }


    void OnTimerTick(object sender, EventArgs e)
    {
      m_timer.Stop();
      this.Document.FoldingManager.UpdateFoldings(null, null);
    }

    // source for the ScriptDocument text
    private string GetEditorText() { return this.Text; }
    private void SetEditorText(string text) { this.Text = text; }

    private int[] GetBreakPointLines()
    {
      List<int> lines = new List<int>();
      foreach (ICSharpCode.TextEditor.Document.Bookmark mark in this.Document.BookmarkManager.Marks)
      {
        if (mark is BreakPoint)
          lines.Add(mark.LineNumber + 1);
      }
      if (lines.Count == 0)
        return null;
      return lines.ToArray();
    }

    public void SetActiveBreakPoint(int line)
    {
      m_active_breakline = line;
      RepaintBookmarks();
    }

    int m_active_breakline = -1;
		static bool IsLineInsideRegion(int top, int bottom, int regionTop, int regionBottom)
		{
			if (top >= regionTop && top <= regionBottom) {
				// Region overlaps the line's top edge.
				return true;
			} else if(regionTop > top && regionTop < bottom) {
				// Region's top edge inside line.
				return true;
			}
			return false;
		}
    void IconBarMargin_Painted(ICSharpCode.TextEditor.AbstractMargin sender, System.Drawing.Graphics g, System.Drawing.Rectangle rect)
    {
      if (m_active_breakline < 0)
        return;

      ICSharpCode.TextEditor.TextArea textArea = this.ActiveTextAreaControl.TextArea;
      int lineNumber = textArea.Document.GetVisibleLine(m_active_breakline);
      int lineHeight = textArea.TextView.FontHeight;
      int yPos = (int)(lineNumber * lineHeight) - textArea.VirtualTop.Y;
      if (IsLineInsideRegion(yPos, yPos + lineHeight, rect.Y, rect.Bottom))
      {
        if (lineNumber == textArea.Document.GetVisibleLine(m_active_breakline - 1))
        {
          // marker is inside folded region, do not draw it
        }
        else
          textArea.IconBarMargin.DrawArrow(g, yPos);
      }
    }

    // set the ScriptDocument modified state
    void OnDocumentChanged(object sender, ICSharpCode.TextEditor.Document.DocumentEventArgs e)
    {
      Documents.SetDocumentModified(m_document_id, e.Document.UndoStack.CanUndo);
    }

    void CloseCodeCompletionWindow(object sender, EventArgs e)
    {
      if (m_code_complete != null)
      {
        m_code_complete.Closed -= new EventHandler(CloseCodeCompletionWindow);
        m_code_complete.Dispose();
        m_code_complete = null;
      }
    }

    /// <summary>
    /// Return true to handle the keypress, return false to let the text area handle the keypress
    /// </summary>
    bool TextAreaKeyEventHandler(char key)
    {
      if (m_code_complete != null)
      {
        // If completion window is open and wants to handle the key, don't let the text area
        // handle it
        if (m_code_complete.ProcessKeyEvent(key))
          return true;
      }

      ICSharpCode.TextEditor.Gui.CompletionWindow.ICompletionDataProvider completionDataProvider = CodeComplete.GetCompletionProvider(this, key);
      if (completionDataProvider == null && key == '(')
      {
        ICSharpCode.TextEditor.Document.LineSegment line = this.Document.GetLineSegment(ActiveTextAreaControl.Caret.Line);
        if (null == line)
          return false;

        List<ICSharpCode.TextEditor.Document.TextWord> words = line.Words;
        if (words == null || words.Count < 1)
          return false;

        ICSharpCode.TextEditor.Document.TextWord lastWord = words[words.Count - 1];
        if (lastWord.Type != ICSharpCode.TextEditor.Document.TextWordType.Word)
          return false;
        // Todo: Remove hard coded colors
        if (lastWord.Color == System.Drawing.Color.Green || lastWord.Color == System.Drawing.Color.Red)
          return false;

        // make sure first real word on this line is not "def"
        for (int i = 0; i < words.Count; i++)
        {
          if (words[i].Type != ICSharpCode.TextEditor.Document.TextWordType.Word)
            continue;
          if (words[i].Word.Equals("def", StringComparison.Ordinal))
            return false;
          break;
        }

        char c = lastWord.Word[0];
        if (char.IsLetter(c))
        {
          // Build up a python script to execute
          int line_number = this.ActiveTextAreaControl.TextArea.Caret.Line;
          StringBuilder script = new StringBuilder();
          for (int i = 0; i < line_number; i++)
          {
            line = this.ActiveTextAreaControl.TextArea.Document.GetLineSegment(i);
            if (line != null && line.Words.Count > 2)
            {
              string firstword = line.Words[0].Word;
              if (firstword.Equals("import", StringComparison.Ordinal) || firstword.Equals("from"))
              {
                script.AppendLine(this.ActiveTextAreaControl.TextArea.Document.GetText(line));
              }
            }
          }

          ICSharpCode.TextEditor.Document.LineSegment lastLine = this.ActiveTextAreaControl.TextArea.Document.GetLineSegment(line_number);
          if (null == lastLine)
            return false;

          //walk backward through the line until we hit something that is NOT a word or period
          string evaluation = "";
          for (int i = lastLine.Words.Count - 1; i >= 0; i--)
          {
            ICSharpCode.TextEditor.Document.TextWord word = lastLine.Words[i];
            if (word.Type != ICSharpCode.TextEditor.Document.TextWordType.Word)
              break;
            c = word.Word[0];
            if (c != '.' && !char.IsLetter(c))
              break;
            evaluation = evaluation.Insert(0, word.Word);
          }

          if (evaluation != "if" && evaluation != "while")
          {
            RhinoDLR_Python.Intellisense isense = m_intellisense;
            if (isense == null)
              isense = CodeComplete.Intellisense;
            string rc = isense.EvaluateHelp(script.ToString(), evaluation);
            if (!string.IsNullOrEmpty(rc) && null != m_help_callback)
            {
              m_help_callback(rc);
            }
          }          
        }
      }
      else if( completionDataProvider != null )
      {
        ScriptEditor.Model.ScriptDocument doc = ScriptEditor.Model.Documents.DocumentFromId(DocumentId);
        string path = "none";
        if (doc != null && !string.IsNullOrEmpty(doc.FullPath) )
        {
          path = doc.FullPath;
        }

        m_code_complete = ICSharpCode.TextEditor.Gui.CompletionWindow.CodeCompletionWindow.ShowCompletionWindow(
          m_mainform,					// The parent window for the completion window
          this, 					    // The text editor to show the window for
          path,               // Filename - will be passed back to the provider
          completionDataProvider,		// Provider to get the list of possible completions
          key							    // Key pressed - will be passed to the provider
        );
        if (m_code_complete != null)
        {
          // ShowCompletionWindow can return null when the provider returns an empty list
          m_code_complete.Closed += new EventHandler(CloseCodeCompletionWindow);
        }
      }
      return false;
    }

    bool TextArea_DoProcessDialogKey(System.Windows.Forms.Keys keyData)
    {
      return false;
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // EditorTabPage
      // 
      this.Name = "EditorTabPage";
      this.ResumeLayout(false);

    }
  }



  /*
  /// <summary>
  /// Represents an item in the code completion window.
  /// </summary>
  class CodeCompletionData : ICSharpCode.TextEditor.Gui.CompletionWindow.DefaultCompletionData, ICSharpCode.TextEditor.Gui.CompletionWindow.ICompletionData
  {
    IMember member;
    IClass c;
    static VBNetAmbience vbAmbience = new VBNetAmbience();
    static CSharpAmbience csharpAmbience = new CSharpAmbience();

    public CodeCompletionData(IMember member)
      : base(member.Name, null, GetMemberImageIndex(member))
    {
      this.member = member;
    }

    public CodeCompletionData(IClass c)
      : base(c.Name, null, GetClassImageIndex(c))
    {
      this.c = c;
    }

    int overloads = 0;

    public void AddOverload()
    {
      overloads++;
    }

    static int GetMemberImageIndex(IMember member)
    {
      // Missing: different icons for private/public member
      if (member is IMethod)
        return 1;
      if (member is IProperty)
        return 2;
      if (member is IField)
        return 3;
      if (member is IEvent)
        return 6;
      return 3;
    }

    static int GetClassImageIndex(IClass c)
    {
      switch (c.ClassType)
      {
        case ClassType.Enum:
          return 4;
        default:
          return 0;
      }
    }

    string description;

    // DefaultCompletionData.Description is not virtual, but we can reimplement
    // the interface to get the same effect as overriding.
    string ICompletionData.Description
    {
      get
      {
        if (description == null)
        {
          IEntity entity = (IEntity)member ?? c;
          description = GetText(entity);
          if (overloads > 1)
          {
            description += " (+" + overloads + " overloads)";
          }
          description += Environment.NewLine + XmlDocumentationToText(entity.Documentation);
        }
        return description;
      }
    }

    /// <summary>
    /// Converts a member to text.
    /// Returns the declaration of the member as C# or VB code, e.g.
    /// "public void MemberName(string parameter)"
    /// </summary>
    static string GetText(IEntity entity)
    {
      IAmbience ambience = MainForm.IsVisualBasic ? (IAmbience)vbAmbience : csharpAmbience;
      if (entity is IMethod)
        return ambience.Convert(entity as IMethod);
      if (entity is IProperty)
        return ambience.Convert(entity as IProperty);
      if (entity is IEvent)
        return ambience.Convert(entity as IEvent);
      if (entity is IField)
        return ambience.Convert(entity as IField);
      if (entity is IClass)
        return ambience.Convert(entity as IClass);
      // unknown entity:
      return entity.ToString();
    }

    public static string XmlDocumentationToText(string xmlDoc)
    {
      System.Diagnostics.Debug.WriteLine(xmlDoc);
      StringBuilder b = new StringBuilder();
      try
      {
        using (XmlTextReader reader = new XmlTextReader(new StringReader("<root>" + xmlDoc + "</root>")))
        {
          reader.XmlResolver = null;
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Text:
                b.Append(reader.Value);
                break;
              case XmlNodeType.Element:
                switch (reader.Name)
                {
                  case "filterpriority":
                    reader.Skip();
                    break;
                  case "returns":
                    b.AppendLine();
                    b.Append("Returns: ");
                    break;
                  case "param":
                    b.AppendLine();
                    b.Append(reader.GetAttribute("name") + ": ");
                    break;
                  case "remarks":
                    b.AppendLine();
                    b.Append("Remarks: ");
                    break;
                  case "see":
                    if (reader.IsEmptyElement)
                    {
                      b.Append(reader.GetAttribute("cref"));
                    }
                    else
                    {
                      reader.MoveToContent();
                      if (reader.HasValue)
                      {
                        b.Append(reader.Value);
                      }
                      else
                      {
                        b.Append(reader.GetAttribute("cref"));
                      }
                    }
                    break;
                }
                break;
            }
          }
        }
        return b.ToString();
      }
      catch (XmlException)
      {
        return xmlDoc;
      }
    }
  }
  */

  // http://ayende.com/Blog/archive/2008/08/19/Smarter-Syntax-Highlighting.aspx
  class PythonFormattingStrategy : ICSharpCode.TextEditor.Document.DefaultFormattingStrategy
  {
    //public override void IndentLines(ICSharpCode.TextEditor.TextArea textArea, int begin, int end)
    //{
    //}

    protected override int SmartIndentLine(ICSharpCode.TextEditor.TextArea area, int line)
    {
      ICSharpCode.TextEditor.Document.IDocument document = area.Document;
      ICSharpCode.TextEditor.Document.LineSegment lineSegment = document.GetLineSegment(line - 1);
      if (document.GetText(lineSegment).EndsWith(":"))
      {
        ICSharpCode.TextEditor.Document.LineSegment segment = document.GetLineSegment(line);
        string indent_str = "    ";// ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(document);
        if (!area.TextEditorProperties.ConvertTabsToSpaces)
          indent_str = "\t";
        string str = base.GetIndentation(area, line - 1) + indent_str;
        document.Replace(segment.Offset, segment.Length, str + document.GetText(segment));
        return str.Length;
      }
      return base.SmartIndentLine(area, line);
    }
  }
}
