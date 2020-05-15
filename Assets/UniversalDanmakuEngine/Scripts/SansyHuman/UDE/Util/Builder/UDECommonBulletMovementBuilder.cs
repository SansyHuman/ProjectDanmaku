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
using UnityEngine;
using static SansyHuman.UDE.Util.Math.UDEMath;

namespace SansyHuman.UDE.Util.Builder
{
    /// <summary>
    /// Base class of all builder of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/>. Contains common variable setters.
    /// <para>All setters returns itself.</para>
    /// </summary>
    /// <typeparam name="T">A type that inherits <see cref="SansyHuman.UDE.Util.Builder.UDECommonBulletMovementBuilder{T}"/>.</typeparam>
    /// <seealso cref="SansyHuman.UDE.Object.UDEBulletMovement"/>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDECartesianMovementBuilder"/>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDECartesianPolarMovementBuilder"/>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDEPolarMovementBuilder"/>
    public abstract class UDECommonBulletMovementBuilder<T> where T : UDECommonBulletMovementBuilder<T>
    {
        #region Basic variables
        protected UDEBulletMovement.MoveMode mode;
        protected float startTime = 0;
        protected float endTime = 0;
        protected bool hasEndTime = false;
        protected bool limitSpeed = false;
        protected bool setSpeedToPrevMovement = false;
        #endregion

        #region Cartesian variables
        protected Vector2 velocity = Vector2.zero;
        protected Vector2 maxVelocity = new Vector2(float.MaxValue, float.MaxValue);
        protected Vector2 minVelocity = new Vector2(float.MinValue, float.MinValue);
        protected float maxMagnitude = float.MaxValue;
        protected float minMagnitude = float.MinValue;
        protected CartesianTimeFunction accel = zeroCartesianTimeFunction;
        #endregion

        #region CartesianPolar mode variables
        protected float speed = 0;
        protected float maxSpeed = float.MaxValue;
        protected float minSpeed = float.MinValue;
        protected float angle = 0;
        protected TimeFunction tangentialAccel = zeroTimeFunction;
        protected TimeFunction normalAccel = zeroTimeFunction;
        #endregion

        #region Polar mode variables
        protected float radialSpeed = 0;
        protected float angularSpeed = 0;
        protected float maxRadialSpeed = float.MaxValue;
        protected float maxAngularSpeed = float.MaxValue;
        protected float minRadialSpeed = float.MinValue;
        protected float minAngularSpeed = float.MinValue;
        protected TimeFunction radialAccel = zeroTimeFunction;
        protected TimeFunction angularAccel = zeroTimeFunction;
        #endregion

        #region Rotation variables
        protected bool faceToMovingDirection = true;
        protected float rotationAngularSpeed = 0;
        protected TimeFunction rotationAngularAcceleration = zeroTimeFunction;
        protected bool limitRotationSpeed = false;
        protected float minRotationSpeed = float.MinValue;
        protected float maxRotationSpeed = float.MaxValue;
        #endregion

        protected UDECommonBulletMovementBuilder(bool setSpeedToPrevMovement = false)
        {
            this.setSpeedToPrevMovement = setSpeedToPrevMovement;
        }

        #region Common variable setters
        /// <summary>
        /// Sets the start time.
        /// </summary>
        /// <param name="startTime">Start time</param>
        /// <returns>Itself</returns>
        public T StartTime(float startTime)
        {
            this.startTime = startTime;
            return this as T;
        }

        /// <summary>
        /// Sets the end time and sets the variable <c>hasEndTime</c> true.
        /// </summary>
        /// <param name="endTime">End time</param>
        /// <returns>Itself</returns>
        public T EndTime(float endTime)
        {
            this.endTime = endTime;
            this.hasEndTime = true;
            return this as T;
        }

        /// <summary>
        /// Sets not to use the end time. Sets the variable <c>hasEndTime</c> false.
        /// </summary>
        /// <returns>Itself</returns>
        public T DoNotUseEndTime()
        {
            this.hasEndTime = false;
            return this as T;
        }

        /// <summary>
        /// Sets not to use the speed limit. Sets the variable <c>limitSpeed</c> false.
        /// </summary>
        /// <returns>Itself</returns>
        public T DoNotUseSpeedLimit()
        {
            this.limitSpeed = false;
            return this as T;
        }

