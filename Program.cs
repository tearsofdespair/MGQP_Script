using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MGQP_Script;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using WindowsInput.Native;
using Size = System.Drawing.Size;


class Program
{
    static void Main()
    {
        /*FarmManager manager = new FarmManager();
        Thread.Sleep(1000);
        manager.FarmEntities(8);*/
        ProgramInterface interf = new ProgramInterface();
        interf.Run();
    }
}