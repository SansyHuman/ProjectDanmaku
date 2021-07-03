using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Util;

namespace SansyHuman.Experiment.Lua
{
    internal static class LuaUtilFunctions
    {
        internal static Table ViewportToWorld(Script script, double x, double y)
        {
            Vector3 vecPos = Camera.main.ViewportToWorldPoint(new Vector3((float)x, (float)y));
            return script.Call(script.Globals["vector"], vecPos.x, vecPos.y).Table;
        }

        internal static Table WorldToViewport(Script script, double x, double y)
        {
            Vector3 vecPos = Camera.main.WorldToViewportPoint(new Vector3((float)x, (float)y));
            return script.Call(script.Globals["vector"], vecPos.x, vecPos.y).Table;
        }

        internal static Table GetPlayerPosition(Script script)
        {
            Vector3 vecPos = GameManager.player.transform.position;
            return script.Call(script.Globals["vector"], vecPos.x, vecPos.y).Table;
        }

        /// <summary>
        /// Transforms vector table to vector.
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="vector">Vector2</param>
        /// <returns>true if the table is valid, else false</returns>
        public static bool TableToVector(Table table, out Vector2 vector)
        {
            DynValue rawX = table.Get("x");
            if (rawX.Type != DataType.Number)
                goto InvalidVector;

            float x = (float)rawX.Number;

            DynValue rawY = table.Get("y");
            if (rawY.Type != DataType.Number)
                goto InvalidVector;

            float y = (float)rawY.Number;

            vector = new Vector2(x, y);
            return true;

            InvalidVector:

            vector = new Vector2();
            return false;
        }

        internal static UDEMath.TimeFunction GetEaseFunction(string type)
        {
            if (Enum.TryParse<UDETransitionHelper.EaseType>("Ease" + type, true, out UDETransitionHelper.EaseType easeType))
            {
                return UDETransitionHelper.EaseTypeOf(easeType);
            }
            else
            {
                Debug.LogError("Unknown ease type");
                return UDETransitionHelper.easeLinear;
            }
        }
    }
}