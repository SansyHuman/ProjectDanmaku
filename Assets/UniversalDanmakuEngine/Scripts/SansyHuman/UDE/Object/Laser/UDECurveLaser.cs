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

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Class of lasers which extend along a curve.
    /// <para>The function of the path of the laser should return the relative coordinate from the origin of the laser.</para>
    /// </summary>
    [AddComponentMenu("UDE/Laser/Curve")]
    public class UDECurveLaser : UDELaser
    {
        [SerializeField] private float headDuration; // Duration of the head.
        [SerializeField] private float pointDuration; // Duration of each point.

        private List<float> pointAddTimes; // Time when points are added.

        /// <summary>
        /// Enum of mode of the movement of the laser head.
        /// <list type="table">
        /// <item>
        /// <term><see cref="CARTESIAN_FUNCTION"/></term>
        /// <description>Expresses the path of the laser with cartesian function.</description>
        /// </item>
        /// <item>
        /// <term><see cref="POLAR_FUNCTION"/></term>
        /// <description>Expresses the path of the laser with polar function.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum HeadMoveMode
        {
            /// <summary>Head moves along line of <see cref="SansyHuman.UDE.Util.Math.UDEMath.CartesianTimeFunction"/>.</summary>
            CARTESIAN_FUNCTION,
            /// <summary>Head moves along line of <see cref="SansyHuman.UDE.Util.Math.UDEMath.PolarTimeFunction"/>.</summary>
            POLAR_FUNCTION
        }

        private HeadMoveMode mode; // Movement mode of the laser head.

        private UDEMath.CartesianTimeFunction cartesianHeadFunction; // Cartesian line function
        private UDEMath.PolarTimeFunction polarHeadFunction; // Polar line function

        private void InitializeInternal(float headDuration, float pointDuration)
        {
            this.headDuration = headDuration;
            this.pointDuration = pointDuration;
            pointAddTimes = new List<float>(128);
            pointAddTimes.Add(0);
            laserHeadTr = laserHead.transform;
        }

        /// <summary>
        /// Initializes the laser.
        /// </summary>
        /// <param name="origin">Origin of the local position of the laser in world space</param>
        /// <param name="originCharacter">Character who fired the laser</param>
        /// <param name="followOriginCharacter">If true, changes the origin of the laser to origin character's position</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="headDuration">Duration of the head. After the duration, the laser no longer extends</param>
        /// <param name="pointDuration">Duration of each point. After the duration, the point is removed from the laser's positions</param>
        /// <param name="headFunction"><see cref="SansyHuman.UDE.Util.Math.UDEMath.CartesianTimeFunction"/> that draws a curve.
        /// The coordinate of the curve should be the relative coordinate from the <paramref name="origin"/></param>
        public void Initialize(Vector2 origin,
                               UDEBaseCharacter originCharacter,
                               bool followOriginCharacter,
                               UDETime.TimeScale timeScale,
                               float headDuration,
                               float pointDuration,
                               UDEMath.CartesianTimeFunction headFunction)
        {
            if (initialized)
            {
                Debug.LogWarning("You tried to initialize laser that already initialized. Initialization is ignored.");
                return;
            }

            Initialize(origin, originCharacter, followOriginCharacter, headFunction(0), timeScale);
            InitializeInternal(headDuration, pointDuration);
            cartesianHeadFunction = headFunction;
            mode = HeadMoveMode.CARTESIAN_FUNCTION;
        }

        /// <summary>
        /// Initializes the laser.
        /// </summary>
        /// <param name="origin">Origin of the local position of the laser in world space</param>
        /// <param name="originCharacter">Character who fired the laser</param>
        /// <param name="followOriginCharacter">If true, changes the origin of the laser to origin character's position</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="headDuration">Duration of the head. After the duration, the laser no longer extends</param>
        /// <param name="pointDuration">Duration of each point. After the duration, the point is removed from the laser's positions</param>
        /// <param name="headFunction"><see cref="SansyHuman.UDE.Util.Math.UDEMath.PolarTimeFunction"/> that draws a curve.
        /// The coordinate of the curve should be the relative coordinate from the <paramref name="origin"/></param>
        public void Initialize(Vector2 origin,
                               UDEBaseCharacter originCharacter,
                               bool followOriginCharacter,
                               UDETime.TimeScale timeScale,
                               float headDuration,
                               float pointDuration,
                               UDEMath.PolarTimeFunction headFunction)
        {
            Initialize(origin, originCharacter, followOriginCharacter, (UDEMath.CartesianCoord)headFunction(0), timeScale);
            InitializeInternal(headDuration, pointDuration);
            polarHeadFunction = headFunction;
            mode = HeadMoveMode.POLAR_FUNCTION;
        }

        private Transform laserHeadTr; // Transform of the laser head.

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDELaser.ExtendLaser(float)"/>.
        /// </summary>
        /// <param name="deltaTime">Delta time from the last call</param>
        protected override void ExtendLaser(float deltaTime)
        {
            base.ExtendLaser(deltaTime);

            float scaledDeltaTime = deltaTime * UDETime.Instance.GetTimeScale(timeScale);
            time += scaledDeltaTime;

            if (laserHead == null)
                goto PointsCheck;

            switch (mode)
            {
                case HeadMoveMode.CARTESIAN_FUNCTION:
                    laserHeadTr.localPosition = cartesianHeadFunction(time).ToVector2();
                    break;
                case HeadMoveMode.POLAR_FUNCTION:
                    laserHeadTr.localPosition = ((UDEMath.CartesianCoord)polarHeadFunction(time)).ToVector2();
                    break;
            }

            if ((Vector2)laserHeadTr.localPosition == points[points.Count - 1])
                goto PointsCheck;

            points.Add(laserHeadTr.localPosition);
            pointAddTimes.Add(time);

            PointsCheck:
            for (int i = 0; i < points.Count; i++)
            {
                if (time - pointAddTimes[i] > pointDuration)
                {
                    points.RemoveAt(i);
                    pointAddTimes.RemoveAt(i);
                    i--;
                }
                else
                    break;
            }

            laser.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
                laser.SetPosition(i, points[i]);
            laserCollider.points = points.ToArray();

            if (time > headDuration)
            {
                Destroy(laserHead);
                laserHead = null;
            }

            if (points.Count == 0)
                DestroyLaser();
        }
    }
}