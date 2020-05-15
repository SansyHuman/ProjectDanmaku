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

using SansyHuman.UDE.Object;
using SansyHuman.UDE.Exception;
using static SansyHuman.UDE.Util.Math.UDEMath;

namespace SansyHuman.UDE.Util.Builder
{
    /// <summary>
    /// Builder of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance in cartesian-polar mode.
    /// </summary>
    public sealed class UDECartesianPolarMovementBuilder : UDECommonBulletMovementBuilder<UDECartesianPolarMovementBuilder>
    {
        private UDECartesianPolarMovementBuilder(bool setSpeedToPrevMovement = false) : base(setSpeedToPrevMovement) { }

        /// <summary>
        /// Creates the builder.
        /// </summary>
        /// <returns><see cref="UDECartesianPolarMovementBuilder"/> instance</returns>
        public static UDECartesianPolarMovementBuilder Create(bool setSpeedToPrevMovement = false)
        {
            UDECartesianPolarMovementBuilder builder = new UDECartesianPolarMovementBuilder(setSpeedToPrevMovement);
            builder.mode = UDEBulletMovement.MoveMode.CARTESIAN_POLAR;
            return builder;
        }

        #region CartesianPolar variable setters
        /// <summary>
        /// Sets the speed of the bullet.
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder Speed(float speed)
        {
            this.speed = speed;
            return this;
        }

        /// <summary>
        /// Sets the maximum speed and sets the variable <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="maxSpeed">Maximum speed</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder MaxSpeed(float maxSpeed)
        {
            this.maxSpeed = maxSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the minimum speed and sets the variabld <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="minSpeed">Minimum speed</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder MinSpeed(float minSpeed)
        {
            this.minSpeed = minSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the moving angle of the bullet.
        /// </summary>
        /// <param name="angle">Moving angle</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder Angle(float angle)
        {
            this.angle = angle;
            return this;
        }

        /// <summary>
        /// Sets the tangential acceleration.
        /// </summary>
        /// <param name="tangentialAccel">Tangential acceleration</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder TangentialAccel(float tangentialAccel)
        {
            this.tangentialAccel = t => tangentialAccel;
            return this;
        }

        /// <summary>
        /// Sets the tangential acceleration.
        /// </summary>
        /// <param name="tangentialAccel">Tangential acceleration by time</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder TangentialAccel(TimeFunction tangentialAccel)
        {
            this.tangentialAccel = tangentialAccel;
            return this;
        }

        /// <summary>
        /// Sets the normal acceleration.
        /// </summary>
        /// <param name="normalAccel">Normal acceleration</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder NormalAccel(float normalAccel)
        {
            this.normalAccel = t => normalAccel;
            return this;
        }

        /// <summary>
        /// Sets the normal acceleration.
        /// </summary>
        /// <param name="normalAccel">Normal acceleration by time</param>
        /// <returns>Itself</returns>
        public UDECartesianPolarMovementBuilder NormalAccel(TimeFunction normalAccel)
        {
            this.normalAccel = normalAccel;
            return this;
        }
        #endregion

        /// <summary>
        /// Override method of <see cref="SansyHuman.UDE.Util.Builder.UDECommonBulletMovementBuilder{UDECartesianMovementBuilder}.Build()"/>.
        /// </summary>
        /// <returns><see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance.</returns>
        /// <exception cref="SansyHuman.UDE.Exception.UDEInvalidBulletMovementException">
        /// Thrown when bullet's minimum speed is greater than maximum speed
        /// </exception>
        public override UDEBulletMovement Build()
        {
            if (this.limitSpeed)
            {
                if (this.minSpeed > this.maxSpeed)
                    throw new UDEInvalidBulletMovementException("Bullet's minimum speed is greater than maximum speed.");
            }
            return base.Build();
        }
    }
}