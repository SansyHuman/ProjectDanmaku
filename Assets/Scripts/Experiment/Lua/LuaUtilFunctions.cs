using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.Experiment.Lua
{
    internal class LuaUtilFunctions
    {
        public static Table WaitForScaledSecond(Script script, string timeScale, double time)
        {
            Table tbl = new Table(script);

            tbl["timescale"] = timeScale;
            tbl["time"] = time;

            return tbl;
        }

        public static Table NewVector(Script script, double x, double y)
        {
            Table vec = new Table(script);

            vec["x"] = x;
            vec["y"] = y;

            return vec;
        }
    }
}