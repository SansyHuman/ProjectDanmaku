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

using UnityEngine;
using SansyHuman.UDE.Management;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Class of laser which extends along a straight line.
    /// </summary>
    [AddComponentMenu("UDE/Laser/Straight")]
    public class UDEStraightLaser : UDELaser
    {
        private Vector2 headVelocity; // Velocity of the head.

        private float maxLaserLength; // Maximum length of the laser.

        private Vector2 maxHeadLocation; // Maximum location of the head.

        /// <summary>
        /// Initializes the laser.
        /// </summary>
        /// <param name="origin">Origin of the local position of the laser in world space</param>
        /// <param name="originCharacter">Character who fired the laser</param>
        /// <param name="followOriginCharacter">If true, changes the origin of the laser to origin character's position</param>
        /// <param name="initialLocalHeadLocation">Start position of the laser in local space</param>
        /// <param name="timeScale">Time scale to use</param>
        /// <param name="headVelocity">Velocity of the head</param>
        /// <param name="maxLaserLength">Maximum length of the laser</param>
        public void Initialize(Vector2 origin,
                               UDEBaseCharacter originCharacter,
                               bool followOriginCharacter,
                               Vector2 initialLocalHeadLocation,
                               UDETime.TimeScale timeScale,
                               Vector2 headVelocity,
                               float maxLaserLength)
        {
            if (initialized)
            {
                Debug.LogWarning("You tried to initialize laser that already initialized. Initialization is ignored.");
                return;
            }

            Initialize(origin, originCharacter, followOriginCharacter, initialLocalHeadLocation, timeScale);
            points.Add(initialLocalHeadLocation);
            laser.positionCount = 2;
            laser.SetPosition(1, points[1]);
            laserCollider.points = points.ToArray();

            this.headVelocity = headVelocity;
            this.maxLaserLength = maxLaserLength;
            maxHeadLocation = initialLocalHeadLocation + headVelocity * maxLaserLength / headVelocity.magnitude;
        }

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDELaser.ExtendLaser(float)"/>.
        /// </summary>
        /// <param name="deltaTime">Delta time from the last call</param>
        protected override void ExtendLaser(float deltaTime)
        {
            base.ExtendLaser(deltaTime);
            float scaledDeltaTime = deltaTime * UDETime.Instance.GetTimeScale(timeScale);
            time += scaledDeltaTime;
            if (points[1] == maxHeadLocation)
                return;

            points[1] += headVelocity * scaledDeltaTime;
            if ((points[1] - points[0]).magnitude > maxLaserLength)
                points[1] = maxHeadLocation;

            laser.SetPosition(1, points[1]);
            laserCollider.points = points.ToArray();
        }
    }
}