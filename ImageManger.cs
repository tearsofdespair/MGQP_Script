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
    private ImagesDB _imagesDb;

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 =
        new IntPtr(-4);

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

    public ImageManager()
    {
        _imagesDb =  new ImagesDB();
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
            _imagesDb.WindowTitle
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

        return image;
    }

    private static Point? FindImage(
        Mat screenshot,
        string templatePath,
        double threshold = 0.9
    )
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

        using Mat resizedTemplate = new Mat();

        Mat searchTemplate = template;

        // Scale down if the template is larger than the screenshot.
        if (
            template.Width > screenshot.Width ||
            template.Height > screenshot.Height
        )
        {
            double scale = Math.Min(
                (double)screenshot.Width / template.Width,
                (double)screenshot.Height / template.Height
            );

            int newWidth = Math.Max(
                1,
                (int)Math.Round(template.Width * scale)
            );

            int newHeight = Math.Max(
                1,
                (int)Math.Round(template.Height * scale)
            );

            Cv2.Resize(
                template,
                resizedTemplate,
                new OpenCvSharp.Size(newWidth, newHeight),
                0,
                0,
                InterpolationFlags.Area
            );

            searchTemplate = resizedTemplate;

            Console.WriteLine(
                $"Template resized: {template.Width}x{template.Height} " +
                $"-> {newWidth}x{newHeight} (Scale: {scale:F3})"
            );
        }

        using Mat result = new Mat();

        Cv2.MatchTemplate(
            screenshot,
            searchTemplate,
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

        Console.WriteLine($"Confidence: {maxValue:F3}");

        if (maxValue >= threshold)
            return maxLocation;

        return null;
    }

    public bool CheckImage(string templatePath, Mat screenshot, double threshold = 0.9)
    {
        Point? position =
            FindImage(
                screenshot,
                templatePath
            );

        if (position.HasValue)
        {
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

        Console.WriteLine("Left Battle Panel");
        return CheckImage(_imagesDb.Left_Battle_Panel, screenshot);
    }

    public bool IsBlackScreen()
    {
        using Mat? screenshot = GetImage();

        if (screenshot is null)
        {
            return false;
        }

        Console.WriteLine("Black Screen");
        return CheckImage(_imagesDb.Black_Screen, screenshot);
    }

    public bool isBottomPanel()
    {
        using Mat? screenshot = GetImage();

        if (screenshot is null)
        {
            return false;
        }

        Console.WriteLine("Bottom Battle Panel");
        return CheckImage(_imagesDb.Bottom_Panel, screenshot, 0.75);
    }
    
    public bool AreImagesSimilar(Mat img1, Mat img2, double threshold = 0.95)
    {
        if (img1.Size() != img2.Size() || img1.Type() != img2.Type())
            return false;

        using var result = new Mat();
        
        Cv2.MatchTemplate(img1, img2, result, TemplateMatchModes.CCoeffNormed);
        
        Cv2.MinMaxLoc(result, out _, out double similarity);

        return similarity >= threshold;
    }
}