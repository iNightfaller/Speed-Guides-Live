using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using Markdig;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web;
using System.Windows.Forms;


namespace LiveSplit.SpeedGuidesLive
{
    public partial class SGLGuideWindow : Form
    {
        private const int s_gripResizeSize = 16;

        private static SGLGuideWindow s_guideWindow = null;
        public static SGLGuideWindow GuideWindow { get { return s_guideWindow; } }

        public delegate void WindowCreatedEventHandler();
        public static WindowCreatedEventHandler WindowCreatedEvent { get; set; }
        public delegate void WindowClosedEventHandler();
        public static WindowClosedEventHandler WindowClosedEvent { get; set; }

        private Size m_startingSize = Size.Empty;

        private SGLComponent m_component = null;
        private Form m_parentForm = null;
        private ILayout m_layout = null;
        private SplitsComponent m_splitsComponent = null;
        private Guide m_guide = null;
        private Brush m_backgroundBrush = new SolidBrush(Color.FromArgb(16, 16, 16));
        private Color m_backgroundColor = Color.FromArgb(16, 16, 16);
        private Color m_textColor = Color.White;
        private int m_currentSplitIndex = -1;
        private MarkdownPipeline m_markdownRenderer = null;

        public SGLGuideWindow(SGLComponent component, Form parentForm, ILayout layout, Guide guide)
        {
            s_guideWindow = this;
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            TopMost = layout.Settings.AlwaysOnTop;
            TopMost = false;

            InitializeComponent();

            m_component = component;
            m_parentForm = parentForm;
            m_layout = layout;
            m_guide = guide;

            OnBackgroundColorChanged(m_component.Settings.BackgroundColor);
            OnTextColorChanged(m_component.Settings.TextColor);

            ValidateComponents(true);

            WindowCreatedEvent.Invoke();

            // browser needs to be set to a default page in order to have a valid document
            Browser.Navigate("about:blank");
            Browser.PreviewKeyDown += Browser_PreviewKeyDown;
            m_markdownRenderer = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseEmojiAndSmiley()
                        .Build();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetPosition(m_component.Settings.WindowPos);
            SetSize(m_component.Settings.WindowSize);
            m_startingSize = ClientSize;

            OnBackgroundColorChanged(m_component.Settings.BackgroundColor);
            OnTextColorChanged(m_component.Settings.TextColor);

            m_component.Settings.BackgroundColorChangedEvent += OnBackgroundColorChanged;
            m_component.Settings.TextColorChangedEvent += OnTextColorChanged;
            m_component.Settings.FontChangedEvent += OnFontChanged;
            m_component.Settings.MardownEnableChangedEvent += OnMarkdownChanged;

            // Debug
            m_component.Settings.DebugCenterEvent += OnDebugCenter;
            m_component.Settings.DebugResizeEvent += OnDebugResize;

            LocationChanged += SGLGuideWindow_LocationChanged;
            SizeChanged += SGLGuideWindow_SizeChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            m_component.Settings.BackgroundColorChangedEvent -= OnBackgroundColorChanged;
            m_component.Settings.TextColorChangedEvent -= OnTextColorChanged;
            m_component.Settings.FontChangedEvent -= OnFontChanged;
            m_component.Settings.MardownEnableChangedEvent -= OnMarkdownChanged;

            // Debug
            m_component.Settings.DebugCenterEvent -= OnDebugCenter;
            m_component.Settings.DebugResizeEvent -= OnDebugResize;

            LocationChanged -= SGLGuideWindow_LocationChanged;
            SizeChanged -= SGLGuideWindow_SizeChanged;
        }

        public new void Close()
        {
            if (null != s_guideWindow)
            {
                WindowClosedEvent.Invoke();
                s_guideWindow = null;
            }
            base.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (null != s_guideWindow)
            {
                WindowClosedEvent.Invoke();
                s_guideWindow = null;
            }
            base.OnClosed(e);
        }

