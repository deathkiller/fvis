namespace fVis.Windows
{
    partial class AboutDialog
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
            if (disposing && (components != null)) {
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
            this.linkLabel = new Death.Controls.LinkLabel();
            this.SuspendLayout();
            // 
            // linkLabel
            // 
            this.linkLabel.HoverColor = System.Drawing.SystemColors.Highlight;
            this.linkLabel.HoverUnderline = true;
            this.linkLabel.Location = new System.Drawing.Point(21, 90);
            this.linkLabel.MaxWidth = 0;
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.RegularColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel.Size = new System.Drawing.Size(170, 14);
            this.linkLabel.TabIndex = 0;
            this.linkLabel.Text = "https://github.com/deathkiller/fvis";
            this.linkLabel.Click += new System.EventHandler(this.OnLinkLabelClick);
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 118);
            this.Controls.Add(this.linkLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[main.about]";
            this.ResumeLayout(false);

        }

        #endregion

        private Death.Controls.LinkLabel linkLabel;
    }
}