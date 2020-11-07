using System;
using System.Collections.Generic;
using System.Text;

namespace ash
{
    public sealed class Module
    {
        public Module(List<ModuleCommand> commands, string name)
        {
            Commands = commands;
            Name = name;
        }

        public int ExecuteCommand(string commandName, string[] args)
        {
            foreach (ModuleCommand c in Commands)
            {
                if (c.Name == commandName)
                {
                    return c.Execute(args);
                }
            }
            return -2;
        }

        public List<ModuleCommand> Commands { get; }
        public string Name { get; }
    }
}
