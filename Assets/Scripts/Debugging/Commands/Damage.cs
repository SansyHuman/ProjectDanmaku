using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SansyHuman.UDE.Object;
using UnityEngine;

namespace SansyHuman.Debugging.Commands
{
    public class Damage : ICommand
    {
        public string Command => "damage";

        public string Help =>
            @"Reduces the health of the enemy or player.
Usage: damage <target: enemy|player> <id> [amount]

target is one of the followings.
enemy: Damages enemy.
player: Damages player.

id is the instance id of the target. It can be obtained by 'list' command.

amount is the damage to deal to the target.
If [amount] is not specified, it will deal 1 damage.";

        public string Execute(params string[] parameters)
        {
            try
            {
                if (parameters.Length == 2 || parameters.Length == 3)
                {
                    GameObject target = DebugUtil.FindObjectFromInstanceID<GameObject>(int.Parse(parameters[1]));
                    if (target == null || !target.activeInHierarchy)
                        return "Error: Invalid instance id.";

                    UDEBaseCharacter targetScript = null;

                    switch (parameters[0])
                    {
                        case "enemy":
                            targetScript = target.GetComponent<UDEEnemy>();
                            break;
                        case "player":
                            targetScript = target.GetComponent<UDEPlayer>();
                            break;
                        default:
                            return "Error: Invalid parameters. Usage: damage <target: enemy|player> <id> [amount]";
                    }

                    if (targetScript == null)
                        return "Error: Invalid instance id.";

                    float health = targetScript.Health;
                    float damage = 0.0f;
                    if (parameters.Length == 3)
                        damage = float.Parse(parameters[2]);
                    else
                        damage = 1.0f;
                    health -= damage;

                    var healthField = typeof(UDEBaseCharacter).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);
                    healthField.SetValue(targetScript, health);

                    return $"The health of {parameters[0]} {target.name}({parameters[1]}) had {damage} damage and the current health is {health}.";
                }
                else
                {
                    return "Error: Invalid parameters. Usage: damage <target: enemy|player> <id> [amount]";
                }
            }
            catch(Exception)
            {
                return "Error: Invalid parameters. Usage: damage <target: enemy|player> <id> [amount]";
            }
        }
    }
}