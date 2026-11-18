using System.Reflection;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Size = System.Drawing.Size;

namespace MGQP_Script;

public class ImagesDB
{
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    public string WindowTitle;
    public readonly Mat Left_Battle_Panel_Template;
    public readonly Mat Coliseum_Template;
    public readonly Mat Black_Screen_Template;
    public readonly Mat Bottom_Panel_Template;
    
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(
        IntPtr hwnd,
        uint dwFlags
    );

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetMonitorInfo(
        IntPtr hMonitor,
        ref MONITORINFO lpmi
    );
    
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }
    
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(
        string? lpClassName,
        string? lpWindowName
    );

    public ImagesDB()
    {
        Console.WriteLine("Creating ImagesDB instance.");
        WindowTitle = "Квест Монстродев: Парадокс";
        Size? resolution = GetGameMonitorResolution();
        Console.WriteLine($"{resolution.Value.Width}x{resolution.Value.Height}");
        if (resolution.Value.Width == 1920 && resolution.Value.Height == 1080)
        {
            Console.WriteLine("1920x1080");
            Left_Battle_Panel_Template = LoadEmbeddedMat("MGQP_Script.imgs._1920x1080.Battle_Left_Panel_1920x1080.png");
            Coliseum_Template = LoadEmbeddedMat("MGQP_Script.imgs._1920x1080.Coliseum_Image_1920x1080.png");
            Black_Screen_Template = LoadEmbeddedMat("MGQP_Script.imgs._1920x1080.Black_Screen_1920x1080.png");
            Bottom_Panel_Template = LoadEmbeddedMat("MGQP_Script.imgs._1920x1080.Bottom_Panel_1920x1080.png");
        }
        else if (resolution.Value.Width == 3840 && resolution.Value.Height == 2160)
        {
            Console.WriteLine("2560x1440");
            Left_Battle_Panel_Template = LoadEmbeddedMat("MGQP_Script.imgs._2560x1440.Battle_Left_Panel_2560x1440.png");
            Coliseum_Template = LoadEmbeddedMat("MGQP_Script.imgs._2560x1440.Coliseum_Image_2560x1440.png");
            Black_Screen_Template = LoadEmbeddedMat("MGQP_Script.imgs._2560x1440.Black_Screen_2560x1440.png");
            Bottom_Panel_Template = LoadEmbeddedMat("MGQP_Script.imgs._2560x1440.Bottom_Panel_2560x1440.png");
        }
    }

    public Size? GetGameMonitorResolution()
    {
        IntPtr hwnd = FindWindow(null, WindowTitle);

        if (hwnd == IntPtr.Zero)
        {
            Console.WriteLine("Game window not found.");
            return null;
        }

        IntPtr monitor = MonitorFromWindow(
            hwnd,
            MONITOR_DEFAULTTONEAREST
        );

        if (monitor == IntPtr.Zero)
            return null;

        MONITORINFO info = new MONITORINFO
        {
            cbSize = Marshal.SizeOf<MONITORINFO>()
        };

        if (!GetMonitorInfo(monitor, ref info))
            return null;

        int width =
            info.rcMonitor.Right - info.rcMonitor.Left;

        int height =
            info.rcMonitor.Bottom - info.rcMonitor.Top;

        return new Size(width, height);
    }
    
    public Mat LoadEmbeddedMat(string resourceName, ImreadModes flags = ImreadModes.Color)
    {
        // Берем сборку главного запускаемого проекта (MGQP_Script), а не текущую библиотеку
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"Ресурс '{resourceName}' не найден в сборке {assembly.FullName}");
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
    
        return Cv2.ImDecode(memoryStream.ToArray(), flags);
    }

}