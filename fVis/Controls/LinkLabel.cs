using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace Death.Controls
{
    internal class LinkLabel : Control
    {
        private Font hoverFont;
        private Rectangle textRect;
        private bool isHovered;
        private bool keyAlreadyProcessed;
        private int maxWidth;

        public bool HoverUnderline { get; set; }

        public Color RegularColor { get; set; }
        public Color HoverColor { get; set; }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text == value)
                    return;

                base.Text = value;
                RefreshTextRect();
                Invalidate();
            }
        }

        public int MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                maxWidth = value;
                RefreshTextRect();
                Invalidate();
            }
        }

        public LinkLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.FixedHeight
                | ControlStyles.FixedWidth, true);

            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);

            hoverFont = new Font(Font, Font.Style | FontStyle.Underline);

            HoverUnderline = true;

            RegularColor = SystemColors.HotTrack;
            HoverColor = SystemColors.Highlight;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                Focus();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.None) {
                if (!ClientRectangle.Contains(e.Location)) {
                    if (isHovered) {
                        isHovered = false;
                        Invalidate();
                    }
                } else if (!isHovered) {
                    isHovered = true;
                    Invalidate();
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            keyAlreadyProcessed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!keyAlreadyProcessed && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)) {
                keyAlreadyProcessed = true;
                OnClick(e);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyAlreadyProcessed = false;

            base.OnKeyUp(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isHovered && e.Clicks == 1 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)) {
                OnClick(e);
            }

            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics, Text,
                isHovered && HoverUnderline ? hoverFont : Font,
                textRect,
                Enabled ? (isHovered ? HoverColor : RegularColor) : Color.FromArgb(0x7E, 0x85, 0x9C),
                (ShowKeyboardCues ? TextFormatFlags.Default : TextFormatFlags.HidePrefix) | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.WordEllipsis);

            if (Focused && ShowFocusCues) {
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            hoverFont = new Font(Font, Font.Style | FontStyle.Underline);
            RefreshTextRect();

            base.OnFontChanged(e);
        }

        private void RefreshTextRect()
        {
            textRect = new Rectangle(Point.Empty, TextRenderer.MeasureText(Text, Font, Size, TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix));
            int width = textRect.Width + 1,
                height = textRect.Height + 1;

            if (maxWidth > 0) {
                int limitedWidth = Math.Min(maxWidth, width);
                textRect.Width -= (width - limitedWidth);
                width = limitedWidth;
            }

            Size = new Size(width, height);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x20) { // WM_SETCURSOR
                SetCursor(LoadCursor(IntPtr.Zero, 0x7F89)); // IDC_HAND

                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        #region Native Methods
        [DllImport("user32"), SuppressUnmanagedCodeSecurity]
        private static extern int LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32"), SuppressUnmanagedCodeSecurity]
        private static extern int SetCursor(int hCursor);
        #endregion
    }
}