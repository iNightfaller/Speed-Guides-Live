using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.SpeedGuidesLive
{
    public partial class SGLGuideEditor : Form
    {
        private static int s_elementHeight = 100;

        private static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                return;
            }

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        private class NoteElement
        {
            public Label label = new Label();
            public Button editButton = new Button();
            public TextBox textBox = new TextBox();
            public int index = -1;
            public Control m_parent = null;

            public NoteElement(Control parent, int _index)
            {
                m_parent = parent;
                parent.Controls.Add(label);
                parent.Controls.Add(editButton);
                parent.Controls.Add(textBox);
                index = _index;
            }

            public void Destroy()
            {
                if(null != m_parent)
                {
                    m_parent.Controls.Remove(label);
                    m_parent.Controls.Remove(editButton);
                    m_parent.Controls.Remove(textBox);
                }
            }
        }

        private LiveSplitState m_state;

        private Guide m_guide = null;
        private List<NoteElement> m_noteElements = new List<NoteElement>();
        private int m_scrollPos = 0;

        private int m_curPage = 0;
        private int m_totalPages = 0;

        public SGLGuideEditor(LiveSplitState state)
        {
            InitializeComponent();

            rootPanel.Scroll += RootPanel_Scroll;
            rootPanel.MouseWheel += RootPanel_MouseWheel;
            m_state = state;
            m_scrollPos = Math.Abs(rootPanel.VerticalScroll.Value);

            leftPage.MouseClick += LeftPage_MouseClick;
            rightPage.MouseClick += RightPage_MouseClick;

            SetDoubleBuffered(rootPanel);
            SetDoubleBuffered(childPanel);

            LoadGuide();
            ValidateGuide();
            SetupNoteElements(true);
        }

        private void LeftPage_MouseClick(object sender, MouseEventArgs e)
        {
            if (0 < m_curPage)
            {
                --m_curPage;
                pageTxt.Text = (m_curPage + 1).ToString() + "/" + (m_totalPages + 1).ToString();
                rootPanel.VerticalScroll.Value = 0;
                m_scrollPos = 0;
                SetupNoteElements(true);
            }
        }

        private void RightPage_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_totalPages > m_curPage)
            {
                ++m_curPage;
                pageTxt.Text = (m_curPage + 1).ToString() + "/" + (m_totalPages + 1).ToString();
                rootPanel.VerticalScroll.Value = 0;
                m_scrollPos = 0;
                SetupNoteElements(true);
            }
        }

        private void CleanNoteElements()
        {
            foreach(NoteElement noteElement in m_noteElements)
            {
                noteElement.Destroy();
            }

            m_noteElements.Clear();
        }

        private void SetupNoteElements(bool init)
        {
            if (init)
            {
                CleanNoteElements();
            }

            if (null == m_guide)
            {
                return;
            }

            float textBoxWidthPct = 0.75f;

            int scrollLoadBuffer = 5;
            int scrollTop = m_scrollPos - scrollLoadBuffer;
            int scrollBot = m_scrollPos + rootPanel.Size.Height + scrollLoadBuffer;

            if (!init)
            {
                for(int i = 0; i < m_noteElements.Count; ++i)
                {
                    NoteElement element = m_noteElements[i];
                    int yPos = element.index * s_elementHeight + 5;
                    int bot = yPos + s_elementHeight + 5;
                    if (bot < scrollTop || yPos > scrollBot)
                    {
                        element.Destroy();
                        m_noteElements.RemoveAt(i);
                        --i;
                    }
                }
            }

            int totalSplitCt = 30000 / s_elementHeight;
            int splitIndex = m_curPage * totalSplitCt;
            int splitEndIndex = Math.Min(splitIndex + totalSplitCt, m_guide.Splits.Count);
            int childHeight = s_elementHeight * (splitEndIndex - splitIndex) + 10;
            childHeight = Math.Min(childHeight, 30010);

            rootPanel.VerticalScroll.Maximum = childHeight;
            childPanel.Size = new Size(childPanel.Size.Width, childHeight);
            int index = 0;
            for(; splitIndex < splitEndIndex; ++splitIndex)
            {
                Guide.Split split = m_guide.Splits[splitIndex];

                int yPos = index * s_elementHeight + 5;
                int bot = yPos + s_elementHeight + 5;
                if (bot < scrollTop || yPos > scrollBot)
                {
                    ++index;
                    continue;
                }

                bool canCreate = true;
                for (int j = 0; j < m_noteElements.Count; ++j)
                {
                    NoteElement element = m_noteElements[j];
                    if(element.index == index)
                    {
                        canCreate = false;
                        break;
                    }
                }
                if(!canCreate)
                {
                    ++index;
                    continue;
                }

                NoteElement ele = new NoteElement(childPanel, index);

                ele.textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                ele.label.Text = split.Name;
                ele.label.Left = 3;
                ele.label.Top = yPos;
                ele.label.Size = new Size((int)((1.0f - textBoxWidthPct) * (float)childPanel.Width), ele.label.Size.Height);
                ele.label.BackColor = System.Drawing.Color.Transparent;

                ele.textBox.Multiline = true;
                split.Note = split.Note.Replace("\r\n", "\n");
                split.Note = split.Note.Replace("\n", "\r\n");
                ele.textBox.Text = split.Note;

                ele.textBox.Left = (int)((1.0f - textBoxWidthPct) * (float)childPanel.Width);
                ele.textBox.Top = yPos;
                ele.textBox.Size = new Size((int)(textBoxWidthPct * (float)childPanel.Width)-16, s_elementHeight);
                ele.textBox.ScrollBars = ScrollBars.Vertical;
                ele.textBox.BringToFront();
                ele.textBox.TextChanged += (sender, e) =>
                {
                    string text = ele.textBox.Text;
                    text = text.Replace("\r\n", "\n");
                    text = text.Replace("\n", "\r\n");
                    split.Note = text;
                };

                ele.editButton.Size = new Size(64, 32);
                ele.editButton.Text = "Edit";
                ele.editButton.Left = ele.textBox.Left - ele.editButton.Size.Width - (ele.editButton.Size.Width / 2);
                ele.editButton.Top = ele.textBox.Top + (ele.textBox.Size.Height / 2) - (ele.editButton.Size.Height / 2);
                ele.editButton.Click += (sender, e) =>
                {
                    SGLTextEditor textEditor = new SGLTextEditor();
                    textEditor.EditorText = ele.textBox.Text;
                    if (textEditor.ShowDialog() == DialogResult.OK)
                    {
                        ele.textBox.Text = textEditor.EditorText;
                    }
                    textEditor.Dispose();
                };

                m_noteElements.Add(ele);
                ++index;
            }
        }

        private void LoadGuide()
        {
            m_guide = Guide.Load(m_state.Run.FilePath);
            if (null == m_guide)
            {
                m_guide = new Guide();
            }
        }

        // Check the guide and make sure that all of the splits are in the guide
        public static void ValidateGuide(Guide guide, LiveSplitState state)
        {
            guide.Name = state.Run.GameName;
            guide.Category = state.Run.CategoryName;
            List<Guide.Split> prevSplits = guide.Splits;
            guide.Splits = new List<Guide.Split>();
            foreach (ISegment split in state.Run)
            {
                bool found = false;
                for (int i = 0; i < prevSplits.Count; ++i)
                {
                    Guide.Split guideSplit = prevSplits[i];
                    if (split.Name == guideSplit.Name)
                    {
                        found = true;
                        prevSplits.RemoveAt(i);
                        guide.Splits.Add(guideSplit);
                        break;
                    }
                }

                if (!found)
                {
                    guide.Splits.Add(new Guide.Split(split.Name));
                }
            }
        }

        private void ValidateGuide()
        {
            ValidateGuide(m_guide, m_state);
            m_curPage = 0;
            m_totalPages = s_elementHeight * m_guide.Splits.Count / 30000;
            pageTxt.Text = (m_curPage + 1).ToString() + "/" + (m_totalPages + 1).ToString();
        }

        private void SGLGuideEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoadGuide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (null != m_guide)
            {
                m_guide.Save(m_state.Run.FilePath);
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            LoadGuide();
            SetupNoteElements(true);
        }

        private void RootPanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                m_scrollPos = Math.Abs(e.NewValue);
                SetupNoteElements(false);
            }
        }

        private void RootPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            m_scrollPos = Math.Abs(rootPanel.VerticalScroll.Value);
            SetupNoteElements(false);
        }
    }
}
