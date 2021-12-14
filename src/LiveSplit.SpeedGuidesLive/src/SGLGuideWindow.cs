using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        private int m_yOffset = 0;
        private Brush m_backgroundBrush = new SolidBrush(Color.FromArgb(16, 16, 16));
        private Color m_backgroundColor = Color.FromArgb(16, 16, 16);
        private Color m_textColor = Color.White;

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

            Browser.Navigate("about:blank");
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
            Invoke(new MethodInvoker(delegate
            {
                try
                {
                    ClearLabels();
                    if (null != split)
                    {
                        if (m_guide.Splits.Count > splitIndex && 0 <= splitIndex)
                        {
                            AddLabel(m_guide.Splits[splitIndex].Note);

                            UpdateActiveSplitTxt(m_guide.Splits[splitIndex].Note);
                        }
                    }
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Failed to set splits!!");
                }
            }));
            
            SetScrollPos(0);
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

        private string GenerateHtmlFromMD(string text)
        {
            string html = "<html><head><style>";

            html += "html,body{background-color: rgb(" + m_backgroundColor.R.ToString() + ", " + m_backgroundColor.G.ToString() + ", " + m_backgroundColor.B.ToString() + ");}";
            html += "html,body{color: rgb(" + m_textColor.R.ToString() + ", " + m_textColor.G.ToString() + ", " + m_textColor.B.ToString() + ");}";
            html += "</style></head><body>";

            // TODO: replace this with a markdown parser
            html += string.Format("<pre>{0}</pre>", text);

            html += "</body></html>";

            return html;
        }

        private void ClearLabels()
        {
            if (Browser.Document != null)
            {
                HtmlDocument doc = Browser.Document.OpenNew(true);
                doc.Write(GenerateHtmlFromMD(string.Empty));
            }
        }

        private void AddLabel(string text)
        {
            if (Browser.Document != null)
            {                
                HtmlDocument doc = Browser.Document.OpenNew(true);
                doc.Write(GenerateHtmlFromMD(text));
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

        private void NewLabel_MouseDown(object sender, MouseEventArgs e)
        {
            SGLGuideWindow_MouseDown(sender, e);
        }

        private void NewLabel_MouseMove(object sender, MouseEventArgs e)
        {
            SGLGuideWindow_MouseMove(sender, e);
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
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(splitTxtPath)))
                    return;
            }

            string ext = System.IO.Path.GetExtension(splitTxtPath);
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
                System.IO.File.WriteAllText(splitTxtPath, text);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to write split guide to txt: " + e.Message);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int scrollRate = 8;
            SetScrollPos(m_yOffset + e.Delta / scrollRate);
        }

        private void SetScrollPos(int pos)
        {
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

        private void SGLGuideWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if  (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void SGLGuideWindow_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONMOVE, HT_CAPTION, 0);
        }

        private void OnFontChanged(Font font)
        {
        }

        private void OnBackgroundColorChanged(Color color)
        {
            m_backgroundColor = color;
            m_backgroundBrush = new SolidBrush(color);
            Invalidate();
        }

        private void OnTextColorChanged(Color color)
        {
            m_textColor = color;
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
            // this is 100% a hack to make the browser slightly smaller than the containing window
            // this allows existing resizing functionality to stay in place
            Size browserSize = size;
            browserSize.Width -= Browser.Margin.Horizontal;
            browserSize.Height -= Browser.Margin.Vertical;
            Browser.Size = browserSize;
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
