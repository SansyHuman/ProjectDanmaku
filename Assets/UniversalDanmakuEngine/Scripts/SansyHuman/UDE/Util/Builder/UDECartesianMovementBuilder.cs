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

using SansyHuman.UDE.Exception;
using SansyHuman.UDE.Object;
using UnityEngine;
using static SansyHuman.UDE.Util.Math.UDEMath;

namespace SansyHuman.UDE.Util.Builder
{
    /// <summary>
    /// Builder of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance in cartesian mode.
    /// </summary>
    public sealed class UDECartesianMovementBuilder : UDECommonBulletMovementBuilder<UDECartesianMovementBuilder>
    {
        private UDECartesianMovementBuilder(bool setSpeedToPrevMovement = false) : base(setSpeedToPrevMovement) { }

        /// <summary>
        /// Creates the builder.
        /// </summary>
        /// <returns><see cref="UDECartesianMovementBuilder"/> instance</returns>
        public static UDECartesianMovementBuilder Create(bool setSpeedToPrevMovement = false)
        {
            UDECartesianMovementBuilder builder = new UDECartesianMovementBuilder(setSpeedToPrevMovement);
            builder.mode = Object.UDEBulletMovement.MoveMode.CARTESIAN;
            return builder;
        }

        #region Cartesian variable setters
        public UDECartesianMovementBuilder Velocity(Vector2 velocity)
        {
            this.velocity = velocity;
            return this;
        }

        public UDECartesianMovementBuilder MaxVelocity(Vector2 maxVelocity)
        {
            this.maxVelocity = maxVelocity;
            this.limitSpeed = true;
            return this;
        }

        public UDECartesianMovementBuilder MinVelocity(Vector2 minVelocity)
        {
            this.minVelocity = minVelocity;
            this.limitSpeed = true;
            return this;
        }

        public UDECartesianMovementBuilder MaxMagnitude(float maxMagnitude)
        {
            this.maxMagnitude = maxMagnitude;
            this.limitSpeed = true;
            return this;
        }

        public UDECartesianMovementBuilder MinMagnitude(float minMagnitude)
        {
            this.minMagnitude = minMagnitude;
            this.limitSpeed = true;
            return this;
        }

        public UDECartesianMovementBuilder Accel(CartesianCoord accel)
        {
            this.accel = t => accel;
            return this;
        }

        public UDECartesianMovementBuilder Accel(CartesianTimeFunction accel)
        {
            this.accel = accel;
            return this;
        }

        public override UDEBulletMovement Build()
        {
            if (this.minVelocity.x > this.maxVelocity.x || this.minVelocity.y > this.maxVelocity.y)
                throw new UDEInvalidBulletMovementException("One of the bullet's component of minimum velocity is greater than that of maximum velocity.");
            if (this.minMagnitude > this.maxMagnitude)
                throw new UDEInvalidBulletMovementException("Minimum magnitude of bullet's velocity is greater than Maximum magnitude of it.");
            return base.Build();
        }
        #endregion
    }
}