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

using System;
using Unity.Mathematics;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.UDE.ECS.Object
{
    /// <summary>
    /// ECS version of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/>.
    /// </summary>
    /// <remarks>Because of the limitation of multi-thread, this struct does not support functional form of accelerations.</remarks>
    [Serializable]
    public struct UDEBulletMovementECS
    {
        #region Basic variables
        /// <summary>Bullet's move mode. See <see cref="UDEBulletMovement.MoveMode"/>.</summary>
        public UDEBulletMovement.MoveMode mode;
        /// <summary>Time to start the movement.</summary>
        public float startTime;
        /// <summary>Time to end the movement.</summary>
        /// <remarks>It is recommended to set to 0 if <see cref="UDEBulletMovement.hasEndTime"/> is <c>false</c>.</remarks>
        public float endTime;
        /// <summary>Whether the movement has the end time.</summary>
        public bool hasEndTime;
        /// <summary>Whether there is a limit to speed of the bullet.</summary>
        public bool limitSpeed;
        /// <summary>Whether the initial speed sets to the current speed of the bullet automatically.</summary>
        public bool setSpeedToPrevMovement;
        #endregion

        /// <summary>Shows how many time the movement is updated.</summary>
        public int updateCount;

        #region Cartesian mode variables
        // Cartesian Mode
        /// <summary>The bullet's velocity in cartesian coordinate system.</summary>
        public float2 velocity;
        /// <summary>The bullet's maximum speed of each component of the velocity in cartesian coordinate system.</summary>
        public float2 maxVelocity;
        /// <summary>The bullet's minimum speed of each compoenent of the velocity in cartesian coordinate system.</summary>
        public float2 minVelocity;
        /// <summary>The bullet's maximum magnitude of the velocity vector.</summary>
        public float maxMagnitude;
        /// <summary>The bullet's minimum magnitude of the velocity vector.</summary>
        public float minMagnitude;
        /// <summary>The bullet's acceleration vector in cartesian coordinate system.</summary>
        public float2 accel;
        #endregion

        #region CartesianPolar mode variables
        // Cartesian-Polar Mode
        /// <summary>The bullet's speed in cartesian coordinate system.</summary>
        public float speed;
        /// <summary>The bullet's maximum speed in cartesian coordinate system.</summary>
        public float maxSpeed;
        /// <summary>The bullet's minimum speed in cartesian coordinate system.</summary>
        public float minSpeed;
        /// <summary>The bullet's movement angle in cartesian coordinate system.</summary>
        /// <remarks>The horizontal right direction is 0 degree.
        /// In polar mode, this variable is set to angle in polar coordinate system if the distance from origin is 0.</remarks>
        public float angle;
        /// <summary>The bullet's acceleration in the direction of the movement.</summary>
        public float tangentialAccel;
        /// <summary>The bullet's acceleration perpendicular to the movement.</summary>
        /// <remarks>It is actually the change of the <see cref="UDEBulletMovementECS.angle"/></remarks>
        public float normalAccel;
        #endregion

        #region Polar mode variables
        //PolarMode
        /// <summary>The bullet's radial speed in polar coordinate system.</summary>
        public float radialSpeed;
        /// <summary>The bullet's angular speed in polar coordinate system.</summary>
        public float angularSpeed;
        /// <summary>The bullet's maximum radial speed in polar coordinate system.</summary>
        public float maxRadialSpeed;
        /// <summary>The bullet's maximum angular speed in polar coordinate system.</summary>
        public float maxAngularSpeed;
        /// <summary>The bullet's minimum radial speed in polar coordinate system.</summary>
        public float minRadialSpeed;
        /// <summary>The bullet's minimum angular speed in polar coordinate system.</summary>
        public float minAngularSpeed;
        /// <summary>The bullet's radial acceleration in polar coordinate system.</summary>
        public float radialAccel;
        /// <summary>The bullet's angular acceleration in polar coordinate system.</summary>
        public float angularAccel;
        #endregion

        #region Rotation variables
        /// <summary>Shows if the bullet automatically rotate to the direction of the movement.</summary>
        public bool faceToMovingDirection;
        /// <summary>Angular spped of bullet's rotation.</summary>
        public float rotationAngularSpeed;
        /// <summary>Angular acceleration of bullet's rotation.</summary>
        public float rotationAngularAcceleration;
        /// <summary>Shows if bullet's rotation speed has a limit.</summary>
        public bool limitRotationSpeed;
        /// <summary>Minimum rotation speed of the bullet.</summary>
        public float minRotationSpeed;
        /// <summary>Maximum rotation speed of the bullet.</summary>
        public float maxRotationSpeed;
        #endregion

        /// <summary>
        /// The basic constructor of the struct.
        /// </summary>
        /// <param name="mode">Move mode</param>
        /// <param name="startTime">Start time to move</param>
        /// <param name="endTime">End time to move</param>
        /// <param name="hasEndTime"><see langword="true"/> if it has an end time</param>
        /// <param name="limitSpeed"><see langword="true"/> if there is a speed limit</param>
        /// <param name="setSpeedToPrevMovement"><see langword="true"/> if the initial speed sets to the current speed of the bullet automatically.</param>
        /// <param name="velocity">Velocity vector in cartesian coordinate system</param>
        /// <param name="maxVelocity">Maximum speed of each component of the velocity in cartesian coordinate system</param>
        /// <param name="minVelocity">Minimum speed of each component of the velocity in cartesian coordinate system</param>
        /// <param name="maxMagnitude">Maximum magnitude of velocity vector in cartesian coordinate system</param>
        /// <param name="minMagnitude">Minimum magnitude of velocity vector in cartesian coordinate system</param>
        /// <param name="accel">Acceleration vector in cartesian coordinate system</param>
        /// <param name="speed">Speed in cartesian coordinate system</param>
        /// <param name="maxSpeed">Maximum speed in cartesian coordinate system</param>
        /// <param name="minSpeed">Minimum speed in cartesian coordinate system</param>
        /// <param name="angle">Angle to move</param>
        /// <param name="tangentialAccel">Tangential acceleration in cartesian coordinate system</param>
        /// <param name="normalAccel">Normal acceleration in cartesian coordinate system</param>
        /// <param name="radialSpeed">Radial speed in polar coordinate system</param>
        /// <param name="angularSpeed">Angular speed in polar coordinate system</param>
        /// <param name="maxRadialSpeed">Maximum radial speed in polar coordinate system</param>
        /// <param name="maxAngularSpeed">Maximum angular speed in polar coordinate system</param>
        /// <param name="minRadialSpeed">Minimum radial speed in polar coordinate system</param>
        /// <param name="minAngularSpeed">Minimum angular speed in polar coordinate system</param>
        /// <param name="radialAccel">Radial acceleration in polar coordinate system</param>
        /// <param name="angularAccel">Angular acceleration in polar coordinate system</param>
        /// <param name="faceToMovingDirection"><see langword="true"/> if the bullet rotates toward the moving direction</param>
        /// <param name="rotationAngularSpeed">Bullel's speed of rotation</param>
        /// <param name="rotationAngularAcceleration">Bullet's acceleration of rotation</param>
        /// <param name="limitRotationSpeed"><see langword="true"/> if there is a limit of rotation speed</param>
        /// <param name="minRotationSpeed">Minimum speed of rotation</param>
        /// <param name="maxRotationSpeed">Maximum speed of rotation</param>
        public UDEBulletMovementECS(UDEBulletMovement.MoveMode mode,
                                    float startTime,
                                    float endTime,
                                    bool hasEndTime,
                                    bool limitSpeed,
                                    bool setSpeedToPrevMovement,
                                    float2 velocity,
                                    float2 maxVelocity,
                                    float2 minVelocity,
                                    float maxMagnitude,
                                    float minMagnitude,
                                    float2 accel,
                                    float speed,
                                    float maxSpeed,
                                    float minSpeed,
                                    float angle,
                                    float tangentialAccel,
                                    float normalAccel,
                                    float radialSpeed,
                                    float angularSpeed,
                                    float maxRadialSpeed,
                                    float maxAngularSpeed,
                                    float minRadialSpeed,
                                    float minAngularSpeed,
                                    float radialAccel,
                                    float angularAccel,
                                    bool faceToMovingDirection,
                                    float rotationAngularSpeed,
                                    float rotationAngularAcceleration,
                                    bool limitRotationSpeed,
                                    float minRotationSpeed,
                                    float maxRotationSpeed)
        {
            this.mode = mode;
            this.startTime = startTime;
            this.endTime = endTime;
            this.hasEndTime = hasEndTime;
            this.limitSpeed = limitSpeed;
            this.setSpeedToPrevMovement = setSpeedToPrevMovement;
            updateCount = 0;
            this.velocity = velocity;
            this.maxVelocity = maxVelocity;
            this.minVelocity = minVelocity;
            this.maxMagnitude = maxMagnitude;
            this.minMagnitude = minMagnitude;
            this.accel = accel;
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.minSpeed = minSpeed;
            this.angle = angle;
            this.tangentialAccel = tangentialAccel;
            this.normalAccel = normalAccel;
            this.radialSpeed = radialSpeed;
            this.angularSpeed = angularSpeed;
            this.maxRadialSpeed = maxRadialSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.minRadialSpeed = minRadialSpeed;
            this.minAngularSpeed = minAngularSpeed;
            this.radialAccel = radialAccel;
            this.angularAccel = angularAccel;
            this.faceToMovingDirection = faceToMovingDirection;
            this.rotationAngularSpeed = rotationAngularSpeed;
            this.rotationAngularAcceleration = rotationAngularAcceleration;
            this.limitRotationSpeed = limitRotationSpeed;
            this.minRotationSpeed = minRotationSpeed;
            this.maxRotationSpeed = maxRotationSpeed;
        }

        /// <summary>
        /// Explicit type conversion to <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/>.
        /// </summary>
        /// <param name="movement"><see cref="UDEBulletMovementECS"/> to convert</param>
        public static explicit operator UDEBulletMovement(UDEBulletMovementECS movement)
        {
            return new UDEBulletMovement(movement.mode, movement.startTime, movement.endTime, movement.hasEndTime, movement.limitSpeed,
                movement.setSpeedToPrevMovement, movement.velocity, movement.maxVelocity, movement.minVelocity, movement.maxMagnitude, movement.minMagnitude,
                t => new UDEMath.CartesianCoord(movement.accel), movement.speed, movement.maxSpeed, movement.minSpeed, movement.angle,
                t => movement.tangentialAccel, t => movement.normalAccel, movement.radialSpeed, movement.angularSpeed,
                movement.maxRadialSpeed, movement.maxAngularSpeed, movement.minRadialSpeed, movement.minAngularSpeed,
                t => movement.radialAccel, t => movement.angularAccel, movement.faceToMovingDirection,
                movement.rotationAngularSpeed, t => movement.rotationAngularAcceleration, movement.limitRotationSpeed,
                movement.minRotationSpeed, movement.maxRotationSpeed);
        }
    }
}