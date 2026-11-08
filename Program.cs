using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MGQP_Script;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = System.Drawing.Size;

class Program
{
    static void Main()
    {
        ImageManager imageManager =
            new ImageManager();
        
        Console.WriteLine(imageManager.IsRightBattlePanel());
    }
}