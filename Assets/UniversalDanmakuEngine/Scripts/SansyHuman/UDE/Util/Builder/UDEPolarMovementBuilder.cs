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
    /// Builder of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance in polar mode.
    /// </summary>
    public class UDEPolarMovementBuilder : UDECommonBulletMovementBuilder<UDEPolarMovementBuilder>
    {
        private UDEPolarMovementBuilder(bool setSpeedToPrevMovement = false) : base(setSpeedToPrevMovement) { }

        /// <summary>
        /// Creates the builder.
        /// </summary>
        /// <returns><see cref="UDEPolarMovementBuilder"/> instance</returns>
        public static UDEPolarMovementBuilder Create(bool setSpeedToPrevMovement = false)
        {
            UDEPolarMovementBuilder builder = new UDEPolarMovementBuilder(setSpeedToPrevMovement);
            builder.mode = UDEBulletMovement.MoveMode.POLAR;
            return builder;
        }

        #region Polar variable setters
        /// <summary>
        /// Sets the initial angle in polar coordinate system if initial distance from the origin is 0.
        /// </summary>
        /// <param name="angle">Initial angle in polar coordinates</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder InitialAngle(float angle)
        {
            this.angle = angle;
            return this;
        }

        /// <summary>
        /// Sets the radial speed of the bullet.
        /// </summary>
        /// <param name="radialSpeed">Radial speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder RadialSpeed(float radialSpeed)
        {
            this.radialSpeed = radialSpeed;
            return this;
        }

        /// <summary>
        /// Sets the angular speed of the bullet.
        /// </summary>
        /// <param name="angularSpeed">Angular speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder AngularSpeed(float angularSpeed)
        {
            this.angularSpeed = angularSpeed;
            return this;
        }

        /// <summary>
        /// Sets the maximum radial speed and sets the variable <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="maxRadialSpeed">Maximum radial speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder MaxRadialSpeed(float maxRadialSpeed)
        {
            this.maxRadialSpeed = maxRadialSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the maximum angular speed and sets the variable <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="maxAngularSpeed">Maximum angular speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder MaxAngularSpeed(float maxAngularSpeed)
        {
            this.maxAngularSpeed = maxAngularSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the minimum radial speed and sets the variable <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="minRadialSpeed">Minimum radial speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder MinRadialSpeed(float minRadialSpeed)
        {
            this.minRadialSpeed = minRadialSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the minimum angular speed and sets the variable <c>limitSpeed</c> true.
        /// </summary>
        /// <param name="minAngularSpeed">Minimum angular speed</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder MinAngularSpeed(float minAngularSpeed)
        {
            this.minAngularSpeed = minAngularSpeed;
            this.limitSpeed = true;
            return this;
        }

        /// <summary>
        /// Sets the radial acceleration.
        /// </summary>
        /// <param name="radialAccel">Radial acceleration</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder RadialAccel(float radialAccel)
        {
            this.radialAccel = t => radialAccel;
            return this;
        }

        /// <summary>
        /// Sets the radial acceleration.
        /// </summary>
        /// <param name="radialAccel">Radial acceleration by time</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder RadialAccel(TimeFunction radialAccel)
        {
            this.radialAccel = radialAccel;
            return this;
        }

        /// <summary>
        /// Sets the anguler acceleration.
        /// </summary>
        /// <param name="angularAccel">Angular acceleration</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder AngularAccel(float angularAccel)
        {
            this.angularAccel = t => angularAccel;
            return this;
        }

        /// <summary>
        /// Sets the anguler acceleration.
        /// </summary>
        /// <param name="angularAccel">Angular acceleration by time</param>
        /// <returns>Itself</returns>
        public UDEPolarMovementBuilder AngularAccel(TimeFunction angularAccel)
        {
            this.angularAccel = angularAccel;
            return this;
        }
        #endregion

        /// <summary>
        /// Override method of <see cref="SansyHuman.UDE.Util.Builder.UDECommonBulletMovementBuilder{UDEPolarMovementBuilder}.Build"/>.
        /// </summary>
        /// <returns><see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance.</returns>
        /// <exception cref="SansyHuman.UDE.Exception.UDEInvalidBulletMovementException">
        /// Thrown when bullet's minimum speed is greater than maximum speed
        /// </exception>
        public override UDEBulletMovement Build()
        {
            if (this.limitSpeed)
            {
                if (this.minRadialSpeed > this.maxRadialSpeed)
                    throw new UDEInvalidBulletMovementException("Bullet's minimum radial speed is greater than maximum radial speed.");

                if (this.minAngularSpeed > this.maxAngularSpeed)
                    throw new UDEInvalidBulletMovementException("Bullet's minimum angular speed is greater than maximum angular speed.");
            }
            return base.Build();
        }
    }
}