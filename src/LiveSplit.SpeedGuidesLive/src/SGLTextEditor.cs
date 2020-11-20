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
    public partial class SGLTextEditor : Form
    {
        private static Size s_windowSize = Size.Empty;
        private static Point s_location = Point.Empty;

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
