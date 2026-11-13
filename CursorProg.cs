using System.Runtime.InteropServices;
using OpenCvSharp;

namespace MGQP_Script;

public static class CursorProg
{
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    public static void ShowCursor()
    {
        Console.WriteLine("Tracking cursor position and color. Press Ctrl+C to exit.\n");

        while (true)
        {
            if (GetCursorPos(out POINT cursorPos))
            {
                // Capture the pixel color at the cursor position
                IntPtr hdc = GetDC(IntPtr.Zero);
                uint pixel = GetPixel(hdc, cursorPos.X, cursorPos.Y);
                ReleaseDC(IntPtr.Zero, hdc);

                // Extract RGB values from the 0x00BBGGRR format
                byte r = (byte)(pixel & 0x000000FF);
                byte g = (byte)((pixel & 0x0000FF00) >> 8);
                byte b = (byte)((pixel & 0x00FF0000) >> 16);

                // Store inside an OpenCV Vec3b (OpenCV uses BGR order)
                Vec3b bgrColor = new Vec3b(b, g, r);

                // Overwrite the current console line with updated data
                Console.Write($"\rPos: ({cursorPos.X,4}, {cursorPos.Y,4}) | RGB: ({r,3}, {g,3}, {b,3}) | OpenCV BGR: ({bgrColor.Item0,3}, {bgrColor.Item1,3}, {bgrColor.Item2,3})   ");
            }

            Thread.Sleep(50); // Update 20 times per second
        }
    }
}