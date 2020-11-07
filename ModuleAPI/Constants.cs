using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ModuleAPI
{
    public enum AshCommands
    {
        Null,
        ReloadModules,
        Error,
    }

    public static class AshUtils
    {
        public static void SendCommand(AshCommands command, object ptr)
        {
            Environment.SetEnvironmentVariable("ASH_COMMAND", $"{(int)command}", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("ASH_COMMAND_PTR", ptr.ToString(), EnvironmentVariableTarget.User);
        }
    }
}
