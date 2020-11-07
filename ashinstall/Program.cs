using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace ashinstall
{
    class Program
    {
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                Console.WriteLine($"INFO: Copying file {file.FullName} to {tempPath}...");
                try { file.CopyTo(tempPath, false); }
                catch { Console.WriteLine($"INFO: File '{tempPath}' already exists."); }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        static void Main(string[] args)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Console.WriteLine("Welcome to the ash installer.");
            Console.WriteLine("The ash installer will deploy itself in the %AppData%/ash/rec/ or $HOME/.config/ash/rec/ folder.");
            Console.WriteLine("Press 'y' to continue the installer.");

            bool alreadyExists = false;

            if (File.Exists(Path.Combine(appdata, "ash", "rec", "ashinstall.exe")))
            {
                Console.WriteLine("NOTE: The installer seems to be already deployed. Press 'i' to skip installer deployment.");
            }

            if (!(Console.ReadKey(true).Key == ConsoleKey.Y))
            {
                Console.WriteLine("Installer canceled.");
                return;
            }

            Directory.CreateDirectory(appdata + Path.DirectorySeparatorChar + "ash" + Path.DirectorySeparatorChar + "rec");

            Console.WriteLine("Starting deployment of the installer...");
            DirectoryCopy(".", Path.Combine(appdata, "ash", "rec"), true);
            Console.WriteLine("Installer deployed. The installer is deployed to make sure that in case of a problem with ash, you can re-install it without another download..");

        restartselection:

            Console.Clear();
            Console.WriteLine("Please select a mirror to download ash from. Press the number associated on the keyboard.");
            Console.WriteLine("1.\thttps://www.github.com/EscapeCharacter-dev/ash/release/");
            Console.WriteLine("2.\tManual mirror");

            string mirror = "";
            string host = "";

            ConsoleKeyInfo k = Console.ReadKey(true);

            Console.WriteLine($"Selected option {k.KeyChar}.");

            if (k.Key == ConsoleKey.D1 || k.Key == ConsoleKey.NumPad1)
            {
                mirror = "https://www.github.com/EscapeCharacter-dev/ash/release/";
            }
            else
            if (k.Key == ConsoleKey.D2 || k.Key == ConsoleKey.NumPad2)
            {
                Console.Write('>');
                mirror = Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"Invalid option {k.KeyChar}!");
                goto restartselection;
            }

            if (!mirror.EndsWith('/'))
            {
                mirror += '/';
            }

            host = mirror.Substring(0, mirror.IndexOf('/', 8));

            Ping p = new Ping();
            PingReply reply;
            Console.WriteLine($"Host {host}");
            try
            {
                reply = p.Send("192.168.2.1", 10000);
                if (reply != null) Console.WriteLine("Catched reply.");
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Host {host} doesn't reply.");
                Console.Error.WriteLine($"Trying to reach host again...");
                Console.ResetColor();
                try
                {
                    reply = p.Send(host, 10000);
                    Console.WriteLine("Received reply. Installation will continue.");
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Host is unreachable!");
                    Console.ResetColor();
                    Environment.Exit(2);
                }
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(mirror + "ash.zip", Path.Combine(appdata, "ash", "app", "ash.zip"));
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.ResetColor();
                Console.WriteLine($"ERROR: Unable to download from mirror {mirror}. Installation will exit.");
                Environment.Exit(1);
            }
        }
    }
}
