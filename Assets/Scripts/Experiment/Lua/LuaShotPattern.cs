using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Util;
using MoonSharp.Interpreter.Interop;
using SansyHuman.Pattern;

namespace SansyHuman.Experiment.Lua
{
    public class LuaShotPattern : ShotPatternBase
    {
        [SerializeField]
        [Tooltip("Name of the shot pattern Lua script")]
        private string scriptName;

        [SerializeField]
        [Tooltip("Background object, which have sprite renderer, if exists")]
        private GameObject background;

        private SpriteRenderer backgroundSprite;
        private Transform backgroundTransform;

        private Script script;
        private DynValue patternFunc;
        private DynValue endFunc;
        private DynValue vectorFunc;
        private Dictionary<string, IEnumerator> subpatternDict;

        private void Awake()
        {
            if (background != null)
            {
                backgroundSprite = background.GetComponent<SpriteRenderer>();
                backgroundTransform = background.transform;
            }

            script = new Script(CoreModules.Preset_SoftSandbox);

            script.Options.DebugPrint = Debug.Log;

            DynValue pattern = UserData.Create(this);
            script.Globals.Set("parent", pattern);
            script.Globals["viewportToWorld_internal"] = (Func<Script, double, double, Table>)LuaUtilFunctions.ViewportToWorld;
            script.Globals["worldToViewport_internal"] = (Func<Script, double, double, Table>)LuaUtilFunctions.WorldToViewport;
            script.Globals["getPlayerPos_internal"] = (Func<Script, Table>)LuaUtilFunctions.GetPlayerPosition;

            script.DoFile("utils");
            script.DoFile(scriptName);
            patternFunc = script.Globals.Get(scriptName);
            endFunc = script.Globals.Get("onPatternEnd");
            vectorFunc = script.Globals.Get("vector");
            subpatternDict = new Dictionary<string, IEnumerator>();
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private bool canBeDamaged
        {
            get => originEnemy.CanBeDamaged;
            set => originEnemy.CanBeDamaged = value;
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void summonBullet(
            string name,
            Table initPos,
            Table origin,
            double initRot,
            Table bulletScale,
            double summonTime,
            bool setOriginToCharacter,
            bool loop,
            Table movements)
        {
            UDEBulletPool bulletPool = UDEBulletPool.Instance;

            UDEAbstractBullet prefab = BulletMap.Instance[name];
            if (prefab != null)
            {
                Vector2 initPosVec;
                if (!LuaUtilFunctions.TableToVector(initPos, out initPosVec))
                    goto VectorError;

                Vector2 originVec;
                if (!LuaUtilFunctions.TableToVector(origin, out originVec))
                    goto VectorError;

                Vector2 scaleVec;
                if (!LuaUtilFunctions.TableToVector(bulletScale, out scaleVec))
                    goto VectorError;

                UDEBulletMovement[] moves = new UDEBulletMovement[movements.Length];

                for (int i = 0; i < movements.Length; i++)
                {
                    Table move = movements[i + 1] as Table;
                    if (move == null)
                        goto MovementError;

                    moves[i] = UDEBulletMovement.GetNoMovement();
                    moves[i].maxVelocity = new Vector2(float.MaxValue, float.MaxValue);
                    moves[i].minVelocity = new Vector2(float.MinValue, float.MinValue);
                    moves[i].maxMagnitude = float.MaxValue;
                    moves[i].minMagnitude = float.MinValue;
                    moves[i].maxSpeed = float.MaxValue;
                    moves[i].minSpeed = float.MinValue;
                    moves[i].maxRadialSpeed = float.MaxValue;
                    moves[i].maxAngularSpeed = float.MaxValue;
                    moves[i].minRadialSpeed = float.MinValue;
                    moves[i].minAngularSpeed = float.MinValue;
                    moves[i].minRotationSpeed = float.MinValue;
                    moves[i].maxRotationSpeed = float.MaxValue;

                    string moveType = move["mode"] as string;
                    switch (moveType)
                    {
                        case "cart":
                            moves[i].mode = UDEBulletMovement.MoveMode.CARTESIAN;
                            break;
                        case "cartPolar":
                            moves[i].mode = UDEBulletMovement.MoveMode.CARTESIAN_POLAR;
                            break;
                        case "polar":
                            moves[i].mode = UDEBulletMovement.MoveMode.POLAR;
                            break;
                        default:
                            goto MovementError;
                    }

                    foreach (TablePair keyVal in move.Pairs)
                    {
                        if (keyVal.Key.Type != DataType.String)
                            goto MovementError;

                        string key = keyVal.Key.String;

                        switch (key)
                        {
                            case "mode":
                                break;
                            case "startTime":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].startTime = (float)keyVal.Value.Number;
                                break;
                            case "endTime":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].endTime = (float)keyVal.Value.Number;
                                break;
                            case "hasEndTime":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto MovementError;
                                moves[i].hasEndTime = keyVal.Value.Boolean;
                                break;
                            case "limitSpeed":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto MovementError;
                                moves[i].limitSpeed = keyVal.Value.Boolean;
                                break;
                            case "setSpeedToPrevMovement":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto MovementError;
                                moves[i].setSpeedToPrevMovement = keyVal.Value.Boolean;
                                break;
                            case "velocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto MovementError;

                                Table rawVelocity = keyVal.Value.Table;
                                if (!LuaUtilFunctions.TableToVector(rawVelocity, out Vector2 velocity))
                                    goto MovementError;

                                moves[i].velocity = velocity;
                                break;
                            case "maxVelocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto MovementError;

                                Table rawMaxVelocity = keyVal.Value.Table;
                                if (!LuaUtilFunctions.TableToVector(rawMaxVelocity, out Vector2 maxVelocity))
                                    goto MovementError;

                                moves[i].maxVelocity = maxVelocity;
                                break;
                            case "minVelocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto MovementError;

                                Table rawMinVelocity = keyVal.Value.Table;
                                if (!LuaUtilFunctions.TableToVector(rawMinVelocity, out Vector2 minVelocity))
                                    goto MovementError;

                                moves[i].minVelocity = minVelocity;
                                break;
                            case "maxMagnitude":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].maxMagnitude = (float)keyVal.Value.Number;
                                break;
                            case "minMagnitude":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].minMagnitude = (float)keyVal.Value.Number;
                                break;
                            case "accel":
                                if (keyVal.Value.Type == DataType.Table)
                                {
                                    Table accel = keyVal.Value.Table;
                                    Vector2 accelVec;
                                    if (!LuaUtilFunctions.TableToVector(accel, out accelVec))
                                        goto MovementError;

                                    moves[i].accel = t => new UDEMath.CartesianCoord(accelVec.x, accelVec.y);
                                }
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].accel = t =>
                                    {
                                        Table a = script.Call(function, t).Table;
                                        LuaUtilFunctions.TableToVector(a, out Vector2 av);
                                        return new UDEMath.CartesianCoord(av.x, av.y);
                                    };
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].accel = t =>
                                    {
                                        Table a = script.Call(function, t).Table;
                                        LuaUtilFunctions.TableToVector(a, out Vector2 av);
                                        return new UDEMath.CartesianCoord(av.x, av.y);
                                    };
                                }
                                else
                                    goto MovementError;
                                break;
                            case "speed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].speed = (float)keyVal.Value.Number;
                                break;
                            case "maxSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].maxSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].minSpeed = (float)keyVal.Value.Number;
                                break;
                            case "angle":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].angle = (float)keyVal.Value.Number;
                                break;
                            case "tangentialAccel":
                                if (keyVal.Value.Type == DataType.Number)
                                    moves[i].tangentialAccel = t => (float)keyVal.Value.Number;
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].tangentialAccel = t => (float)script.Call(function, t).Number;
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].tangentialAccel = t => (float)script.Call(function, t).Number;
                                }
                                else
                                    goto MovementError;
                                break;
                            case "normalAccel":
                                if (keyVal.Value.Type == DataType.Number)
                                    moves[i].normalAccel = t => (float)keyVal.Value.Number;
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].normalAccel = t => (float)script.Call(function, t).Number;
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].normalAccel = t => (float)script.Call(function, t).Number;
                                }
                                else
                                    goto MovementError;
                                break;
                            case "radialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].radialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "angularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].angularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxRadialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].maxRadialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].maxAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minRadialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].minRadialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].minAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "radialAccel":
                                if (keyVal.Value.Type == DataType.Number)
                                    moves[i].radialAccel = t => (float)keyVal.Value.Number;
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].radialAccel = t => (float)script.Call(function, t).Number;
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].radialAccel = t => (float)script.Call(function, t).Number;
                                }
                                else
                                    goto MovementError;
                                break;
                            case "angularAccel":
                                if (keyVal.Value.Type == DataType.Number)
                                    moves[i].angularAccel = t => (float)keyVal.Value.Number;
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].angularAccel = t => (float)script.Call(function, t).Number;
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].angularAccel = t => (float)script.Call(function, t).Number;
                                }
                                else
                                    goto MovementError;
                                break;
                            case "faceToMovingDirection":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto MovementError;
                                moves[i].faceToMovingDirection = keyVal.Value.Boolean;
                                break;
                            case "rotationAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].rotationAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "rotationAngularAcceleration":
                                if (keyVal.Value.Type == DataType.Number)
                                    moves[i].rotationAngularAcceleration = t => (float)keyVal.Value.Number;
                                else if (keyVal.Value.Type == DataType.String)
                                {
                                    DynValue function = script.Globals.Get(keyVal.Value.String);
                                    if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
                                        goto MovementError;

                                    moves[i].rotationAngularAcceleration = t => (float)script.Call(function, t).Number;
                                }
                                else if (keyVal.Value.Type == DataType.Function || keyVal.Value.Type == DataType.ClrFunction)
                                {
                                    DynValue function = keyVal.Value;

                                    moves[i].rotationAngularAcceleration = t => (float)script.Call(function, t).Number;
                                }
                                else
                                    goto MovementError;
                                break;
                            case "limitRotationSpeed":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto MovementError;
                                moves[i].limitRotationSpeed = keyVal.Value.Boolean;
                                break;
                            case "minRotationSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].minRotationSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxRotationSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto MovementError;
                                moves[i].maxRotationSpeed = (float)keyVal.Value.Number;
                                break;
                            default:
                                break;
                        }
                    }
                }

                UDEAbstractBullet bullet = bulletPool.GetBullet(prefab);
                bullet.transform.localScale = scaleVec;
                bullet.SummonTime = (float)summonTime;
                bullet.Initialize(
                    initPosVec,
                    originVec,
                    (float)initRot,
                    originEnemy,
                    this,
                    moves,
                    setOriginToCharacter,
                    loop);
                return;
            }
            else
            {
                Debug.LogError($"No such bullet named {name}.");
                return;
            }

            MovementError:

            Debug.LogError($"The movement argument is wrong.");
            return;

            VectorError:

            Debug.LogError($"The vector in argument is invalid.");
            return;
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private Table getCharacterPos()
        {
            return script.Call(vectorFunc, originEnemy.transform.position.x, originEnemy.transform.position.y).Table;
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void moveCharacterTo(double x, double y, double duration, string easeType, bool physics)
        {
            UDETransitionHelper.MoveTo(originEnemy.gameObject, (float)x, (float)y, (float)duration, LuaUtilFunctions.GetEaseFunction(easeType), UDETime.TimeScale.ENEMY, physics);
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void moveCharacterAmount(double dx, double dy, double duration, string easeType, bool physics)
        {
            UDETransitionHelper.MoveAmount(originEnemy.gameObject, (float)dx, (float)dy, (float)duration, LuaUtilFunctions.GetEaseFunction(easeType), UDETime.TimeScale.ENEMY, physics);
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void setCharacterPosition(double x, double y)
        {
            originEnemy.transform.position = new Vector3((float)x, (float)y);
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private double getBackgroundScale(out double y)
        {
            if (background == null)
            {
                Debug.LogError("No character background.");
                y = -1;
                return -1;
            }

            y = backgroundTransform.localScale.y;
            return backgroundTransform.localScale.x;
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void changeBackgroundScaleTo(double x, double y, double duration, string easeType, bool physics)
        {
            if (background == null)
            {
                Debug.LogError("No character background.");
                return;
            }
            UDETransitionHelper.ChangeScaleTo(background, new Vector3((float)x, (float)y, 1), (float)duration, LuaUtilFunctions.GetEaseFunction(easeType), UDETime.TimeScale.ENEMY, physics);
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private double getBackgroundColor(out double g, out double b, out double a)
        {
            if (background == null)
            {
                Debug.LogError("No character background.");
                g = -1;
                b = -1;
                a = -1;
                return -1;
            }

            Color color = backgroundSprite.color;

            g = color.g;
            b = color.b;
            a = color.a;
            return color.r;
        }

        /// <summary>
        /// Used in Lua script. Changes the background color by time with ease function, if the background exists.
        /// </summary>
        /// <param name="r">R value of the color to change to</param>
        /// <param name="g">G value of the color to change to</param>
        /// <param name="b">B value of the color to change to</param>
        /// <param name="a">A value of the color to change to</param>
        /// <param name="duration">Transition time</param>
        /// <param name="easeType">Type of ease function</param>
        /// <param name="physics">Whether the transition is calculated in FixedUpdate or not</param>
        [MoonSharpVisible(true)]
        private void changeBackgroundColorTo(double r, double g, double b, double a, double duration, string easeType, bool physics)
        {
            if (background == null)
            {
                Debug.LogError("No character background.");
                return;
            }
            UDETransitionHelper.ChangeColorTo(background, new Color((float)r, (float)g, (float)b, (float)a), (float)duration, LuaUtilFunctions.GetEaseFunction(easeType), UDETime.TimeScale.ENEMY, physics);
        }

        /// <summary>
        /// Used in Lua script. Stops all transitions of the character.
        /// </summary>
        [MoonSharpVisible(true)]
        private void stopAllTransitions()
        {
            UDETransitionHelper.StopAllTransitions(originEnemy.gameObject);
            if (background != null)
            {
                UDETransitionHelper.StopAllTransitions(background);
            }
        }

        /// <summary>
        /// Used in Lua script. Sets the rotation of the character.
        /// </summary>
        /// <param name="deg">Rotation of the character in degrees</param>
        [MoonSharpVisible(true)]
        private void setCharacterRotation(double deg)
        {
            originEnemy.transform.rotation = Quaternion.Euler(0, 0, (float)deg);
        }

        /// <summary>
        /// Used in Lua script. Starts subpattern that runs parallel with the main pattern.
        /// </summary>
        /// <param name="name">The name of subpattern function in Lua script</param>
        [MoonSharpVisible(true)]
        private void startSubpattern(string name)
        {
            if (subpatternDict.ContainsKey(name))
            {
                Debug.LogError($"Subpattern {name} is already running.");
                return;
            }

            DynValue subpattern = script.Globals.Get(name);
            if (subpattern.Type != DataType.Function && subpattern.Type != DataType.ClrFunction)
            {
                Debug.LogError($"No such subpattern function named {name}.");
                return;
            }

            IEnumerator coroutine = PlayPattern(subpattern);
            subpatternDict.Add(name, coroutine);

            StartCoroutine(coroutine);
        }

        /// <summary>
        /// Used in Lua script. Stops subpattern.
        /// </summary>
        /// <param name="name">The name of subpattern function in Lua script</param>
        [MoonSharpVisible(true)]
        private void stopSubpattern(string name)
        {
            if (!subpatternDict.ContainsKey(name))
            {
                Debug.LogError($"Subpattern {name} is not running.");
                return;
            }

            StopCoroutine(subpatternDict[name]);
            subpatternDict.Remove(name);
        }

        private IEnumerator PlayPattern(DynValue pattern)
        {
            DynValue coroutine = script.CreateCoroutine(pattern);

            foreach (DynValue ret in coroutine.Coroutine.AsTypedEnumerable())
            {
                if (ret.Type == DataType.Void)
                    yield break;

                if (ret.Type != DataType.Table)
                {
                    Debug.LogError("Unknown yield.");
                    yield return null;
                    continue;
                }

                Table wait = ret.Table;
                DynValue rawTimeScale = wait.Get("timescale");
                if (rawTimeScale.Type != DataType.String)
                {
                    Debug.LogError("Unknown yield.");
                    yield return null;
                    continue;
                }
                string timeScale = rawTimeScale.String;

                DynValue rawTime = wait.Get("time");
                if (rawTime.Type != DataType.Number)
                {
                    Debug.LogError("Unknown yield.");
                    yield return null;
                    continue;
                }
                float time = (float)rawTime.Number;

                switch (timeScale)
                {
                    case "enemy":
                        yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(time, UDETime.TimeScale.ENEMY));
                        break;
                    case "player":
                        yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(time, UDETime.TimeScale.ENEMY)); yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(time, UDETime.TimeScale.ENEMY));
                        break;
                    case "unscaled":
                        yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(time, UDETime.TimeScale.ENEMY));
                        break;
                    default:
                        Debug.LogError("Unknown yield.");
                        yield return null;
                        continue;
                }
            }
        }

        [MoonSharpHidden]
        protected override IEnumerator ShotPattern()
        {
            yield return StartCoroutine(PlayPattern(patternFunc));
        }

        public override void EndPattern()
        {
            base.EndPattern();

            subpatternDict.Clear();

            script.Call(endFunc);
        }
    }
}