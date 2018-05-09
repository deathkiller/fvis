namespace fVis.Windows
{
    partial class FindDifferencesDialog
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
            this.startLabel = new System.Windows.Forms.Label();
            this.endLabel = new System.Windows.Forms.Label();
            this.startTextBox = new System.Windows.Forms.TextBox();
            this.endTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.mainInstructionLabel = new System.Windows.Forms.Label();
            this.listView = new fVis.Controls.ListView();
            this.SuspendLayout();
            // 
            // startLabel
            // 
            this.startLabel.AutoSize = true;
            this.startLabel.Location = new System.Drawing.Point(10, 51);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(112, 13);
            this.startLabel.TabIndex = 0;
            this.startLabel.Text = "[find diffs.interval start]";
            // 
            // endLabel
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.Location = new System.Drawing.Point(10, 80);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(110, 13);
            this.endLabel.TabIndex = 1;
            this.endLabel.Text = "[find diffs.interval end]";
            // 
            // startTextBox
            // 
            this.startTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startTextBox.Location = new System.Drawing.Point(120, 48);
            this.startTextBox.Name = "startTextBox";
            this.startTextBox.Size = new System.Drawing.Size(427, 20);
            this.startTextBox.TabIndex = 2;
            this.startTextBox.TextChanged += new System.EventHandler(this.OnStartTextBoxTextChanged);
            // 
            // endTextBox
            // 
            this.endTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.endTextBox.Location = new System.Drawing.Point(120, 77);
            this.endTextBox.Name = "endTextBox";
            this.endTextBox.Size = new System.Drawing.Size(427, 20);
            this.endTextBox.TabIndex = 3;
            this.endTextBox.TextChanged += new System.EventHandler(this.OnEndTextBoxTextChanged);
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.AutoEllipsis = true;
            this.infoLabel.Location = new System.Drawing.Point(10, 220);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(374, 13);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(473, 214);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 24);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "[main.close]";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.runButton.Location = new System.Drawing.Point(392, 214);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 24);
            this.runButton.TabIndex = 7;
            this.runButton.Text = "[main.run]";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.OnRunButtonClick);
            // 
            // mainInstructionLabel
            // 
            this.mainInstructionLabel.AutoSize = true;
            this.mainInstructionLabel.Location = new System.Drawing.Point(10, 16);
            this.mainInstructionLabel.Name = "mainInstructionLabel";
            this.mainInstructionLabel.Size = new System.Drawing.Size(106, 13);
            this.mainInstructionLabel.TabIndex = 8;
            this.mainInstructionLabel.Text = "[find diffs.description]";
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.EmptyText = null;
            this.listView.FocusedItem = null;
            this.listView.Location = new System.Drawing.Point(7, 108);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(540, 100);
            this.listView.TabIndex = 5;
            // 
            // FindDifferencesDialog
            // 
            this.AcceptButton = this.runButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(554, 245);
            this.Controls.Add(this.mainInstructionLabel);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.endTextBox);
            this.Controls.Add(this.startTextBox);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.startLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 220);
            this.Name = "FindDifferencesDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[find diffs.title]";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.TextBox startTextBox;
        private System.Windows.Forms.TextBox endTextBox;
        private System.Windows.Forms.Label infoLabel;
        private Controls.ListView listView;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Label mainInstructionLabel;
    }
}