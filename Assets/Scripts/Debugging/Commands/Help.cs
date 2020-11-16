using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    public class Help : ICommand
    {
        public Help() { }

        public string Command => "help";

        string ICommand.Help =>
            @"Prints helps.
Usage: help [command]

command is the name of the command.
If [command] is not specified, it will list all existing commands.";

        public string Execute(params string[] parameters)
        {
            StringBuilder builder = new StringBuilder("Existing commands...");

            if (parameters.Length == 0) // help
            {
                var commands = CommandList.Commands;
                foreach(var command in commands)
                {
                    builder.Append($"\n{command}");
                }
                builder.AppendLine();

                return builder.ToString();
            }
            else if (parameters.Length == 1) // help [command]
            {
                return CommandList.GetHelp(parameters[0]);
            }
            else
            {
                return "Error: Invalid parameters. Usage: help [command]";
            }
        }
    }

}