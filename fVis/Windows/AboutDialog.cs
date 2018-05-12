using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using fVis.Misc;
using Unclassified.TxLib;

namespace fVis.Windows
{
    public partial class AboutDialog : Form
    {
        private Font mainInstructionFont;
        private Font subtitleFont;

        public AboutDialog()
        {
            InitializeComponent();

            TxWinForms.Bind(this);

            DoubleBuffered = true;
            BackColor = SystemColors.Window;

            Font = SystemFonts.MenuFont;
            mainInstructionFont = UI.GetMainInstructionFont();
            subtitleFont = new Font(Font.FontFamily, Font.Size * 0.9f, Font.Style | FontStyle.Italic, Font.Unit);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Size clientSize = ClientSize;

            UI.PaintDirtGradient(e.Graphics, 1, 1, clientSize.Width - 1, 360, 360);

            int y = 9;

            using (GdiGraphics g = GdiGraphics.FromGraphics(e.Graphics)) {
                y += DrawString(g, App.AssemblyTitle, mainInstructionFont, new Point(23, y)) + 4;

                y += DrawString(g, Tx.T("main.about.description"), subtitleFont, new Point(23, y)) + 18;

                y += DrawString(g, App.AssemblyCopyright, Font, new Point(23, y)) + 3;
                y += DrawString(g, "This project is licensed under the terms of the GNU General Public License v3.0.", Font, new Point(23, y)) + 10;
            }

            Point linkPos = new Point(21, y);
            if (linkLabel.Location != linkPos) {
                linkLabel.Location = linkPos;
            }
        }

        private static int DrawString(GdiGraphics g, string text, Font font, Point location)
        {
            g.DrawString(text, font, Color.Black, location);

            int charFit, charFitWidth;
            return g.MeasureString(text, font, int.MaxValue, out charFit, out charFitWidth).Height;
        }

        private void OnLinkLabelClick(object sender, EventArgs e)
        {
            try {
                Process.Start("https://github.com/deathkiller/fvis");
            } catch {
                // Nothing to do...
            }
        }
    }
}