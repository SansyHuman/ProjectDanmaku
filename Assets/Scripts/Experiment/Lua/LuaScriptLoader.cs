using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace SansyHuman.Experiment.Lua
{
    /// <summary>
    /// Initializes Lua script loader
    /// </summary>
    [DisallowMultipleComponent]
    public class LuaScriptLoader : UDESingleton<LuaScriptLoader>
    {
        protected override void Awake()
        {
            base.Awake();

            Dictionary<string, string> scripts = new Dictionary<string, string>();

            TextAsset[] result = Resources.LoadAll<TextAsset>("Lua");
            for (int i = 0; i < result.Length; i++)
                scripts.Add(result[i].name, result[i].text);

            var scriptLoader = new UnityAssetsScriptLoader(scripts);
            Script.DefaultOptions.ScriptLoader = scriptLoader;

            UserData.RegisterType<LuaShotPattern>();
        }
    }
}