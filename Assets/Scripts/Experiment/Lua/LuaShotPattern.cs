using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Pattern;
using MoonSharp.Interpreter.Interop;

namespace SansyHuman.Experiment.Lua
{
    public class LuaShotPattern : UDEBaseShotPattern
    {
        [SerializeField]
        [Tooltip("Name of the shot pattern Lua script")]
        private string scriptName;

        private Script script;
        private DynValue function;

        private void Awake()
        {
            script = new Script(CoreModules.Preset_SoftSandbox);

            DynValue pattern = UserData.Create(this);
            script.Globals.Set("parent", pattern);
            script.Globals["waitForScaledSecond"] = (Func<Script, string, double, Table>)LuaUtilFunctions.WaitForScaledSecond;
            script.Globals["vector"] = (Func<Script, double, double, Table>)LuaUtilFunctions.NewVector;

            function = script.DoFile(scriptName);
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void summonBullet(
            string name,
            double initX,
            double initY,
            double originX,
            double originY,
            double initRot,
            bool setOriginToCharacter,
            bool loop,
            Table movements)
        {
            UDEBulletPool bulletPool = UDEBulletPool.Instance;

            UDEAbstractBullet prefab = BulletMap.Instance[name];
            if (prefab != null)
            {
                UDEBulletMovement[] moves = new UDEBulletMovement[movements.Length];

                for (int i = 0; i < movements.Length; i++)
                {
                    Table move = movements[i + 1] as Table;
                    if (move == null)
                        goto TableError;

                    moves[i] = UDEBulletMovement.GetNoMovement();

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
                            goto TableError;
                    }

                    foreach (TablePair keyVal in move.Pairs)
                    {
                        if (keyVal.Key.Type != DataType.String)
                            goto TableError;

                        string key = keyVal.Key.String;

                        switch (key)
                        {
                            case "mode":
                                break;
                            case "startTime":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].startTime = (float)keyVal.Value.Number;
                                break;
                            case "endTime":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].endTime = (float)keyVal.Value.Number;
                                break;
                            case "hasEndTime":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto TableError;
                                moves[i].hasEndTime = keyVal.Value.Boolean;
                                break;
                            case "limitSpeed":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto TableError;
                                moves[i].limitSpeed = keyVal.Value.Boolean;
                                break;
                            case "setSpeedToPrevMovement":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto TableError;
                                moves[i].setSpeedToPrevMovement = keyVal.Value.Boolean;
                                break;
                            case "velocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto TableError;

                                Table velocity = keyVal.Value.Table;
                                if (velocity.Length != 2)
                                    goto TableError;

                                if (velocity.Get(1).Type != DataType.Number)
                                    goto TableError;
                                if (velocity.Get(2).Type != DataType.Number)
                                    goto TableError;

                                moves[i].velocity = new Vector2((float)velocity.Get(1).Number, (float)velocity.Get(2).Number);
                                break;
                            case "maxVelocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto TableError;

                                Table maxVelocity = keyVal.Value.Table;
                                if (maxVelocity.Length != 2)
                                    goto TableError;

                                if (maxVelocity.Get(1).Type != DataType.Number)
                                    goto TableError;
                                if (maxVelocity.Get(2).Type != DataType.Number)
                                    goto TableError;

                                moves[i].maxVelocity = new Vector2((float)maxVelocity.Get(1).Number, (float)maxVelocity.Get(2).Number);
                                break;
                            case "minVelocity":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto TableError;

                                Table minVelocity = keyVal.Value.Table;
                                if (minVelocity.Length != 2)
                                    goto TableError;

                                if (minVelocity.Get(1).Type != DataType.Number)
                                    goto TableError;
                                if (minVelocity.Get(2).Type != DataType.Number)
                                    goto TableError;

                                moves[i].maxVelocity = new Vector2((float)minVelocity.Get(1).Number, (float)minVelocity.Get(2).Number);
                                break;
                            case "maxMagnitude":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].maxMagnitude = (float)keyVal.Value.Number;
                                break;
                            case "minMagnitude":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].minMagnitude = (float)keyVal.Value.Number;
                                break;
                            case "accel":
                                if (keyVal.Value.Type != DataType.Table)
                                    goto TableError;

                                Table accel = keyVal.Value.Table;
                                if (accel.Length != 2)
                                    goto TableError;

                                if (accel.Get(1).Type != DataType.Number)
                                    goto TableError;
                                if (accel.Get(2).Type != DataType.Number)
                                    goto TableError;

                                moves[i].accel = t => new UDEMath.CartesianCoord((float)accel.Get(1).Number, (float)accel.Get(2).Number);
                                break;
                            case "speed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].speed = (float)keyVal.Value.Number;
                                break;
                            case "maxSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].maxSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].minSpeed = (float)keyVal.Value.Number;
                                break;
                            case "angle":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].angle = (float)keyVal.Value.Number;
                                break;
                            case "tangentialAccel":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].tangentialAccel = t => (float)keyVal.Value.Number;
                                break;
                            case "normalAccel":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].normalAccel = t => (float)keyVal.Value.Number;
                                break;
                            case "radialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].radialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "angularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].angularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxRadialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].maxRadialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].maxAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minRadialSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].minRadialSpeed = (float)keyVal.Value.Number;
                                break;
                            case "minAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].minAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "radialAccel":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].radialAccel = t => (float)keyVal.Value.Number;
                                break;
                            case "angularAccel":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].angularAccel = t => (float)keyVal.Value.Number;
                                break;
                            case "faceToMovingDirection":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto TableError;
                                moves[i].faceToMovingDirection = keyVal.Value.Boolean;
                                break;
                            case "rotationAngularSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].rotationAngularSpeed = (float)keyVal.Value.Number;
                                break;
                            case "rotationAngularAcceleration":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].rotationAngularAcceleration = t => (float)keyVal.Value.Number;
                                break;
                            case "limitRotationSpeed":
                                if (keyVal.Value.Type != DataType.Boolean)
                                    goto TableError;
                                moves[i].limitRotationSpeed = keyVal.Value.Boolean;
                                break;
                            case "minRotationSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].minRotationSpeed = (float)keyVal.Value.Number;
                                break;
                            case "maxRotationSpeed":
                                if (keyVal.Value.Type != DataType.Number)
                                    goto TableError;
                                moves[i].maxRotationSpeed = (float)keyVal.Value.Number;
                                break;
                            default:
                                break;
                        }
                    }
                }

                UDEAbstractBullet bullet = bulletPool.GetBullet(prefab);
                bullet.Initialize(
                    new Vector2((float)initX,
                    (float)initY),
                    new Vector2((float)originX,
                    (float)originY),
                    (float)initRot,
                    originEnemy,
                    this,
                    moves,
                    setOriginToCharacter,
                    loop);
            }
            else
            {
                Debug.LogError($"No such bullet named {name}.");
                return;
            }

            TableError:

            Debug.LogError($"The movement argument is wrong.");
            return;
        }

        /// <summary>
        /// Used in Lua script.
        /// </summary>
        [MoonSharpVisible(true)]
        private void getCharacterPos(out double x, out double y)
        {
            x = originEnemy.transform.position.x;
            y = originEnemy.transform.position.y;
        }

        [MoonSharpVisible(true)]
        private void cart2polar(double x, double y, out double r, out double deg)
        {
            float fr, fdeg;
            UDEMath.Cartesian2Polar((float)x, (float)y, out fr, out fdeg);
            r = fr; deg = fdeg;
        }

        [MoonSharpVisible(true)]
        private void polar2cart(double r, double deg, out double x, out double y)
        {
            float fx, fy;
            UDEMath.Polar2Cartesian((float)r, (float)deg, out fx, out fy);
            x = fx; y = fy;
        }

        [MoonSharpHidden]
        protected override IEnumerator ShotPattern()
        {
            DynValue coroutine = script.CreateCoroutine(function);

            foreach (DynValue ret in coroutine.Coroutine.AsTypedEnumerable())
            {
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
                float time = (float)rawTimeScale.Number;

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
    }
}