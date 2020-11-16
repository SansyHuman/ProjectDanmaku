using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    [DisallowMultipleComponent]
    public class ShowCoordinates : CommandBehaviour
    {
        [SerializeField]
        private TransformDebug transformCanvas;

        public override string Command => "showcoord";

        public override string Help =>
            @"Shows the coordinates of entities.
Usage: showcoord <target: enemy|player|bullet|id|all|none> [params: id]

target is one of the followings
player: Shows all players.
enemy: Shows all enemies.
bullet: Shows all bullets.
id: Shows the specified object with the instance id.
all: Shows all players, enemies, and bullets.
none: Stops showing coordinates.

id is the list of instance id of the entities to show.
This should be specified only if the target is id.

if new showcoord command is executed, the previous showcoord will be disabled.";

        public override string Execute(params string[] parameters)
        {
            try
            {
                StringBuilder result = new StringBuilder();

                if (parameters.Length == 0)
                {
                    return "Error: Invalid parameters. Usage: showcoord <target: enemy|player|bullet|id|all|none> [params: id]";
                }

                if (parameters[0] == "id" && parameters.Length == 1)
                {
                    return "Error: You should specify instance ids if the target is id.";
                }

                if (parameters[0] != "id" && parameters.Length > 1)
                {
                    return "Error: Do not specify instance ids if the target is not id.";
                }

                switch (parameters[0])
                {
                    case "enemy":
                        transformCanvas.ShowEnemies();
                        break;
                    case "player":
                        transformCanvas.ShowPlayer();
                        break;
                    case "bullet":
                        transformCanvas.ShowBullets();
                        break;
                    case "id":
                        {
                            int[] ids = new int[parameters.Length - 1];
                            for (int i = 0; i < ids.Length; i++)
                                ids[i] = int.Parse(parameters[i + 1]);

                            var exists = transformCanvas.ShowInstanceID(ids);
                            for (int i = 0; i < exists.Length; i++)
                            {
                                if (!exists[i])
                                {
                                    result.AppendLine($"Warning: The instance with id {ids[i]} does not exist or have transform.");
                                }
                            }
                        }
                        break;
                    case "all":
                        transformCanvas.ShowAll();
                        break;
                    case "none":
                        transformCanvas.StopShowing();
                        break;
                    default:
                        return "Error: Invalid parameters. Usage: showcoord <target: enemy|player|bullet|id|all|none> [params: id]";
                }

                return result.ToString();
            }
            catch (Exception)
            {
                return "Error: Invalid parameters. Usage: showcoord <target: enemy|player|bullet|id|all|none> [params: id]";
            }
        }
    }
}