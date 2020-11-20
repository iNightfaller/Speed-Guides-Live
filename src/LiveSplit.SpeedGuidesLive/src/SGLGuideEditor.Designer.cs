namespace LiveSplit.SpeedGuidesLive
{
    partial class SGLGuideEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SGLGuideEditor));
            this.rootPanel = new LiveSplit.SpeedGuidesLive.NoScrollPanel();
            this.childPanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.leftPage = new System.Windows.Forms.Button();
            this.rightPage = new System.Windows.Forms.Button();
            this.pageTxt = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rootPanel.AutoScroll = true;
            this.rootPanel.Controls.Add(this.childPanel);
            this.rootPanel.Location = new System.Drawing.Point(0, 32);
            this.rootPanel.Margin = new System.Windows.Forms.Padding(2);
            this.rootPanel.Name = "rootPanel";
            this.rootPanel.Size = new System.Drawing.Size(579, 353);
            this.rootPanel.TabIndex = 0;
            // 
            // childPanel
            // 
            this.childPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.childPanel.Location = new System.Drawing.Point(0, 0);
            this.childPanel.Name = "childPanel";
            this.childPanel.Size = new System.Drawing.Size(579, 100);
            this.childPanel.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(8, 5);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(88, 21);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // reloadButton
            // 
            this.reloadButton.Location = new System.Drawing.Point(115, 5);
            this.reloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(88, 21);
            this.reloadButton.TabIndex = 2;
            this.reloadButton.Text = "Reload";
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // leftPage
            // 
            this.leftPage.Location = new System.Drawing.Point(374, 4);
            this.leftPage.Name = "leftPage";
            this.leftPage.Size = new System.Drawing.Size(29, 23);
            this.leftPage.TabIndex = 3;
            this.leftPage.Text = "<";
            this.leftPage.UseVisualStyleBackColor = true;
            // 
            // rightPage
            // 
            this.rightPage.Location = new System.Drawing.Point(463, 4);
            this.rightPage.Name = "rightPage";
            this.rightPage.Size = new System.Drawing.Size(29, 23);
            this.rightPage.TabIndex = 4;
            this.rightPage.Text = ">";
            this.rightPage.UseVisualStyleBackColor = true;
            // 
            // pageTxt
            // 
            this.pageTxt.AutoSize = true;
            this.pageTxt.Location = new System.Drawing.Point(419, 9);
            this.pageTxt.Name = "pageTxt";
            this.pageTxt.Size = new System.Drawing.Size(24, 13);
            this.pageTxt.TabIndex = 5;
            this.pageTxt.Text = "1/3";
            // 
            // SGLGuideEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 385);
            this.Controls.Add(this.pageTxt);
            this.Controls.Add(this.rightPage);
            this.Controls.Add(this.leftPage);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.rootPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SGLGuideEditor";
            this.Text = "SGLGuideEditor";
            this.rootPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.Panel childPanel;
        private System.Windows.Forms.Button leftPage;
        private System.Windows.Forms.Button rightPage;
        private System.Windows.Forms.Label pageTxt;
        private NoScrollPanel rootPanel;
    }
}