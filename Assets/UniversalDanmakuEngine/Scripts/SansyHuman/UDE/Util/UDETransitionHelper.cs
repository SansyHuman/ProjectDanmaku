// Copyright (c) 2019 Subo Lee (KAIST HAJE)
// Please direct any bugs/comments/suggestions to suboo0308@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.UDE.Util
{
    /// <summary>
    /// Class to help control objects' movements and transitions.
    /// <para>All easing function used in this class and methods should start from (0, 0) and end to (1, 1).</para>
    /// </summary>
    public static class UDETransitionHelper
    {
        /// <summary>
        /// Interface of all structs that contains informations of transition.
        /// </summary>
        public interface ITransitiontInfo
        {
            /// <summary>
            /// Add this transition information to the object.
            /// </summary>
            /// <param name="obj">Game object to apply the transition</param>
            /// <returns><see cref="UDETransitionHelper.TransitionResult"/> that contains the state of the transition</returns>
            TransitionResult AddTo(GameObject obj);

            /// <summary>
            /// Progress and update the transition.
            /// Only called internally in <see cref="UDETransition.Update()"/> and <see cref="UDETransition.FixedUpdate()"/>.
            /// </summary>
            /// <param name="deltaTime">Unscaled delta time in <see cref="UDETransition.Update()"/> and <see cref="UDETransition.FixedUpdate()"/></param>
            /// <param name="transform"><see cref="Transform"/> of the game object</param>
            /// <seealso cref="UDETransition"/>
            void Progress(float deltaTime, Transform transform);

            /// <value>Gets and sets if the transition should be managed in <see cref="UDETransition.FixedUpdate()"/>.
            /// If it is physics, it is managed in FixedUpdate.</value>
            bool IsPhysics { get; set; }

            /// <value>Gets the progression of the transition. Progression is between 0 to 1.</value>
            float Progression { get; }
        }

        /// <summary>
        /// Class that contains the result of the transition.
        /// </summary>
        public class TransitionResult
        {
            // Only used internally.
            internal bool endTransition = false;

            /// <value>Gets whether the transition ended.</value>
            public bool EndTransition { get => endTransition; }
        }

        /// <summary>
        /// Class that controls transitions registered to the game object.
        /// Only added to game object internally when it is needed.
        /// </summary>
        [DisallowMultipleComponent]
        public class UDETransition : MonoBehaviour
        {
            private struct TransitionInfo
            {
                public ITransitiontInfo info;
                public TransitionResult result;
            }

            private List<TransitionInfo> nonphysicsInfos = new List<TransitionInfo>();
            private List<TransitionInfo> physicsInfos = new List<TransitionInfo>();

            private Transform tr;

            private void Start()
            {
                tr = transform;
            }

            /// <summary>
            /// Adds transition information to the object.
            /// Only used internally in <see cref="ITransitiontInfo.AddTo(GameObject)"/>.
            /// </summary>
            /// <param name="info">Transition informaion to add</param>
            /// <returns><see cref="TransitionResult"/> that contains the state of the transition</returns>
            public TransitionResult AddInfo(ITransitiontInfo info)
            {
                TransitionInfo information = new TransitionInfo();
                information.info = info;
                TransitionResult result = new TransitionResult();
                information.result = result;

                if (information.info.IsPhysics)
                    physicsInfos.Add(information);
                else
                    nonphysicsInfos.Add(information);

                if (!enabled)
                    enabled = true;

                return result;
            }

            /// <summary>
            /// Stops all transitions. Only used internally.
            /// </summary>
            public void StopAllTransition()
            {
                nonphysicsInfos.Clear();
                physicsInfos.Clear();
                enabled = false;
            }

            // Not physics
            private void Update()
            {
                if (nonphysicsInfos.Count == 0)
                {
                    if (physicsInfos.Count == 0)
                        enabled = false;

                    return;
                }

                ITransitiontInfo current;
                for (int i = 0; i < nonphysicsInfos.Count; i++)
                {
                    current = nonphysicsInfos[i].info;
                    current.Progress(Time.deltaTime, tr);
                    if (current.Progression >= 1)
                    {
                        nonphysicsInfos[i].result.endTransition = true;
                        nonphysicsInfos.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Physics
            private void FixedUpdate()
            {
                if (physicsInfos.Count == 0)
                {
                    if (nonphysicsInfos.Count == 0)
                        enabled = false;

                    return;
                }

                ITransitiontInfo current;
                for (int i = 0; i < physicsInfos.Count; i++)
                {
                    current = physicsInfos[i].info;
                    current.Progress(Time.deltaTime, tr);
                    if (current.Progression >= 1)
                    {
                        physicsInfos[i].result.endTransition = true;
                        physicsInfos.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #region Basic Transitions
        private struct MoveToInfo : ITransitiontInfo
        {
            public Vector2 from;
            public Vector2 to;
            public float duration;
            public UDEMath.TimeFunction easeFunction;
            public UDETime.TimeScale timeScale;
            public bool isPhysics;

            private float progression;

            private float time;

            public bool IsPhysics { get => isPhysics; set => isPhysics = value; }
            public float Progression => progression;

            public TransitionResult AddTo(GameObject obj)
            {
                progression = 0; time = 0;

                UDETransition transition = obj.GetComponent<UDETransition>();
                if (transition == null)
                    transition = obj.AddComponent<UDETransition>();
                return transition.AddInfo(this);
            }

            public void Progress(float deltaTime, Transform transform)
            {
                float scaledDeltaTime = deltaTime;
                switch (timeScale)
                {
                    case UDETime.TimeScale.ENEMY:
                        scaledDeltaTime *= UDETime.Instance.EnemyTimeScale;
                        break;
                    case UDETime.TimeScale.PLAYER:
                        scaledDeltaTime *= UDETime.Instance.PlayerTimeScale;
                        break;
                    case UDETime.TimeScale.UNSCALED:
                    default:
                        break;
                }

                time += scaledDeltaTime;
                if (time > duration)
                    time = duration;

                progression = time / duration;
                float t = easeFunction(progression);
                Vector2 location = from * (1 - t) + to * t;

                transform.position = location;
            }
        }

        private struct MoveOnCurveInfo : ITransitiontInfo
        {
            public UDEMath.CartesianTimeFunction curve;
            public float duration;
            public UDEMath.TimeFunction easeFunction;
            public UDETime.TimeScale timeScale;
            public bool isPhysics;

            private float progression;

            private float time;

            public bool IsPhysics { get => isPhysics; set => isPhysics = value; }
            public float Progression => progression;

            public TransitionResult AddTo(GameObject obj)
            {
                progression = 0; time = 0;

                UDETransition transition = obj.GetComponent<UDETransition>();
                if (transition == null)
                    transition = obj.AddComponent<UDETransition>();
                return transition.AddInfo(this);
            }

            public void Progress(float deltaTime, Transform transform)
            {
                float scaledDeltaTime = deltaTime;
                switch (timeScale)
                {
                    case UDETime.TimeScale.ENEMY:
                        scaledDeltaTime *= UDETime.Instance.EnemyTimeScale;
                        break;
                    case UDETime.TimeScale.PLAYER:
                        scaledDeltaTime *= UDETime.Instance.PlayerTimeScale;
                        break;
                    case UDETime.TimeScale.UNSCALED:
                    default:
                        break;
                }

                time += scaledDeltaTime;
                if (time > duration)
                    time = duration;

                progression = time / duration;
                float t = easeFunction(progression);
                Vector2 location = curve(t);

                transform.position = location;
            }
        }

        private struct ColorChangeInfo : ITransitiontInfo
        {
            public Color from;
            public Color to;
            public float duration;
            public UDEMath.TimeFunction easeFunction;
            public UDETime.TimeScale timeScale;
            public bool isPhysics;

            private SpriteRenderer sprite;

            private float progression;

            private float time;

            public bool IsPhysics { get => isPhysics; set => isPhysics = value; }
            public float Progression => progression;

            public TransitionResult AddTo(GameObject obj)
            {
                progression = 0; time = 0;

                UDETransition transition = obj.GetComponent<UDETransition>();
                if (transition == null)
                    transition = obj.AddComponent<UDETransition>();

                sprite = obj.GetComponent<SpriteRenderer>();

                return transition.AddInfo(this);
            }

            public void Progress(float deltaTime, Transform transform)
            {
                float scaledDeltaTime = deltaTime;
                switch (timeScale)
                {
                    case UDETime.TimeScale.ENEMY:
                        scaledDeltaTime *= UDETime.Instance.EnemyTimeScale;
                        break;
                    case UDETime.TimeScale.PLAYER:
                        scaledDeltaTime *= UDETime.Instance.PlayerTimeScale;
                        break;
                    case UDETime.TimeScale.UNSCALED:
                    default:
                        break;
                }

                time += scaledDeltaTime;
                if (time > duration)
                    time = duration;

                progression = time / duration;
                float t = easeFunction(progression);
                Color color = Color.Lerp(from, to, t);

                sprite.color = color;
            }
        }

        private struct ScaleChangeInfo : ITransitiontInfo
        {
            public Vector3 from;
            public Vector3 to;
            public float duration;
            public UDEMath.TimeFunction easeFunction;
            public UDETime.TimeScale timeScale;
            public bool isPhysics;

            private float progression;

            private float time;

            public bool IsPhysics { get => isPhysics; set => isPhysics = value; }
            public float Progression => progression;

            public TransitionResult AddTo(GameObject obj)
            {
                progression = 0; time = 0;

                UDETransition transition = obj.GetComponent<UDETransition>();
                if (transition == null)
                    transition = obj.AddComponent<UDETransition>();
                return transition.AddInfo(this);
            }

            public void Progress(float deltaTime, Transform transform)
            {
                float scaledDeltaTime = deltaTime;
                switch (timeScale)
                {
                    case UDETime.TimeScale.ENEMY:
                        scaledDeltaTime *= UDETime.Instance.EnemyTimeScale;
                        break;
                    case UDETime.TimeScale.PLAYER:
                        scaledDeltaTime *= UDETime.Instance.PlayerTimeScale;
                        break;
                    case UDETime.TimeScale.UNSCALED:
                    default:
                        break;
                }

                time += scaledDeltaTime;
                if (time > duration)
                    time = duration;

                progression = time / duration;
                float t = easeFunction(progression);
                Vector3 scale = from * (1 - t) + to * t;

                transform.localScale = scale;
            }
        }

        private struct RotationInfo : ITransitiontInfo
        {
            public Vector3 from;
            public Vector3 to;
            public float duration;
            public UDEMath.TimeFunction easeFunction;
            public UDETime.TimeScale timeScale;
            public bool isPhysics;

            private float progression;

            private float time;

            public bool IsPhysics { get => isPhysics; set => isPhysics = value; }
            public float Progression => progression;

            public TransitionResult AddTo(GameObject obj)
            {
                progression = 0; time = 0;

                UDETransition transition = obj.GetComponent<UDETransition>();
                if (transition == null)
                    transition = obj.AddComponent<UDETransition>();
                return transition.AddInfo(this);
            }

            public void Progress(float deltaTime, Transform transform)
            {
                float scaledDeltaTime = deltaTime;
                switch (timeScale)
                {
                    case UDETime.TimeScale.ENEMY:
                        scaledDeltaTime *= UDETime.Instance.EnemyTimeScale;
                        break;
                    case UDETime.TimeScale.PLAYER:
                        scaledDeltaTime *= UDETime.Instance.PlayerTimeScale;
                        break;
                    case UDETime.TimeScale.UNSCALED:
                    default:
                        break;
                }

                time += scaledDeltaTime;
                if (time > duration)
                    time = duration;

                progression = time / duration;
                float t = easeFunction(progression);
                Vector3 rotation = from * (1 - t) + to * t;

                transform.rotation = Quaternion.Euler(rotation);
            }
        }
        #endregion

        #region Ease Functions
        /// <summary>
        /// Convert easing functon from in to out.
        /// <para>It can also convert out ease to in.</para>
        /// </summary>
        /// <param name="easeIn">Easing function to convert</param>
        /// <returns>Conversed easing function</returns>
        public static UDEMath.TimeFunction ConvertIn2Out(UDEMath.TimeFunction easeIn)
        {
            return new UDEMath.TimeFunction((t) => 1 - easeIn(1 - t));
        }

        /// <summary>
        /// Combine in easing function and out easing function to make inout easing function.
        /// </summary>
        /// <param name="easeIn">In easing function</param>
        /// <param name="easeOut">Out easing function</param>
        /// <returns>Inout easing function</returns>
        public static UDEMath.TimeFunction CombineInOutEase(UDEMath.TimeFunction easeIn, UDEMath.TimeFunction easeOut)
        {
            UDEMath.TimeFunction doubleInterval = new UDEMath.TimeFunction((t) => 2 * t);
            UDEMath.TimeFunction comb = new UDEMath.TimeFunction((t) =>
            {
                return t < 1 ? 0.5f * easeIn(t) : 0.5f + 0.5f * easeOut(t - 1);
            });
            return comb.Composite(doubleInterval);
        }

        /// <summary>Linear easing function.</summary>
        public static readonly UDEMath.TimeFunction easeLinear = UDEMath.GetLinear();
        /// <summary>Sine in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInSine = UDEMath.GetSine();
        /// <summary>Sine out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutSine = ConvertIn2Out(easeInSine);
        /// <summary>Sine inout easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutSine = CombineInOutEase(easeInSine, easeOutSine);
        /// <summary>Quadratic in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInQuad = UDEMath.GetQuadratic();
        /// <summary>Quadratic out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutQuad = ConvertIn2Out(easeInQuad);
        /// <summary>Quadratic inout easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutQuad = CombineInOutEase(easeInQuad, easeOutQuad);
        /// <summary>Cubic in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInCubic = UDEMath.GetCubic();
        /// <summary>Cubic out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutCubic = ConvertIn2Out(easeInCubic);
        /// <summary>Cubic inout easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutCubic = CombineInOutEase(easeInCubic, easeOutCubic);
        /// <summary>Quartic in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInQuart = new UDEMath.TimeFunction((t) => Mathf.Pow(t, 4));
        /// <summary>Quartic out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutQuart = ConvertIn2Out(easeInQuart);
        /// <summary>Quartic inout easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutQuart = CombineInOutEase(easeInQuart, easeOutQuart);
        /// <summary>Quintic in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInQuint = new UDEMath.TimeFunction((t) => Mathf.Pow(t, 5));
        /// <summary>Quintic out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutQuint = ConvertIn2Out(easeInQuint);
        /// <summary>Quintic inout easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutQuint = CombineInOutEase(easeInQuint, easeOutQuint);
        /// <summary>Exponential in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInExpo = UDEMath.GetExponential(12);
        /// <summary>Exponential out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutExpo = ConvertIn2Out(easeInExpo);
        /// <summary>Exponential in out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutExpo = CombineInOutEase(easeInExpo, easeOutExpo);
        /// <summary>Circular in easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInCirc = UDEMath.GetCircularArc();
        /// <summary>Circular out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeOutCirc = ConvertIn2Out(easeInCirc);
        /// <summary>Circular in out easing function.</summary>
        public static readonly UDEMath.TimeFunction easeInOutCirc = CombineInOutEase(easeInCirc, easeOutCirc);
        /// <summary>In easing function that once goes to backward and then frontward again.</summary>
        public static readonly UDEMath.TimeFunction easeInBack = new UDEMath.TimeFunction((t) => UDEMath.GetNaturalCubicSplineFunction((0, 0), (0.35f, -0.15f), (0.58f, 0), (1, 1))(t));
        /// <summary>Out easing function that once goes to frontward over the boundary and back to end point.</summary>
        public static readonly UDEMath.TimeFunction easeOutBack = ConvertIn2Out(easeInBack);
        /// <summary>Inout easing function that goes over lower bound and again upper bound.</summary>
        public static readonly UDEMath.TimeFunction easeInOutBack = CombineInOutEase(easeInBack, easeOutBack);
        /// <summary>In easing function that vibrates 3 times that the amplitude increases every oscillation.</summary>
        public static readonly UDEMath.TimeFunction easeInElastic = UDEMath.GetElasticOscillation(3, 9);
        /// <summary>Out easing function that vibrates 3 times that the amplitude decreases every oscillation.</summary>
        public static readonly UDEMath.TimeFunction easeOutElastic = ConvertIn2Out(easeInElastic);
        /// <summary>Inout easing function that vibrates 6 times that the amplitude increases halfway and after that decreases.</summary>
        public static readonly UDEMath.TimeFunction easeInOutElastic = CombineInOutEase(easeInElastic, easeOutElastic);
        /// <summary>In easing function that bounces at the start point 4 times.</summary>
        public static readonly UDEMath.TimeFunction easeInBounce = UDEMath.GetBounce(4);
        /// <summary>Out easing function that bounces at the end point 4 times.</summary>
        public static readonly UDEMath.TimeFunction easeOutBounce = ConvertIn2Out(easeInBounce);
        /// <summary>Inout easing function that bounces at the start point and ent point 4 times each.</summary>
        public static readonly UDEMath.TimeFunction easeInOutBounce = CombineInOutEase(easeInBounce, easeOutBounce);

        /// <summary>
        /// Enum of type of easing function.
        /// </summary>
        public enum EaseType
        {
            EaseLinear,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce
        }

        /// <summary>
        /// Gets easing function of given type.
        /// </summary>
        /// <param name="type">Type of the easing function</param>
        /// <returns>Easing function of the type</returns>
        public static UDEMath.TimeFunction EaseTypeOf(EaseType type)
        {
            switch (type)
            {
                case EaseType.EaseLinear:
                    return easeLinear;
                case EaseType.EaseInSine:
                    return easeInSine;
                case EaseType.EaseOutSine:
                    return easeOutSine;
                case EaseType.EaseInOutSine:
                    return easeInOutSine;
                case EaseType.EaseInQuad:
                    return easeInQuad;
                case EaseType.EaseOutQuad:
                    return easeOutQuad;
                case EaseType.EaseInOutQuad:
                    return easeInOutQuad;
                case EaseType.EaseInCubic:
                    return easeInCubic;
                case EaseType.EaseOutCubic:
                    return easeOutCubic;
                case EaseType.EaseInOutCubic:
                    return easeInOutCubic;
                case EaseType.EaseInQuart:
                    return easeInQuart;
                case EaseType.EaseOutQuart:
                    return easeOutQuart;
                case EaseType.EaseInOutQuart:
                    return easeInOutQuart;
                case EaseType.EaseInQuint:
                    return easeInQuint;
                case EaseType.EaseOutQuint:
                    return easeOutQuint;
                case EaseType.EaseInOutQuint:
                    return easeInOutQuint;
                case EaseType.EaseInExpo:
                    return easeInExpo;
                case EaseType.EaseOutExpo:
                    return easeOutExpo;
                case EaseType.EaseInOutExpo:
                    return easeInOutExpo;
                case EaseType.EaseInCirc:
                    return easeInCirc;
                case EaseType.EaseOutCirc:
                    return easeOutCirc;
                case EaseType.EaseInOutCirc:
                    return easeInOutCirc;
                case EaseType.EaseInBack:
                    return easeInBack;
                case EaseType.EaseOutBack:
                    return easeOutBack;
                case EaseType.EaseInOutBack:
                    return easeInOutBack;
                case EaseType.EaseInElastic:
                    return easeInElastic;
                case EaseType.EaseOutElastic:
                    return easeOutElastic;
                case EaseType.EaseInOutElastic:
                    return easeInOutElastic;
                case EaseType.EaseInBounce:
                    return easeInBounce;
                case EaseType.EaseOutBounce:
                    return easeOutBounce;
                case EaseType.EaseInOutBounce:
                    return easeInOutBounce;
                default:
                    return null;
            }
        }
        #endregion

        #region Transition Functions
        /// <summary>
        /// Move the object to x = <paramref name="destX"/> and y = <paramref name="destY"/> for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="destX">X coordinate of the final position of the object</param>
        /// <param name="destY">Y coordinate of the final position of the object</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveTo(GameObject target, float destX, float destY, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            MoveToInfo info = new MoveToInfo()
            {
                from = target.transform.position,
                to = new Vector2(destX, destY),
                duration = duration,
                easeFunction = easeFunc,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            return info.AddTo(target);
        }

        /// <summary>
        /// Move the object to x = <paramref name="destX"/> and y = <paramref name="destY"/> at constant speed <paramref name="speed"/> per second.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="destX">X coordinate of the final position of the object</param>
        /// <param name="destY">Y coordinate of the final position of the object</param>
        /// <param name="speed">Speed of the object</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveTo(GameObject target, float destX, float destY, float speed, UDETime.TimeScale timeScale, bool isPhysics)
        {
            MoveToInfo info = new MoveToInfo()
            {
                from = target.transform.position,
                to = new Vector2(destX, destY),
                easeFunction = easeLinear,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            info.duration = (info.from - info.to).magnitude / speed;
            return info.AddTo(target);
        }

        /// <summary>
        /// Move the object to (x, y) = <paramref name="dest"/> for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="dest">Coordinate of the final position of the object</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveTo(GameObject target, Vector2 dest, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            return MoveTo(target, dest.x, dest.y, duration, easeFunc, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object to (x, y) = <paramref name="dest"/> at constant speed <paramref name="speed"/> per second.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="dest">Coordinate of the final position of the object</param>
        /// <param name="speed">Speed of the object</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveTo(GameObject target, Vector2 dest, float speed, UDETime.TimeScale timeScale, bool isPhysics)
        {
            return MoveTo(target, dest.x, dest.y, speed, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object for dx = <paramref name="delX"/> and dy = <paramref name="delY"/> for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="delX">Change in x coordinate of the object</param>
        /// <param name="delY">Change in y coordinate of the object</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveAmount(GameObject target, float delX, float delY, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            Transform targetTr = target.transform;
            float destX = targetTr.position.x + delX;
            float destY = targetTr.position.y + delY;
            return MoveTo(target, destX, destY, duration, easeFunc, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object for dx = <paramref name="delX"/> and dy = <paramref name="delY"/> at constant speed <paramref name="speed"/> per second.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="delX">Change in x coordinate of the object</param>
        /// <param name="delY">Change in y coordinate of the object</param>
        /// <param name="speed">Speed of the object</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveAmount(GameObject target, float delX, float delY, float speed, UDETime.TimeScale timeScale, bool isPhysics)
        {
            Transform targetTr = target.transform;
            float destX = targetTr.position.x + delX;
            float destY = targetTr.position.y + delY;
            float duration = Mathf.Sqrt(delX * delX + delY * delY) / speed;
            return MoveTo(target, destX, destY, duration, easeLinear, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object for dr = <paramref name="delta"/> for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="delta">Change in coordinates of the object</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveAmount(GameObject target, Vector2 delta, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            return MoveAmount(target, delta.x, delta.y, duration, easeFunc, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object for dr = <paramref name="delta"/> at constant speed <paramref name="speed"/> per second.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="delta">Change in coordinates of the object</param>
        /// <param name="speed">Speed of the object</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveAmount(GameObject target, Vector2 delta, float speed, UDETime.TimeScale timeScale, bool isPhysics)
        {
            float duration = delta.magnitude / speed;
            return MoveAmount(target, delta.x, delta.y, duration, easeLinear, timeScale, isPhysics);
        }

        /// <summary>
        /// Move the object on the curve for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to move</param>
        /// <param name="curve"><see cref="SansyHuman.UDE.Util.Math.UDECurve"/> instance which is the path of the object.
        /// <para>The path of the object should be in absolute coordinate.</para></param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult MoveOnCurve(GameObject target, UDECurve curve, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            MoveOnCurveInfo info = new MoveOnCurveInfo()
            {
                curve = curve.GetFunctionOfCurve(),
                duration = duration,
                easeFunction = easeFunc,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            return info.AddTo(target);
        }

        /// <summary>
        /// Change the color of the sprite for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> that has a sprite to change the color</param>
        /// <param name="to">Color to change to</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult ChangeColorTo(GameObject target, Color to, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            Color initial = target.GetComponent<SpriteRenderer>().color;
            ColorChangeInfo info = new ColorChangeInfo()
            {
                from = initial,
                to = to,
                duration = duration,
                easeFunction = easeFunc,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            return info.AddTo(target);
        }

        /// <summary>
        /// Change the scale of the object for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to change the scale</param>
        /// <param name="to">Scale to change to</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult ChangeScaleTo(GameObject target, Vector3 to, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            Vector3 initial = target.transform.localScale;
            ScaleChangeInfo info = new ScaleChangeInfo()
            {
                from = initial,
                to = to,
                duration = duration,
                easeFunction = easeFunc,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            return info.AddTo(target);
        }

        /// <summary>
        /// Rotate the object for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to rotate</param>
        /// <param name="to">Euler angles to rotate to</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult RotateTo(GameObject target, Vector3 to, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            Vector3 initial = target.transform.rotation.eulerAngles;
            RotationInfo info = new RotationInfo()
            {
                from = initial,
                to = to,
                duration = duration,
                easeFunction = easeFunc,
                timeScale = timeScale,
                isPhysics = isPhysics
            };
            return info.AddTo(target);
        }

        /// <summary>
        /// Rotate the object for an amount of <paramref name="delta"/> angles for <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to rotate</param>
        /// <param name="delta">Changes of euler angles</param>
        /// <param name="duration">Duration of the transition in seconds</param>
        /// <param name="easeFunc">Ease function applied to the transition</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="isPhysics">If <see langword="true"/>, the transition will be calculated in FixedUpdate.
        /// Else, it will be calculated in Update.</param>
        /// <returns><see cref="TransitionResult"/> instance that contains whether the transition ended</returns>
        public static TransitionResult RotateAmount(GameObject target, Vector3 delta, float duration, UDEMath.TimeFunction easeFunc, UDETime.TimeScale timeScale, bool isPhysics)
        {
            return RotateTo(target, target.transform.rotation.eulerAngles + delta, duration, easeFunc, timeScale, isPhysics);
        }

        /// <summary>
        /// Stops all currently running transitions of the object.
        /// </summary>
        /// <param name="target"><see cref="GameObject"/> to stop transition</param>
        public static void StopAllTransitions(GameObject target)
        {
            UDETransition transition = target.GetComponent<UDETransition>();
            if (transition != null && transition.enabled)
                transition.StopAllTransition();
        }
        #endregion
    }
}