        public void SetSplit(ISegment split, int splitIndex)
        {
            m_currentSplitIndex = splitIndex;
            Invoke(new MethodInvoker(delegate
            {
                try
                {
                    if (null != split)
                    {
                        if (m_guide.Splits.Count > splitIndex && 0 <= splitIndex)
                        {
                            SetGuideText(m_guide.Splits[splitIndex].Note);

                            UpdateActiveSplitTxt(m_guide.Splits[splitIndex].Note);
                        }
                    }
                    else
                    {
                        SetGuideText(string.Empty);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Failed to set splits!! Exception: {0}", e.Message));
                }
            }));
        }

        public void SetGuide(Guide guide)
        {
            m_guide = guide;
        }

        private void ValidateComponents(bool force)
        {
            if (null != m_splitsComponent && !force)
            {
                return;
            }

            m_splitsComponent = null;

            if (null == m_layout)
            {
                return;
            }

            foreach (ILayoutComponent layoutComponent in m_layout.LayoutComponents)
            {
                if (layoutComponent.Component.GetType() == typeof(SplitsComponent))
                {
                    m_splitsComponent = (SplitsComponent)layoutComponent.Component;
                    break;
                }
            }
        }

        /// <summary>
        /// Convert markdown to a valid html page.
        /// </summary>
        /// <param name="text">Markdown string that is in the process of being rendered.</param>
        /// <returns>A string containing valid html that can be placed into a web browser.</returns>
        private string GenerateHtmlFromMD(string text)
        {
            // convert the markdown to html and add it to the browser page
            string notes;
            try
            {
                if (m_component.Settings.MarkdownEnabled)
                {
                    notes = Markdown.ToHtml(HttpUtility.HtmlEncode(text), m_markdownRenderer);
                }
                else
                {
                    notes = $"<pre>{text}</pre>";
                }
            }
            catch (Exception e)
            {
                notes = e.Message;
            }

            // set the styles for the browser window based on user settings
            return
                $@"<html><head><style>
                    html,body{{
                        background-color: rgb({m_backgroundColor.R}, {m_backgroundColor.G}, {m_backgroundColor.B});
                        color: rgb({m_textColor.R}, {m_textColor.G}, {m_textColor.B});
                        font-family: {m_component.Settings.GuideFont.Name};
                        font-size: {m_component.Settings.GuideFont.Size}px;
                    }}
                    img{{max-width:100%;}}
                    pre{{word-wrap:break-word;}}
                </style></head>
                <body>
                    {notes}
                </body>
                </html>";
        }

        /// <summary>
        /// Set the current notes being displayed.
        /// </summary>
        /// <param name="text">A string containing markdown formatted text to be displayed.</param>
        private void SetGuideText(string text)
        {
            // fill in the browser document contents with raw html containing the user notes
            if (Browser.Document != null)
            {                
                HtmlDocument doc = Browser.Document.OpenNew(true);
                doc.Write(GenerateHtmlFromMD(text));
                doc.MouseDown += Document_MouseDown;
            }
        }

        /// <summary>
        /// Refresh the current notes on display. This is used when settings are changed for SGL.
        /// </summary>
        private void RefreshGuide()
        {
            if (m_currentSplitIndex >= 0 && m_guide != null)
            {
                SetGuideText(m_guide.Splits[m_currentSplitIndex].Note);
            }
        }

        private void SGLGuideWindow_SizeChanged(object sender, EventArgs e)
        {
            SetSize(Size);
        }

        private void SGLGuideWindow_LocationChanged(object sender, EventArgs e)
        {
            SetPosition(Location);
        }

        private void UpdateActiveSplitTxt(string text)
        {
            string splitTxtPath = m_component.Settings.ActiveSplitTxtOutputPath;
            if (0 == splitTxtPath.Length)
            {
                return;
            }

            if(!File.Exists(splitTxtPath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(splitTxtPath)))
                    return;
            }

            string ext = Path.GetExtension(splitTxtPath);
            if (0 == ext.Length)
            {
                splitTxtPath += ".txt";
            }
            else if(".txt" != ext)
            {
                splitTxtPath.Replace(ext, ".txt");
            }

            try
            {
                File.WriteAllText(splitTxtPath, text);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to write split guide to txt: " + e.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(ClientSize.Width - s_gripResizeSize, ClientSize.Height - s_gripResizeSize, s_gripResizeSize, s_gripResizeSize);
            ControlPaint.DrawSizeGrip(e.Graphics, BackColor, rc);
            rc = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            e.Graphics.FillRectangle(m_backgroundBrush, rc);
            base.OnPaint(e);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WM_NCLBUTTONMOVE = 0x00A0;
        private const int HT_CAPTION = 0x2;
        private const int HT_BOTTOMRIGHT = 17;
        protected bool HandleWndProc(ref Message msg)
        {
            if (msg.Msg == WM_NCHITTEST || msg.Msg == WM_MOUSEMOVE)
            {
                Point pos = new Point(msg.LParam.ToInt32() & 0xffff, msg.LParam.ToInt32() >> 16);
                pos = PointToClient(pos);
                if (pos.X >= ClientSize.Width - s_gripResizeSize && pos.Y >= ClientSize.Height - s_gripResizeSize)
                {
                    msg.Result = (IntPtr)HT_BOTTOMRIGHT;
                    return true;
                }
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            if (!HandleWndProc(ref m))
            {
                base.WndProc(ref m);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        /// <summary>
        /// Event handler for the base window to move/resize.
        /// </summary>
        private void SGLGuideWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if  (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /// <summary>
        /// Event Handler for the browser to pass off mouse events properly for moving/resizing the window.
        /// </summary>
        private void Document_MouseDown(object sender, HtmlElementEventArgs e)
        {
            if (e.MouseButtonsPressed == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /// <summary>
        /// Focus event for the browser component. This is a hack to force focus back to LiveSplit.
        /// This is done as a workaround so that splitting does not return focus to the browser.
        /// </summary>
        private void Browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (Focused || Browser.Focused)
            {
                // This is definitely a hack, but the browser is grabbing focus from key down events while splitting.
                // Putting this inside a focus event disables the ability to move and resize the window, however,
                // keyboard is not used to control the notes window so ... this technically works.
                // In addition: this is only an issue while the livesplit window has focus! The browser is not intercepting
                // key events while things such as OBS, Chrome, etc are in the foreground.
                m_parentForm.Focus();
            }
        }

        private void OnFontChanged(Font font)
        {
            RefreshGuide();
        }

        private void OnMarkdownChanged(bool enabled)
        {
            RefreshGuide();
        }

        private void OnBackgroundColorChanged(Color color)
        {
            m_backgroundColor = color;
            m_backgroundBrush = new SolidBrush(color);
            Invalidate();
            RefreshGuide();
        }

        private void OnTextColorChanged(Color color)
        {
            m_textColor = color;
            RefreshGuide();
        }

        private void SetPosition(Point pos)
        {
            Location = pos;
            if(null != m_component)
            {
                m_component.Settings.WindowPos = pos;
            }
        }

        private void SetSize(Size size)
        {
            ClientSize = size;
            if (null != m_component)
            {
                m_component.Settings.WindowSize = size;
            }
        }

        private void OnDebugCenter()
        {
            CenterToScreen();
            SetPosition(Location);
        }

        private void OnDebugResize()
        {
            SetSize(SGLSettings.StartingSize);
        }
    }
}
