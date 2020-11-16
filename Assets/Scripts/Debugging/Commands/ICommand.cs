using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    /// <summary>
    /// Interface of all debug console commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// The information printed when using help command. It contains the descriptions of the
        /// command and the usage.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameters">The parameters of the command.</param>
        /// <returns>The string that represents the result of the command execution.
        /// This string will be printed on the debug console.</returns>
        string Execute(params string[] parameters);
    }

    /// <summary>
    /// Base class of all MonoBehaviour-based commands. Use this if the command
    /// needs objects to be assigned in the inspector.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class CommandBehaviour : MonoBehaviour, ICommand
    {
        public abstract string Command { get; }
        public abstract string Help { get; }

        public abstract string Execute(params string[] parameters);
    }

}