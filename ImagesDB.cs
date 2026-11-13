using System.Drawing;
using System.Runtime.InteropServices;

namespace MGQP_Script;

public class ImagesDB
{
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    public string WindowTitle;
    public readonly string Left_Battle_Panel;
    public readonly string Coliseum_Floor;
    public readonly string Black_Screen;
    public readonly string Bottom_Panel;
    
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
            Left_Battle_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\1920x1080\Battle_Left_Panel.png";
            Coliseum_Floor = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\1920x1080\coliseum_image.png";
            Black_Screen = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\1920x1080\black_screen.png";
            Bottom_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\1920x1080\Bottom_Panel.png";
        }
        else if (resolution.Value.Width == 3840 && resolution.Value.Height == 2160)
        {
            Console.WriteLine("2560x1440");
            Left_Battle_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\2560x1440\Battle_Left_Panel.png";
            Coliseum_Floor = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\2560x1440\coliseum_image.png";
            Black_Screen = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\2560x1440\black_screen.png";
            Bottom_Panel = @"D:\myprogs\C#\MGQP_Script\MGQP_Script\imgs\2560x1440\Bottom_Panel.png";
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

}