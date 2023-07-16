using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace FishBot
{
    public class KeysManager
    {
        private const int WM_ACTIVATE = 0x0006;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private const int WA_ACTIVATE = 0x0001;

        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        public enum VK : int
        {
            I = 0x49,
            H = 0x48,
            //J = 0x4A,
            Ctrl = 0x11,
            F4 = 0x73,

            Number0 = 0x30
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys key);

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool enable);

        public static void EnableInput(IntPtr hwnd, bool state = true) => EnableWindow(hwnd, state);

        public static bool IsKeyPressed(System.Windows.Forms.Keys key)
        {
            if (GetAsyncKeyState(key) != 0)
                return true;
            else return false;
        }

        public static void SendKey(IntPtr hwnd, VK key, int postTimeout = 0)
        {
            PostMessage(hwnd, WM_ACTIVATE, WA_ACTIVATE, 0);

            PostMessage(hwnd, WM_KEYDOWN, (int)key, 0);

            Thread.Sleep(postTimeout);
        }

        public static void SendKey(IntPtr hwnd, int key, int postTimeout = 0)
        {
            PostMessage(hwnd, WM_ACTIVATE, WA_ACTIVATE, 0);

            PostMessage(hwnd, WM_KEYDOWN, key, 0);

            Thread.Sleep(postTimeout);
        }

        public static void SendLeftClick(IntPtr hwnd, Point point)
        {
            SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVATE, 0);

            int MakeLParam(int x, int y) => ((y << 16) | (x & 0xFFFF));

            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, pointPtr);
            SendMessage(hwnd, WM_LBUTTONDOWN, 0, pointPtr);
            SendMessage(hwnd, WM_LBUTTONUP, 0, pointPtr);
        }

        public static void SendRightClick(IntPtr hwnd, Point point)
        {
            SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVATE, 0);

            int MakeLParam(int x, int y) => ((y << 16) | (x & 0xFFFF));

            var pointPtr = MakeLParam(point.X, point.Y);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, pointPtr);
            SendMessage(hwnd, WM_RBUTTONDOWN, 0, pointPtr);
            SendMessage(hwnd, WM_RBUTTONUP, 0, pointPtr);
        }

        public static void DragMouse(IntPtr hwnd, Point start, Point end, bool ctrlMod = false)
        {
            SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVATE, 0);

            int MakeLParam(int x, int y) => ((y << 16) | (x & 0xFFFF));

            var startPtr = MakeLParam(start.X, start.Y);
            var endPtr = MakeLParam(end.X, end.Y);

            if (ctrlMod)
                SendKey(hwnd, VK.Ctrl);

            SendMessage(hwnd, WM_MOUSEMOVE, 0, startPtr);
            SendMessage(hwnd, WM_LBUTTONDOWN, 0, startPtr);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, endPtr);
            SendMessage(hwnd, WM_LBUTTONUP, 0, endPtr);

            Thread.Sleep(500);
        }
    }
}