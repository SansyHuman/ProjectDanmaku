using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.Debugging.Commands
{
    [DisallowMultipleComponent]
    public class Clear : CommandBehaviour
    {
        public override string Command => "clear";

        public override string Help =>
            @"Clears the debug console.
Usage: clear";

        [SerializeField]
        [Tooltip("The debug console text box area.")]
        private Text consoleTextArea;

        public override string Execute(params string[] parameters)
        {
            if (parameters.Length == 0)
            {
                consoleTextArea.text = "";
                return "Debug Console.";
            }
            else
            {
                return "Error: Invalid parameters. Usage: clear";
            }
        }
    }
}