﻿using fVis.Controls;

namespace fVis.Windows
{
    partial class MainWindow
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.graph = new fVis.Controls.Graph();
            this.listView = new fVis.Controls.ListView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addButton = new System.Windows.Forms.ToolStripButton();
            this.removeButton = new System.Windows.Forms.ToolStripButton();
            this.duplicateButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importButton = new System.Windows.Forms.ToolStripButton();
            this.exportButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.implementationButton = new System.Windows.Forms.ToolStripSplitButton();
            this.highlightDifferencesButton = new System.Windows.Forms.ToolStripSplitButton();
            this.analyzeButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.scaleFactorBox = new System.Windows.Forms.ToolStripTextBox();
            this.zoomInButton = new System.Windows.Forms.ToolStripButton();
            this.resetButton = new System.Windows.Forms.ToolStripButton();
            this.zoomToValueButton = new System.Windows.Forms.ToolStripButton();
            this.helpDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.langMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 26);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.graph);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listView);
            this.splitContainer.Size = new System.Drawing.Size(870, 467);
            this.splitContainer.SplitterDistance = 297;
            this.splitContainer.TabIndex = 1;
            // 
            // graph
            // 
            this.graph.DataSource = null;
            this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph.HighlightDifferences = fVis.Controls.Graph.HighlightDifferencesMode.None;
            this.graph.Location = new System.Drawing.Point(0, 0);
            this.graph.Name = "graph";
            this.graph.Size = new System.Drawing.Size(870, 297);
            this.graph.TabIndex = 0;
            this.graph.Text = "Graph";
            this.graph.ScaleFactorChanged += new System.Action(this.OnScaleFactorChanged);
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.EmptyText = null;
            this.listView.FocusedItem = null;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(870, 166);
            this.listView.TabIndex = 0;
            this.listView.Text = "ListView";
            this.listView.ItemPropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OnItemPropertyChanged);
            this.listView.BeforeEdit += new System.ComponentModel.CancelEventHandler(this.OnItemBeforeEdit);
            this.listView.ItemSelectionChanged += new System.EventHandler(this.OnItemSelectionChanged);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.addButton,
            this.removeButton,
            this.duplicateButton,
            this.toolStripSeparator1,
            this.importButton,
            this.exportButton,
            this.toolStripSeparator4,
            this.implementationButton,
            this.highlightDifferencesButton,
            this.analyzeButton,
            this.toolStripSeparator2,
            this.zoomOutButton,
            this.scaleFactorBox,
            this.zoomInButton,
            this.resetButton,
            this.zoomToValueButton,
            this.helpDropDown});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(870, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.TabStop = true;
            this.toolStrip.Text = "ToolStrip";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // addButton
            // 
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(69, 22);
            this.addButton.Text = "[main.add]";
            this.addButton.ToolTipText = "[main.tooltips.add]";
            this.addButton.Click += new System.EventHandler(this.OnAddButtonClick);
            // 
            // removeButton
            // 
            this.removeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(89, 22);
            this.removeButton.Text = "[main.remove]";
            this.removeButton.ToolTipText = "[main.tooltips.remove]";
            this.removeButton.Click += new System.EventHandler(this.OnRemoveButtonClick);
            // 
            // duplicateButton
            // 
            this.duplicateButton.Name = "duplicateButton";
            this.duplicateButton.Size = new System.Drawing.Size(98, 22);
            this.duplicateButton.Text = "[main.duplicate]";
            this.duplicateButton.ToolTipText = "[main.tooltips.duplicate]";
            this.duplicateButton.Click += new System.EventHandler(this.OnDuplicateButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // importButton
            // 
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(85, 22);
            this.importButton.Text = "[main.import]";
            this.importButton.ToolTipText = "[main.tooltips.import]";
            this.importButton.Click += new System.EventHandler(this.OnImportButtonClick);
            // 
            // exportButton
            // 
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(82, 22);
            this.exportButton.Text = "[main.export]";
            this.exportButton.ToolTipText = "[main.tooltips.export]";
            this.exportButton.Click += new System.EventHandler(this.OnExportSelectedButtonClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // implementationButton
            // 
            this.implementationButton.Name = "implementationButton";
            this.implementationButton.Size = new System.Drawing.Size(95, 22);
            this.implementationButton.Text = "[mathlib.title]";
            this.implementationButton.ToolTipText = "[mathlib.tooltip]";
            this.implementationButton.ButtonClick += new System.EventHandler(this.OnImplementationButtonClick);
            // 
            // highlightDifferencesButton
            // 
            this.highlightDifferencesButton.Name = "highlightDifferencesButton";
            this.highlightDifferencesButton.Size = new System.Drawing.Size(102, 22);
            this.highlightDifferencesButton.Text = "[highlight.title]";
            this.highlightDifferencesButton.ToolTipText = "[highlight.tooltip]";
            this.highlightDifferencesButton.ButtonClick += new System.EventHandler(this.OnHighlightDifferencesButtonClick);
            // 
            // analyzeButton
            // 
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(70, 22);
            this.analyzeButton.Text = "[main.find]";
            this.analyzeButton.ToolTipText = "[main.tooltips.find]";
            this.analyzeButton.Click += new System.EventHandler(this.OnAnalyzeButtonClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutButton.Image = global::fVis.Properties.Resources.ZoomOut;
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(23, 22);
            this.zoomOutButton.Text = "Oddálit";
            this.zoomOutButton.ToolTipText = "[main.tooltips.zoom out]";
            this.zoomOutButton.Click += new System.EventHandler(this.OnZoomOutButtonClick);
            // 
            // scaleFactorBox
            // 
            this.scaleFactorBox.MaxLength = 200;
            this.scaleFactorBox.Name = "scaleFactorBox";
            this.scaleFactorBox.Size = new System.Drawing.Size(160, 23);
            this.scaleFactorBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.scaleFactorBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnScaleFactorBoxKeyDown);
            // 
            // zoomInButton
            // 
            this.zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInButton.Image = global::fVis.Properties.Resources.ZoomIn;
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(23, 20);
            this.zoomInButton.Text = "Přiblížit";
            this.zoomInButton.ToolTipText = "[main.tooltips.zoom in]";
            this.zoomInButton.Click += new System.EventHandler(this.OnZoomInButtonClick);
            // 
            // resetButton
            // 
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(101, 19);
            this.resetButton.Text = "[main.reset view]";
            this.resetButton.ToolTipText = "[main.tooltips.reset view]";
            this.resetButton.Click += new System.EventHandler(this.OnResetButtonClick);
            // 
            // zoomToValueButton
            // 
            this.zoomToValueButton.Name = "zoomToValueButton";
            this.zoomToValueButton.Size = new System.Drawing.Size(93, 19);
            this.zoomToValueButton.Text = "[main.zoom to]";
            this.zoomToValueButton.ToolTipText = "[main.tooltips.zoom to]";
            this.zoomToValueButton.Click += new System.EventHandler(this.OnZoomToValueButtonClick);
            // 
            // helpDropDown
            // 
            this.helpDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.langMenuItem,
            this.toolStripSeparator5,
            this.aboutMenuItem});
            this.helpDropDown.Image = global::fVis.Properties.Resources.Help;
            this.helpDropDown.Name = "helpDropDown";
            this.helpDropDown.Size = new System.Drawing.Size(29, 20);
            this.helpDropDown.Text = "[main.help]";
            // 
            // langMenuItem
            // 
            this.langMenuItem.Name = "langMenuItem";
            this.langMenuItem.Size = new System.Drawing.Size(180, 22);
            this.langMenuItem.Text = "[main.language]";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(177, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutMenuItem.Text = "[main.about]";
            this.aboutMenuItem.Click += new System.EventHandler(this.OnAboutMenuItemClick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 493);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(340, 260);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graph graph;
        private System.Windows.Forms.SplitContainer splitContainer;
        private ListView listView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addButton;
        private System.Windows.Forms.ToolStripButton removeButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSplitButton implementationButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton zoomOutButton;
        private System.Windows.Forms.ToolStripTextBox scaleFactorBox;
        private System.Windows.Forms.ToolStripButton zoomInButton;
        private System.Windows.Forms.ToolStripButton resetButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton importButton;
        private System.Windows.Forms.ToolStripButton exportButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton duplicateButton;
        private System.Windows.Forms.ToolStripSplitButton highlightDifferencesButton;
        private System.Windows.Forms.ToolStripButton analyzeButton;
        private System.Windows.Forms.ToolStripButton zoomToValueButton;
        private System.Windows.Forms.ToolStripDropDownButton helpDropDown;
        private System.Windows.Forms.ToolStripMenuItem langMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
    }
}

