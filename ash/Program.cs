using System;
using System.IO;

namespace ash
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ash shell.");

            string modpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/ash/mod/";

            if (!Directory.Exists(modpath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FATAL: The module folder doesn't exist:\n (%AppData%/ash/mod/ on Microsoft Windows, $USER/.config/ash/mod/ on Linux.)");
                Console.ResetColor();
                Console.WriteLine("You should re-install ash. Press ENTER to relaunch the installer.");
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    string installpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/ash/rec/";
                    if (!Directory.Exists(installpath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FATAL: You don't have any installer of ash, you probably deleted it.");
                        Console.ResetColor();
                        Environment.Exit(0xBAD);
                    }
                }
                else
                {
                    Console.WriteLine("WARNING: Without modules, you won't be able to do much without executables.");
                }
            }

            Environment.SetEnvironmentVariable("ASH_LMOD_PATH", "");

            Shell s = new Shell(".");

            while (true)
                s.ProcessInputLine();
        }
    }
}
