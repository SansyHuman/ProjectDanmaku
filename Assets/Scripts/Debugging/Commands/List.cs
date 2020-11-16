using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    public class List : ICommand
    {
        public string Command => "list";

        public string Help =>
            @"Lists all entities's name and instance id.
Usage: list <target: player|enemy|bullet|all> [start] [count]

target is one of the followings
player: Lists all players.
enemy: Lists all enemies.
bullet: Lists all bullets.
all: Lists all players, enemies, and bullets.

start is the start index of the entity array to print.
If not specified, the start index is 0.

count is the number of entities to print.
If not specified, it prints the whole entities from the start index.

If target is all, then start and count parameter cannot be used.";

        public string Execute(params string[] parameters)
        {
            try
            {
                if (parameters.Length >= 1)
                {
                    if (parameters[0] == "all" && parameters.Length != 1)
                        return "Error: all target should not have start and count parameters.";

                    int start = 0;
                    int count = -1;

                    bool startAssigned = false;
                    bool countAssigned = false;

                    if (parameters.Length >= 2)
                    {
                        int index = parameters[1].IndexOf('=');
                        if (index < 0)
                        {
                            start = int.Parse(parameters[1].Substring(index + 1));
                            startAssigned = true;
                        }
                        else
                        {
                            string[] nameValue = parameters[1].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (nameValue.Length != 2)
                                return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";

                            if (nameValue[0] == "start")
                            {
                                start = int.Parse(nameValue[1]);
                                startAssigned = true;
                            }
                            else if (nameValue[0] == "count")
                            {
                                count = int.Parse(nameValue[1]);
                                countAssigned = true;
                            }
                            else
                                return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";
                        }
                    }
                    if (parameters.Length == 3)
                    {
                        int index = parameters[2].IndexOf('=');
                        if (index < 0)
                        {
                            if (countAssigned)
                                return "Error: Duplicated count parameter.";

                            count = int.Parse(parameters[2].Substring(index + 1));
                        }
                        else
                        {
                            string[] nameValue = parameters[2].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (nameValue.Length != 2)
                                return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";

                            if (nameValue[0] == "start")
                            {
                                if (startAssigned)
                                    return "Error: Duplicated start parameter.";

                                start = int.Parse(nameValue[1]);
                            }
                            else if (nameValue[0] == "count")
                            {
                                if (countAssigned)
                                    return "Error: Duplicated count parameter.";

                                count = int.Parse(nameValue[1]);
                            }
                            else
                                return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";
                        }
                    }

                    if (parameters.Length > 3)
                        return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";

                    StringBuilder result = new StringBuilder();

                    switch (parameters[0])
                    {
                        case "player":
                            ListPlayers(result, start, count);
                            break;
                        case "enemy":
                            ListEnemies(result, start, count);
                            break;
                        case "bullet":
                            ListBullets(result, start, count);
                            break;
                        case "all":
                            ListPlayers(result, 0, -1);
                            result.AppendLine();
                            ListEnemies(result, 0, -1);
                            result.AppendLine();
                            ListBullets(result, 0, -1);
                            break;
                        default:
                            return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";
                    }

                    return result.ToString();
                }
                else
                {
                    return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";
                }
            }
            catch (Exception)
            {
                return "Error: Invalid parameters. Usage: list <player|enemy|bullet|all> [start] [count]";
            }
        }

        private void ListPlayers(StringBuilder builder, int start, int count)
        {
            UDEPlayer[] players = UnityEngine.Object.FindObjectsOfType<UDEPlayer>();
            if (count < 0)
                count = players.Length - start;

            builder.AppendLine("Player list...");

            for (int i = start; i < players.Length && i < start + count; i++)
            {
                builder.AppendLine($"Name: {players[i].gameObject.name}, ID: {players[i].gameObject.GetInstanceID()}");
            }
        }

        private void ListEnemies(StringBuilder builder, int start, int count)
        {
            var enemies = UDEObjectManager.Instance.GetAllEnemies();
            if (count < 0)
                count = enemies.Count - start;

            builder.AppendLine("Enemy list...");

            for (int i = start; i < enemies.Count && i < start + count; i++)
            {
                builder.AppendLine($"Name: {enemies[i].gameObject.name}, ID: {enemies[i].gameObject.GetInstanceID()}");
            }
        }

        private void ListBullets(StringBuilder builder, int start, int count)
        {
            var bullets = UDEObjectManager.Instance.GetAllBullets();
            if (count < 0)
                count = bullets.Count - start;

            builder.AppendLine("Bullet list...");

            for (int i = start; i < bullets.Count && i < start + count; i++)
            {
                builder.AppendLine($"Name: {bullets[i].gameObject.name}, ID: {bullets[i].gameObject.GetInstanceID()}");
            }
        }
    }
}