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
        private Timer animationTimer;
        private int animationValue;

        public AboutDialog()
        {
            InitializeComponent();

            TxWinForms.Bind(this);

            DoubleBuffered = true;
            BackColor = SystemColors.Window;

            Font = SystemFonts.MenuFont;
            mainInstructionFont = UI.GetMainInstructionFont();
            subtitleFont = new Font(Font.FontFamily, Font.Size * 0.9f, Font.Style | FontStyle.Italic, Font.Unit);

            animationTimer = new Timer();
            animationTimer.Interval = 24;
            animationTimer.Tick += OnAnimationTimer;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            animationTimer.Start();
        }

        private void OnAnimationTimer(object sender, EventArgs e)
        {
            animationValue += 20;

            if (animationValue >= 255) {
                animationValue = 255;
                animationTimer.Stop();
            }

            Invalidate(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Size clientSize = ClientSize;

            UI.PaintDirtGradient(e.Graphics, 1, 1, clientSize.Width - 1, 360, 360, 255);

            int y = 9;

            Color back = SystemColors.Window;
            Color fore = SystemColors.WindowText;
            Color foreAnimated = Color.FromArgb(
                back.R + (fore.R - back.R) * animationValue / 255,
                back.G + (fore.G - back.G) * animationValue / 255,
                back.B + (fore.B - back.B) * animationValue / 255
            );

            using (GdiGraphics g = GdiGraphics.FromGraphics(e.Graphics)) {
                y += DrawString(g, App.AssemblyTitle, mainInstructionFont, foreAnimated, new Point(23, y)) + 4;

                y += DrawString(g, Tx.T("main.about.description"), subtitleFont, foreAnimated, new Point(23, y)) + 18;

                y += DrawString(g, App.AssemblyCopyright, Font, fore, new Point(23, y)) + 3;
                y += DrawString(g, "This project is licensed under the terms of the GNU General Public License v3.0.", Font, fore, new Point(23, y)) + 10;
            }

            Point linkPos = new Point(21, y);
            if (linkLabel.Location != linkPos) {
                linkLabel.Location = linkPos;
            }
        }

        private void OnLinkLabelClick(object sender, EventArgs e)
        {
            try {
                Process.Start("https://github.com/deathkiller/fvis");
            } catch {
                // It's not critical
            }
        }

        private static int DrawString(GdiGraphics g, string text, Font font, Color color, Point location)
        {
            g.DrawString(text, font, color, location);

            int charFit, charFitWidth;
            return g.MeasureString(text, font, int.MaxValue, out charFit, out charFitWidth).Height;
        }
    }
}