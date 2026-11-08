using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using OpenCvSharp;
using OpenCvSharp.Extensions;

using Point = OpenCvSharp.Point;
using Size = System.Drawing.Size;

namespace MGQP_Script;

public class ImageManager
{
    private readonly string windowTitle;
    private readonly string left_Battle_Panel;
    private readonly string right_Battle_Panel;

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 =
        new IntPtr(-4);

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    public ImageManager()
    {
        windowTitle = "Квест Монстродев: Парадокс";
        left_Battle_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\Battle_Left_Panel.png";
        right_Battle_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\Right_Battle_Panel.png";
    }

    static ImageManager()
    {
        SetProcessDpiAwarenessContext(
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2
        );
    }

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

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public Mat? GetImage()
    {
        IntPtr hwnd = FindWindow(
            null,
            windowTitle
        );

        if (hwnd == IntPtr.Zero)
        {
            Console.WriteLine("Window not found.");
            return null;
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

            return null;
        }

        int width =
            rect.Right - rect.Left;

        int height =
            rect.Bottom - rect.Top;

        Console.WriteLine(
            $"Window: X={rect.Left}, Y={rect.Top}, " +
            $"Width={width}, Height={height}"
        );

        using Bitmap bitmap = new Bitmap(
            width,
            height,
            PixelFormat.Format24bppRgb
        );

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

        Mat image =
            BitmapConverter.ToMat(bitmap);

        Console.WriteLine(
            $"Captured: {image.Width}x{image.Height}"
        );

        return image;
    }

    private static Point? FindImage(Mat screenshot, string templatePath, double threshold = 0.90)
    {
        using Mat template = Cv2.ImRead(
            templatePath,
            ImreadModes.Color
        );

        if (template.Empty())
        {
            Console.WriteLine(
                $"Couldn't load template: {templatePath}"
            );

            return null;
        }

        if (
            template.Width > screenshot.Width ||
            template.Height > screenshot.Height
        )
        {
            Console.WriteLine(
                "Template is larger than screenshot."
            );

            return null;
        }

        using Mat result = new Mat();

        Cv2.MatchTemplate(
            screenshot,
            template,
            result,
            TemplateMatchModes.CCoeffNormed
        );

        Cv2.MinMaxLoc(
            result,
            out _,
            out double maxValue,
            out _,
            out Point maxLocation
        );

        Console.WriteLine(
            $"Confidence: {maxValue:F3}"
        );

        if (maxValue >= threshold)
            return maxLocation;

        return null;
    }

    public bool CheckImage(string templatePath, Mat screenshot)
    {
        Point? position =
            FindImage(
                screenshot,
                templatePath
            );

        if (position.HasValue)
        {
            Console.WriteLine(
                $"Found: " +
                $"X={position.Value.X}, " +
                $"Y={position.Value.Y}"
            );
            return true;
        }
        else
        {
            Console.WriteLine("Not found");
            return false;
        }
    }

    public bool IsLeftBattlePanel()
    {
        using Mat? screenshot = GetImage();

        if (screenshot is null)
        {
            return false;
        }

        return CheckImage(left_Battle_Panel, screenshot);
    }

    public bool IsRightBattlePanel()
    {
        using Mat? screenshot = GetImage();

        if (screenshot is null)
        {
            return false;
        }

        return CheckImage(right_Battle_Panel, screenshot);
    }
}