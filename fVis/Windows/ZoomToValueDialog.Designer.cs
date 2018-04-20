﻿namespace fVis.Windows
{
    partial class ZoomToValueDialog
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
            this.mainInstructionLabel = new System.Windows.Forms.Label();
            this.findButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.listView = new fVis.Controls.ListView();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.startLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainInstructionLabel
            // 
            this.mainInstructionLabel.AutoSize = true;
            this.mainInstructionLabel.Location = new System.Drawing.Point(10, 11);
            this.mainInstructionLabel.Name = "mainInstructionLabel";
            this.mainInstructionLabel.Size = new System.Drawing.Size(412, 13);
            this.mainInstructionLabel.TabIndex = 14;
            this.mainInstructionLabel.Text = "Enter a value of x and select a function. The application will zoom to the desire" +
    "d point.";
            // 
            // findButton
            // 
            this.findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.findButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findButton.Location = new System.Drawing.Point(384, 214);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(75, 24);
            this.findButton.TabIndex = 13;
            this.findButton.Text = "Zoom";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.OnFindButtonClick);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(465, 214);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 24);
            this.closeButton.TabIndex = 12;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.FocusedItem = null;
            this.listView.Location = new System.Drawing.Point(7, 73);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(532, 135);
            this.listView.TabIndex = 11;
            this.listView.ItemPropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OnItemPropertyChanged);
            this.listView.ItemSelectionChanged += new System.EventHandler(this.OnItemSelectionChanged);
            // 
            // valueTextBox
            // 
            this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTextBox.Location = new System.Drawing.Point(95, 43);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(444, 20);
            this.valueTextBox.TabIndex = 10;
            this.valueTextBox.TextChanged += new System.EventHandler(this.OnValueTextBoxTextChanged);
            // 
            // startLabel
            // 
            this.startLabel.AutoSize = true;
            this.startLabel.Location = new System.Drawing.Point(10, 46);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(57, 13);
            this.startLabel.TabIndex = 9;
            this.startLabel.Text = "Value of x:";
            // 
            // ZoomToValueDialog
            // 
            this.AcceptButton = this.findButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(546, 245);
            this.Controls.Add(this.mainInstructionLabel);
            this.Controls.Add(this.findButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.startLabel);
            this.Name = "ZoomToValueDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zoom To Value";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mainInstructionLabel;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.Button closeButton;
        private Controls.ListView listView;
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label startLabel;
    }
}