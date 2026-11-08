using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = System.Drawing.Size;

class Program
{
    // Windows 10+
    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 =
        new IntPtr(-4);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(
        IntPtr dpiContext
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(
        string? lpClassName,
        string? lpWindowName
    );

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        out RECT pvAttribute,
        int cbAttribute
    );

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    static void Main()
    {
        SetProcessDpiAwarenessContext(
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2
        );

        string windowTitle = "Квест Монстродев: Парадокс";

        IntPtr hwnd = FindWindow(null, windowTitle);

        if (hwnd == IntPtr.Zero)
        {
            Console.WriteLine("Window not found.");
            return;
        }

        int result = DwmGetWindowAttribute(
            hwnd,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            out RECT rect,
            Marshal.SizeOf<RECT>()
        );

        if (result != 0)
        {
            Console.WriteLine(
                $"Couldn't get window bounds. Error: {result}"
            );

            return;
        }

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Console.WriteLine(
            $"Window: X={rect.Left}, Y={rect.Top}, " +
            $"Width={width}, Height={height}"
        );

        using Bitmap bitmap =
            new Bitmap(width, height);

        using (Graphics graphics =
               Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(
                rect.Left,
                rect.Top,
                0,
                0,
                new Size(width, height),
                CopyPixelOperation.SourceCopy
            );
        }

        using Mat image =
            BitmapConverter.ToMat(bitmap);

        Cv2.ImWrite(
            "D:\\myprogs\\C#\\MGQP_Script\\MGQP_Script\\screenshot.png",
            image
        );

        Console.WriteLine(
            $"Saved: {image.Width}x{image.Height}"
        );
    }
}