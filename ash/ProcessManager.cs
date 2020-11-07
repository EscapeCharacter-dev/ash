using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ash
{
    public class ProcessManager
    {
        public ProcessManager(string processName, string []args)
        {
            ProcessName = processName;
            Args = args;
        }

        public void Init()
        {
            p = new Process();
            p.StartInfo = new ProcessStartInfo(ProcessName, Args.Flat());
        }

        public void Start()
        {
            p.Start();
        }

        public void Stop()
        {
            p.Kill();
        }

        public string ProcessName { get; }
        public string[] Args { get; }
        public Process p { get; private set; }
    }

    public static class _EXT__
    {
        public static string Flat(this string[] strarray)
        {
            string result = "";
            foreach (string s in strarray)
                result += " " + s;
            return result;
        }
    }
}
