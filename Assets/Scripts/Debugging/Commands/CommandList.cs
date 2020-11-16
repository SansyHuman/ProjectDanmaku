using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;

namespace SansyHuman.Debugging.Commands
{
    public class CommandList : UDESingleton<CommandList>
    {
        [SerializeField]
        [Tooltip("Prefabs that have script that inherits CommandBehaviour")]
        private List<CommandBehaviour> commands;

        private Dictionary<string, ICommand> commandList;

        public static Dictionary<string, ICommand>.KeyCollection Commands => Instance.commandList.Keys;

        public static bool CommandExists(string command) => Instance.commandList.ContainsKey(command);

        protected override void Awake()
        {
            base.Awake();

            commandList = new Dictionary<string, ICommand>();

            for (int i = 0; i < commands.Count; i++)
                AddCommand(commands[i]);
        }

        private void Start()
        {
            // TODO: Add non-CommandBehaviour commands.

            AddCommand(new Help());
            AddCommand(new List());
            AddCommand(new Damage());
        }

        public static string GetHelp(string command)
        {
            var inst = Instance;

            if (!inst.commandList.ContainsKey(command))
                return "Error: Unknown command.";

            return inst.commandList[command].Help;
        }

        // If there is already same command, then it does not add it.
        private static void AddCommand(ICommand command)
        {
            var inst = Instance;

            if (inst.commandList.ContainsKey(command.Command))
                return;
            inst.commandList.Add(command.Command, command);
        }

        public static string ExecuteCommand(string command)
        {
            var inst = Instance;

            var parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parameters == null || parameters.Length == 0 || !inst.commandList.ContainsKey(parameters[0]))
                return "Error: Invalid command.";

            string[] @params = new string[parameters.Length - 1];
            for (int i = 0; i < @params.Length; i++)
                @params[i] = parameters[i + 1];

            return inst.commandList[parameters[0]].Execute(@params);
        }
    }
}