        /// <summary>
        /// Sets not to automatically rotate the bullet to the direction of movement. Sets the variable <c>faceToMovingDirection</c> false.
        /// </summary>
        /// <returns>Itself</returns>
        public T DoNotFaceToMovingDirection()
        {
            this.faceToMovingDirection = false;
            return this as T;
        }

        /// <summary>
        /// Sets the rotation speed of the bullet.
        /// </summary>
        /// <param name="rotationAngularSpeed">Rotation angular speed</param>
        /// <returns>Itself</returns>
        public T RotationAngularSpeed(float rotationAngularSpeed)
        {
            this.rotationAngularSpeed = rotationAngularSpeed;
            return this as T;
        }

        /// <summary>
        /// Sets the rotation acceleration of the bullet.
        /// </summary>
        /// <param name="rotationAngularAcceleration">Rotation angular acceleration</param>
        /// <returns>Itself</returns>
        public T RotationAngularAcceleration(float rotationAngularAcceleration)
        {
            this.rotationAngularAcceleration = new TimeFunction(t => rotationAngularAcceleration);
            return this as T;
        }

        /// <summary>
        /// Sets the rotation acceleration of the bullet.
        /// </summary>
        /// <param name="rotationAngularAcceleration">Rotation angular acceleration by time</param>
        /// <returns>Itself</returns>
        public T RotationAngularAcceleration(TimeFunction rotationAngularAcceleration)
        {
            this.rotationAngularAcceleration = rotationAngularAcceleration;
            return this as T;
        }

        /// <summary>
        /// Sets the minimum rotation speed of the bullet and sets the variable <c>limitRotationSpeed</c> true.
        /// </summary>
        /// <param name="minRotationSpeed">Minimum rotation speed</param>
        /// <returns>Itself</returns>
        public T MinRotationSpeed(float minRotationSpeed)
        {
            this.minRotationSpeed = minRotationSpeed;
            this.limitRotationSpeed = true;
            return this as T;
        }

        /// <summary>
        /// Sets the maximum rotation speed of the bullet and sets the variable <c>limitRotationSpeed</c> true.
        /// </summary>
        /// <param name="maxRotationSpeed">Maximum rotation speed</param>
        /// <returns>Itself</returns>
        public T MaxRotationSpeed(float maxRotationSpeed)
        {
            this.maxRotationSpeed = maxRotationSpeed;
            this.limitRotationSpeed = true;
            return this as T;
        }

        /// <summary>
        /// Sets not to use rotation speed limit. Sets the variable <c>limitRotationSpeed</c> false.
        /// </summary>
        /// <returns>Itself</returns>
        public T DoNotUseRotationSpeedLimit()
        {
            this.limitRotationSpeed = false;
            return this as T;
        }
        #endregion

        /// <summary>
        /// Build an instance of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/>.
        /// </summary>
        /// <returns><see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance.</returns>
        /// <exception cref="SansyHuman.UDE.Exception.UDEInvalidBulletMovementException">
        /// Thrown when bullet's minimum speed is greater than maximum speed
        /// </exception>
        public virtual UDEBulletMovement Build()
        {
            if (this.limitRotationSpeed)
            {
                if (this.minRotationSpeed > this.maxRotationSpeed)
                    throw new UDEInvalidBulletMovementException("Bullet's minimum rotation speed is greater than maximum rotation speed");
            }
            return new UDEBulletMovement(mode,
                                         startTime,
                                         endTime,
                                         hasEndTime,
                                         limitSpeed,
                                         setSpeedToPrevMovement,
                                         velocity,
                                         maxVelocity,
                                         minVelocity,
                                         maxMagnitude,
                                         minMagnitude,
                                         accel,
                                         speed,
                                         maxSpeed,
                                         minSpeed,
                                         angle,
                                         tangentialAccel,
                                         normalAccel,
                                         radialSpeed,
                                         angularSpeed,
                                         maxRadialSpeed,
                                         maxAngularSpeed,
                                         minRadialSpeed,
                                         minAngularSpeed,
                                         radialAccel,
                                         angularAccel,
                                         faceToMovingDirection,
                                         rotationAngularSpeed,
                                         rotationAngularAcceleration,
                                         limitRotationSpeed,
                                         minRotationSpeed,
                                         maxRotationSpeed);
        }
    }
}