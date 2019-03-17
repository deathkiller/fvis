//
// Based on article "Using native GDI for text rendering in C#".
// https://theartofdev.com/2013/08/12/using-native-gdi-for-text-rendering-in-c/
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using fVis.Utils;

namespace fVis.Misc
{
    [SuppressUnmanagedCodeSecurity]
    public class GdiGraphics : Disposable, IDeviceContext
    {
        public static GdiGraphics FromGraphics(Graphics g)
        {
            return new GdiGraphics(g);
        }

        private readonly Graphics g;
        private IntPtr hdc;

        private static readonly int[] charFitWidth = new int[1024];

        private static readonly Dictionary<string, Dictionary<float, Dictionary<FontStyle, IntPtr>>> fontsCache =
            new Dictionary<string, Dictionary<float, Dictionary<FontStyle, IntPtr>>>(
                StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Temporary reinitialize Graphics context
        /// </summary>
        public Graphics ManagedGraphics
        {
            get
            {
                if (g == null) {
                    throw new InvalidOperationException();
                }

                ReleaseHdc();
                return g;
            }
        }

        /// <summary>
        /// Prepares Graphics instance for context switching
        /// </summary>
        /// <param name="g"></param>
        private GdiGraphics(Graphics g)
        {
            if (g == null) {
                throw new ArgumentNullException(nameof(g));
            }

            this.g = g;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHdc();
        }

        /// <summary>
        /// Release Graphics context and initialize GDI context
        /// </summary>
        private void InitHdc()
        {
            if (g != null && hdc == IntPtr.Zero) {
                IntPtr clip = g.Clip.GetHrgn(g);

                hdc = g.GetHdc();
                SetBkMode(hdc, 1);

                SelectClipRgn(hdc, clip);

                DeleteObject(clip);
            }
        }

        /// <summary>
        /// Get GDI context
        /// </summary>
        /// <returns></returns>
        public IntPtr GetHdc()
        {
            InitHdc();
            return hdc;
        }

        /// <summary>
        /// Release GDI context and reinitialize Graphics context
        /// </summary>
        public void ReleaseHdc()
        {
            if (g != null && hdc != IntPtr.Zero) {
                SelectClipRgn(hdc, IntPtr.Zero);
                g.ReleaseHdc(hdc);
                hdc = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Measure size of given text
        /// </summary>
        /// <param name="text">Text to measure</param>
        /// <param name="font">Font</param>
        /// <param name="maxWidth">Max. width</param>
        /// <param name="charFit">Index of last character that fit</param>
        /// <param name="charFitWidth">Width of text that fit</param>
        /// <returns>Size of text</returns>
        public Size MeasureString(string text, Font font, int maxWidth, out int charFit, out int charFitWidth)
        {
            InitHdc();
            SetFont(font);

            Size size;
            GetTextExtentExPoint(hdc, text, text.Length, maxWidth, out charFit, GdiGraphics.charFitWidth, out size);
            charFitWidth = (charFit > 0 ? GdiGraphics.charFitWidth[charFit - 1] : 0);
            return size;
        }

        /// <summary>
        /// Draw string on specified location
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="font">Font</param>
        /// <param name="color">Color</param>
        /// <param name="point">Location</param>
        public void DrawString(string text, Font font, Color color, Point point)
        {
            InitHdc();
            SetFont(font);
            SetTextColor(hdc, (color.B << 16) | (color.G << 8) | color.R);

            TextOut(hdc, point.X, point.Y, text, text.Length);
        }

        /// <summary>
        /// Set Font as GDI font
        /// </summary>
        /// <param name="font">Font</param>
        private void SetFont(Font font)
        {
            SelectObject(hdc, GetCachedHFont(font));
        }

        /// <summary>
        /// Try to find GDI font in cache
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>Handle to GDI font</returns>
        private static IntPtr GetCachedHFont(Font font)
        {
            IntPtr hFont = IntPtr.Zero;
            Dictionary<float, Dictionary<FontStyle, IntPtr>> sizeResolve;
            if (fontsCache.TryGetValue(font.Name, out sizeResolve)) {
                Dictionary<FontStyle, IntPtr> styleResolve;
                if (sizeResolve.TryGetValue(font.Size, out styleResolve)) {
                    styleResolve.TryGetValue(font.Style, out hFont);
                } else {
                    sizeResolve[font.Size] = new Dictionary<FontStyle, IntPtr>();
                }
            } else {
                fontsCache[font.Name] = new Dictionary<float, Dictionary<FontStyle, IntPtr>>();
                fontsCache[font.Name][font.Size] = new Dictionary<FontStyle, IntPtr>();
            }

            if (hFont == IntPtr.Zero) {
                fontsCache[font.Name][font.Size][font.Style] = hFont = font.ToHfont();
            }

            return hFont;
        }

        #region Native Methods
        [DllImport("gdi32")]
        private static extern int SetBkMode(IntPtr hdc, int mode);
        [DllImport("gdi32")]
        private static extern int SetTextColor(IntPtr hdc, int color);
        [DllImport("gdi32", EntryPoint = "GetTextExtentExPointW")]
        private static extern bool GetTextExtentExPoint(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int nLength, int nMaxExtent, out int lpnFit, int[] alpDx, out Size size);
        [DllImport("gdi32", EntryPoint = "TextOutW")]
        private static extern bool TextOut(IntPtr hdc, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string str, int len);
        [DllImport("gdi32")]
        private static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

        [DllImport("gdi32", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);
        #endregion
    }
}