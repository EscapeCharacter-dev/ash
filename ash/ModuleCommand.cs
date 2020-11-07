using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ash
{
    public class ModuleCommand
    {
        public ModuleCommand(MethodInfo func,
            Shell shell, string name, Type t)
        {
            Shell = shell;
            Function = func;
            Name = name;
        }

        /// <summary>
        /// Executes Module:Command.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns the result of the command.</returns>
        public int Execute(string[] args)
        {
            int result = -1;
            try
            {
                object[] p = new object[1];
                p[0] = args;
                result = (int)Function.Invoke(Function.GetType(), p);
            }
            catch (Exception e)
            {
                Shell.Error($"Invalid module function (or module)!");
                throw e;
            }
            return result;
        }

        public MethodInfo Function;
        public Shell Shell;
        public string Name;
        public Type T;
    }
}
