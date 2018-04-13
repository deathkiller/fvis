using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace fVis.Misc
{
    public class UI
    {
        /// <summary>
        /// Enables close button of specified window
        /// </summary>
        /// <param name="window">Window</param>
        /// <param name="enabled">True to enable; otherwise disable</param>
        public static void EnableCloseBox(IWin32Window window, bool enabled)
        {
            IntPtr hWnd = window.Handle;
            IntPtr hMenu = GetSystemMenu(hWnd, false);
            if (hMenu == IntPtr.Zero) {
                throw new InvalidOperationException("The Window must be shown before changing menu");
            }

            EnableMenuItem(hMenu, 0xf060 /*SC_CLOSE*/, (uint)(enabled ? 0 : 1));
            if (!DrawMenuBar(hWnd)) {
                throw new InvalidOperationException("The Close menu does not exist");
            }

            GC.KeepAlive(window);
        }

        #region Native Methods
        [DllImport("user32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);
        [DllImport("user32")]
        private static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32", SetLastError = true)]
        private static extern bool DrawMenuBar(IntPtr hWnd);
        #endregion
    }
}