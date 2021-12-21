namespace LiveSplit.SpeedGuidesLive
{
    partial class SGLGuideWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SGLGuideWindow));
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // Browser
            // 
            this.Browser.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.Browser.AllowNavigation = false;
            this.Browser.AllowWebBrowserDrop = false;
            this.Browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Browser.CausesValidation = false;
            this.Browser.IsWebBrowserContextMenuEnabled = false;
            this.Browser.Location = new System.Drawing.Point(3, 3);
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(444, 239);
            this.Browser.TabIndex = 0;
            this.Browser.TabStop = false;
            this.Browser.WebBrowserShortcutsEnabled = false;
            // 
            // SGLGuideWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(450, 245);
            this.Controls.Add(this.Browser);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(80, 40);
            this.Name = "SGLGuideWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SGLGuideWindow";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SGLGuideWindow_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser Browser;
    }
}