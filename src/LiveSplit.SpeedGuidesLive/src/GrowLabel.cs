using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.SpeedGuidesLive
{
    public class GrowLabel : Label
    {
        private Control m_parent = null;
        private bool m_resizing = false;

        public GrowLabel()
        {
            AutoSize = false;
            ParentChanged += GrowLabel_ParentChanged;
        }

        private void GrowLabel_ParentChanged(object sender, EventArgs e)
        {
            if(null != m_parent)
            {
                m_parent.SizeChanged -= Parent_SizeChanged;
                m_parent = null;
            }
            m_parent = Parent;
            if (null != m_parent)
            {
                m_parent.SizeChanged += Parent_SizeChanged;
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            resizeLabel();
        }

        private void resizeLabel()
        {
            // Make sure we have a parent and we're not already resizing
            if (null == Parent || m_resizing)
            {
                return;
            }

            try
            {
                m_resizing = true;
                // Get the parent's width and measure the text
                Size sz = new Size(Parent.Width, Int32.MaxValue);
                sz = TextRenderer.MeasureText(Text, Font, sz, TextFormatFlags.WordBreak);
                // Set the label to our measured text
                Size = sz;
            }
            finally
            {
                m_resizing = false;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            resizeLabel();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            resizeLabel();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            resizeLabel();
        }

        protected override void WndProc(ref Message msg)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = (-1);

            if (msg.Msg == WM_NCHITTEST)
            {
                msg.Result = (IntPtr)HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref msg);
            }
        }
    }
}