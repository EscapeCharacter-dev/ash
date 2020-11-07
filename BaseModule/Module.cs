using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ModuleAPI;

namespace BaseModule
{
    [IncludedInModule]
    public class Module
    {
        public static int Test(string []args)
        {
            Console.WriteLine("Testing...");
            Console.WriteLine("Passed parameters:");
            foreach (string arg in args)
                Console.WriteLine($"\t{arg}");
            return 0;
        }

        public static int List(string []args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Files:");
                foreach (string s in Directory.EnumerateFiles("."))
                {
                    Console.WriteLine($"{s} <FILE>");
                }
                foreach (string s in Directory.EnumerateDirectories("."))
                {
                    Console.WriteLine($"{s} <DIR>");
                }
            }

            foreach (string arg in args)
            {
                Console.WriteLine($"Files in directory {arg}:");
                foreach (string s in Directory.EnumerateFiles(arg))
                {
                    Console.WriteLine($"{s} <FILE>");
                }
                foreach (string s in Directory.EnumerateDirectories(arg))
                {
                    Console.WriteLine($"{s} <DIR>");
                }
            }

            return 0;
        }

        public static int Exit(string []args)
        {
            Environment.Exit(0);
            Console.WriteLine("Exiting...");
            return 0;
        }

        public static int WhereAmI(string[] args)
        {
            Console.WriteLine("Current path:");
            Console.WriteLine(Path.GetFullPath("."));
            return 0;
        }

        public static int InstallModule(string []args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify module paths.");
                return -1;
            }

            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    File.Copy(arg, Path.Combine(Environment.GetFolderPath
                        (Environment.SpecialFolder.ApplicationData), "ash", "mod", new FileInfo(arg).Name), true);
                }
            }

            AshUtils.SendCommand(AshCommands.ReloadModules, ""); // reloads modules

            return 0;
        }

        public static int OpenInExplorer(string []args)
        {
            //windows code
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer.exe";
                if (args.Length == 0)
                {
                    info.Arguments = ".";
                }
                else
                {
                    info.Arguments = args[0];
                }
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                Process.Start(info);
            }

            return 0;
        }

        public static int ColorProfile(string []args)
        {
            Console.WriteLine("Color profile:");
            for (int i = 0; i <= (int)ConsoleColor.White; i++)
            {
                Console.Write("  [");
                Console.BackgroundColor = (ConsoleColor)i;
                Console.ForegroundColor = ConsoleColor.White - i;
                Console.Write("ABCD");
                Console.ResetColor();
                Console.Write("]");
            }
            Console.WriteLine();
            return 0;
        }
    }
}
