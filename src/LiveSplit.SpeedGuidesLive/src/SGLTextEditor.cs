﻿using System;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using Markdig;

namespace LiveSplit.SpeedGuidesLive
{
    public partial class SGLTextEditor : Form
    {
        private static Size s_windowSize = Size.Empty;
        private static Point s_location = Point.Empty;

        private MarkdownPipeline m_markdownRenderer = null;

        public string EditorText
        {
            get { return editorTextBox.Text; }
            set { editorTextBox.Text = value; }
        }

        public SGLTextEditor()
        {
            InitializeComponent();

            if (Size.Empty == s_windowSize)
            {
                s_windowSize = Size;
                s_location = Location;
            }
            else
            {
                Size = s_windowSize;
                Location = s_location;
            }

            SizeChanged += SGLTextEditor_SizeChanged;
            LocationChanged += SGLTextEditor_LocationChanged;

            editorTextBox.TextChanged += EditorTextBox_TextChanged;
            webBrowser.Navigate("about:blank");
            m_markdownRenderer = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseEmojiAndSmiley()
                        .Build();
        }

        private void EditorTextBox_TextChanged(object sender, EventArgs e)
        {
            HtmlDocument doc = webBrowser.Document.OpenNew(true);
            doc.Write(
                $@"<html><head><style>
                    img{{max-width:100%;}}
                    pre{{word-wrap:break-word;}}
                </style></head><body>
                    {Markdown.ToHtml(HttpUtility.HtmlEncode(EditorText), m_markdownRenderer)}
                </body></html>");
        }

        private void SGLTextEditor_LocationChanged(object sender, EventArgs e)
        {
            s_location = Location;
        }

        private void SGLTextEditor_SizeChanged(object sender, EventArgs e)
        {
            s_windowSize = Size;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
