using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    [DisallowMultipleComponent]
    public class ShowCollider : CommandBehaviour
    {
        [SerializeField]
        private GLDraw targetCamera;

        public override string Command => "showcoll";

        public override string Help =>
            @"Shows the bound of the collider of the entity.
Usage: showcoll <target: enemy|player|bullet|id|all|none> <mode: quad|line> [params: id]

target is one of the followings.
player: Shows all players.
enemy: Shows all enemies.
bullet: Shows all bullets.
id: Shows the specified object with the instance id.
all: Shows all players, enemies, and bullets.
none: Stops showing colliders.

mode is one of the followings.
quad: Draws filled rectangle.
line: Draws rectangle's border line.

id is the list of instance id of the entities to show.
This should be specified only if the target is id.

if new showcoll command is executed, the previous showcoll will be disabled.";

        public override string Execute(params string[] parameters)
        {
            try
            {
                StringBuilder result = new StringBuilder();

                if (parameters.Length < 2)
                {
                    return "Error: Invalid parameters. Usage: showcoll <target: enemy|player|bullet|id|all|none> <mode: quad|line> [params: id]";
                }

                if (parameters[0] == "id" && parameters.Length == 2)
                {
                    return "Error: You should specify instance ids if the target is id.";
                }

                if (parameters[0] != "id" && parameters.Length > 2)
                {
                    return "Error: Do not specify instance ids if the target is not id.";
                }

                switch (parameters[0])
                {
                    case "enemy":
                        targetCamera.RenderEnemies();
                        break;
                    case "player":
                        targetCamera.RenderPlayer();
                        break;
                    case "bullet":
                        targetCamera.RenderBullets();
                        break;
                    case "id":
                        {
                            int[] ids = new int[parameters.Length - 2];
                            for (int i = 0; i < ids.Length; i++)
                                ids[i] = int.Parse(parameters[i + 2]);

                            var exists = targetCamera.RenderInstanceID(ids);
                            for (int i = 0; i < exists.Length; i++)
                            {
                                if (!exists[i])
                                {
                                    result.AppendLine($"Warning: The instance with id {ids[i]} does not exist or have collider.");
                                }
                            }
                        }
                        break;
                    case "all":
                        targetCamera.RenderAll();
                        break;
                    case "none":
                        targetCamera.StopRendering();
                        break;
                    default:
                        return "Error: Invalid parameters. Usage: showcoll <target: enemy|player|bullet|id|all|none> <mode: quad|line> [params: id]";
                }

                switch (parameters[1])
                {
                    case "quad":
                        targetCamera.DrawingMode = GLDraw.DrawMode.QUADS;
                        break;
                    case "line":
                        targetCamera.DrawingMode = GLDraw.DrawMode.LINES;
                        break;
                    default:
                        return "Error: Invalid parameters. Usage: showcoll <target: enemy|player|bullet|id|all|none> <mode: quad|line> [params: id]";
                }

                return result.ToString();
            }
            catch(Exception)
            {
                return "Error: Invalid parameters. Usage: showcoll <target: enemy|player|bullet|id|all|none> <mode: quad|line> [params: id]";
            }
        }
    }
}