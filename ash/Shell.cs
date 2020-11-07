using Microsoft.VisualBasic.CompilerServices;
using ModuleAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace ash
{
    public class Shell
    {

        public List<Module> modules = new List<Module>();

        public Shell(string initpath)
        {
            Directory.SetCurrentDirectory(initpath);
            LoadModules();
        }

        public void LoadModules()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string modpath = Path.Combine(appdata, "ash", "mod");

            if (Directory.Exists(modpath))
            {
                List<string> conflicts = new List<string>();
                conflicts.AddRange(Directory.GetFiles(modpath, "*.dll", SearchOption.AllDirectories));
                conflicts.AddRange(Directory.GetFiles(modpath, "*.mod", SearchOption.AllDirectories));
                foreach (string file in conflicts)
                {
                    if (file.EndsWith("ModuleAPI.dll"))
                        continue;
                    List<ModuleCommand> commands = new List<ModuleCommand>();
                    Assembly lib = Assembly.LoadFrom(file);
                    foreach (Type t in lib.GetTypes())
                    {
                        bool includeAll = false;
                        if (t.IsDefined(typeof(IncludedInModule), false))
                            includeAll = true;
                        Console.WriteLine($"Found class '{t.FullName}' in '{lib.GetName().Name}' at '{file}'.");
                        foreach (MethodInfo i in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            if ((i.GetCustomAttribute(typeof(IncludedInModule), false) != null) || includeAll)
                            {
                                Console.WriteLine($"Found valid method '{i.Name}' in previous class.");
                                ModuleCommand item = new ModuleCommand(i, this, i.Name, t);
                                if (!commands.Contains(item))
                                {
                                    commands.Add(item);
                                }
                                else
                                    Console.WriteLine($"Warning: Function already exists. Keeping first version.");
                            }

                        }
                    }
                    modules.Add(new Module(commands, lib.GetName().Name));
                }
            }
        }

        public void SwitchColor(bool Backwards = false)
        {
            if (Backwards)
            {
                if (Console.ForegroundColor < ConsoleColor.White)
                    Console.ForegroundColor++;
                else
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
            }
            else
            {
                if (Console.ForegroundColor > ConsoleColor.Black)
                    Console.ForegroundColor--;
                else
                    Console.ForegroundColor = ConsoleColor.White;
            }
        }

        ConsoleKeyInfo Creading = new ConsoleKeyInfo();
        char reading => Creading.KeyChar;

        private string ReadLine()
        {
            string retString = "";

            int curIndex = 0;
            do
            {
                ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                // handle Esc
                if (readKeyResult.KeyChar == ':')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                // handle Enter
                if (readKeyResult.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    goto end;
                }

                if (readKeyResult.Key == ConsoleKey.Spacebar)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                // handle backspace
                if (readKeyResult.Key == ConsoleKey.Backspace)
                {
                    if (curIndex > 0)
                    {
                        retString = retString.Remove(retString.Length - 1);
                        Console.Write(readKeyResult.KeyChar);
                        Console.Write(' ');
                        Console.Write(readKeyResult.KeyChar);
                        curIndex--;
                    }
                }
                else
                // handle all other keypresses
                {
                    retString += readKeyResult.KeyChar;
                    Console.Write(readKeyResult.KeyChar);
                    curIndex++;
                }
            }
            while (true);

        end:
            Console.ResetColor();
            return retString;
        }

        public void ProcessInputLine()
        {
            var cmd = Environment.GetEnvironmentVariable("ASH_COMMAND");
            if (!string.IsNullOrWhiteSpace(cmd))
                switch (cmd)
                {
                    case "0":
                    case "":
                        break;
                    case "1":
                        modules.Clear();
                        LoadModules();
                        break;
                    case "2":
                        var val = Environment.GetEnvironmentVariable("ASH_COMMAND_PTR");
                        Console.WriteLine(val);
                        break;
                    default:
                        Error($"INTERNAL ERROR: Unknown command given to Ash '{cmd}'.");
                        break;
                }

            Environment.SetEnvironmentVariable("ASH_COMMAND", "", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("ASH_COMMAND_PTR", "", EnvironmentVariableTarget.User);

            Console.Write($"{Environment.MachineName}:{Environment.UserName}~$");
            string s = ReadLine();

            s = s.Trim();

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            s = regex.Replace(s, " ");

            string[] argv = s.Split(' ');

            string first = argv[0];

            if (first.ToLower() == "cd")
            {
                if (argv.Length == 1)
                    Error("Please specify a path!");
                else
                {
                    Directory.SetCurrentDirectory(argv[1]);
                }
            }

            // this means the following command is a Module.
            if (first.Contains(':'))
            {
                if (first.StartsWith(':'))
                {
                    foreach (Module m in modules)
                    {
                        if (m.Name == "Base")
                            m.ExecuteCommand(first.Remove(0, 1), argv[1..]);
                    }
                }
                string[] mod = first.Split(':', 2);
                foreach (Module m in modules)
                {
                    if (m.Name == mod[0].Trim())
                        m.ExecuteCommand(mod[1], argv[1..]);
                }
            }

        }

        public void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Error: " + error);
            Console.ResetColor();
        }
    }
}
