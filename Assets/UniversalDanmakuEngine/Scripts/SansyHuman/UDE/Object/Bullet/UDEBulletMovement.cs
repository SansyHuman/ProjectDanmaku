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
using System.Text;
using SansyHuman.UDE.ECS.Object;
using UnityEngine;
using static SansyHuman.UDE.Util.Math.UDEMath;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Struct to save the movement of the bullet. All time units are seconds and all angle units are degrees.
    /// </summary>
    [Serializable]
    public struct UDEBulletMovement
    {
        /// <summary>
        /// Enum of bullet's move.
        /// <list type="table">
        /// <item>
        /// <term><see cref="CARTESIAN"/></term>
        /// <description>Expresses the velocity 2-dimensional vector.</description>
        /// </item>
        /// <item>
        /// <term><see cref="CARTESIAN_POLAR"/></term>
        /// <description>Expresses the velocity with speed and angle.</description>
        /// </item>
        /// <item>
        /// <term><see cref="POLAR"/></term>
        /// <description>Expresses the velocity with radial and angular speed.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum MoveMode
        {
            /// <summary>Mode that expresses the velocity with a 2-dimensional vector.</summary>
            CARTESIAN,
            /// <summary>Mode that expresses the velocity with speed and angle.</summary>
            CARTESIAN_POLAR,
            /// <summary>Mode that expresses the velocity with radial and angular speed.</summary>
            POLAR
        }

        #region Basic variables
        /// <summary>Bullet's move mode. See <see cref="UDEBulletMovement.MoveMode"/>.</summary>
        public MoveMode mode;
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

        /// <summary>Shows how many time the movement is used in the method <see cref="SansyHuman.UDE.Object.UDEBaseBullet.MoveBullet(float)"/>.</summary>
        public int updateCount;

        #region Cartesian mode variables
        // Cartesian Mode
        /// <summary>The bullet's velocity in cartesian coordinate system.</summary>
        public Vector2 velocity;
        /// <summary>The bullet's maximum speed of each component of the velocity in cartesian coordinate system.</summary>
        public Vector2 maxVelocity;
        /// <summary>The bullet's minimum speed of each compoenent of the velocity in cartesian coordinate system.</summary>
        public Vector2 minVelocity;
        /// <summary>The bullet's maximum magnitude of the velocity vector.</summary>
        public float maxMagnitude;
        /// <summary>The bullet's minimum magnitude of the velocity vector.</summary>
        public float minMagnitude;
        /// <summary>The bullet's acceleration vector by time in cartesian coordinate system.</summary>
        public CartesianTimeFunction accel;
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
        /// <summary>The bullet's acceleration by time in the direction of the movement.</summary>
        public TimeFunction tangentialAccel;
        /// <summary>The bullet's acceleration by time perpendicular to the movement.</summary>
        /// <remarks>It is actually the change of the <see cref="UDEBulletMovement.angle"/></remarks>
        public TimeFunction normalAccel;
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
        /// <summary>The bullet's radial acceleration by time in polar coordinate system.</summary>
        public TimeFunction radialAccel;
        /// <summary>The bullet's angular acceleration by time in polar coordinate system.</summary>
        public TimeFunction angularAccel;
        #endregion

        #region Rotation variables
        /// <summary>Shows if the bullet automatically rotate to the direction of the movement.</summary>
        public bool faceToMovingDirection;
        /// <summary>Angular spped of bullet's rotation.</summary>
        public float rotationAngularSpeed;
        /// <summary>Angular acceleration by time of bullet's rotation.</summary>
        public TimeFunction rotationAngularAcceleration;
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
        /// <param name="accel">Acceleration vector by time in cartesian coordinate system</param>
        /// <param name="speed">Speed in cartesian coordinate system</param>
        /// <param name="maxSpeed">Maximum speed in cartesian coordinate system</param>
        /// <param name="minSpeed">Minimum speed in cartesian coordinate system</param>
        /// <param name="angle">Angle to move</param>
        /// <param name="tangentialAccel">Tangential acceleration by time in cartesian coordinate system</param>
        /// <param name="normalAccel">Normal acceleration by time in cartesian coordinate system</param>
        /// <param name="radialSpeed">Radial speed in polar coordinate system</param>
        /// <param name="angularSpeed">Angular speed in polar coordinate system</param>
        /// <param name="maxRadialSpeed">Maximum radial speed in polar coordinate system</param>
        /// <param name="maxAngularSpeed">Maximum angular speed in polar coordinate system</param>
        /// <param name="minRadialSpeed">Minimum radial speed in polar coordinate system</param>
        /// <param name="minAngularSpeed">Minimum angular speed in polar coordinate system</param>
        /// <param name="radialAccel">Radial acceleration by time in polar coordinate system</param>
        /// <param name="angularAccel">Angular acceleration by time in polar coordinate system</param>
        /// <param name="faceToMovingDirection"><see langword="true"/> if the bullet rotates toward the moving direction</param>
        /// <param name="rotationAngularSpeed">Bullel's speed of rotation</param>
        /// <param name="rotationAngularAcceleration">Bullet's acceleration of rotation by time</param>
        /// <param name="limitRotationSpeed"><see langword="true"/> if there is a limit of rotation speed</param>
        /// <param name="minRotationSpeed">Minimum speed of rotation</param>
        /// <param name="maxRotationSpeed">Maximum speed of rotation</param>
        public UDEBulletMovement(MoveMode mode,
                                 float startTime,
                                 float endTime,
                                 bool hasEndTime,
                                 bool limitSpeed,
                                 bool setSpeedToPrevMovement,
                                 Vector2 velocity,
                                 Vector2 maxVelocity,
                                 Vector2 minVelocity,
                                 float maxMagnitude,
                                 float minMagnitude,
                                 CartesianTimeFunction accel,
                                 float speed,
                                 float maxSpeed,
                                 float minSpeed,
                                 float angle,
                                 TimeFunction tangentialAccel,
                                 TimeFunction normalAccel,
                                 float radialSpeed,
                                 float angularSpeed,
                                 float maxRadialSpeed,
                                 float maxAngularSpeed,
                                 float minRadialSpeed,
                                 float minAngularSpeed,
                                 TimeFunction radialAccel,
                                 TimeFunction angularAccel,
                                 bool faceToMovingDirection,
                                 float rotationAngularSpeed,
                                 TimeFunction rotationAngularAcceleration,
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
            this.accel = accel ?? throw new ArgumentNullException(nameof(accel));
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.minSpeed = minSpeed;
            this.angle = angle;
            this.tangentialAccel = tangentialAccel ?? throw new ArgumentNullException(nameof(tangentialAccel));
            this.normalAccel = normalAccel ?? throw new ArgumentNullException(nameof(normalAccel));
            this.radialSpeed = radialSpeed;
            this.angularSpeed = angularSpeed;
            this.maxRadialSpeed = maxRadialSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.minRadialSpeed = minRadialSpeed;
            this.minAngularSpeed = minAngularSpeed;
            this.radialAccel = radialAccel ?? throw new ArgumentNullException(nameof(radialAccel));
            this.angularAccel = angularAccel ?? throw new ArgumentNullException(nameof(angularAccel));
            this.faceToMovingDirection = faceToMovingDirection;
            this.rotationAngularSpeed = rotationAngularSpeed;
            this.rotationAngularAcceleration = rotationAngularAcceleration;
            this.limitRotationSpeed = limitRotationSpeed;
            this.minRotationSpeed = minRotationSpeed;
            this.maxRotationSpeed = maxRotationSpeed;
        }

        public UDEBulletMovementECS ToECSMovement()
        {
            return new UDEBulletMovementECS(mode, startTime, endTime, hasEndTime, limitSpeed,
                setSpeedToPrevMovement, velocity.Tofloat2(), maxVelocity.Tofloat2(),
                minVelocity.Tofloat2(), maxMagnitude, minMagnitude, accel(0).ToVector2().Tofloat2(), speed,
                maxSpeed, minSpeed, angle, tangentialAccel(0), normalAccel(0), radialSpeed,
                angularSpeed, maxRadialSpeed, maxAngularSpeed, minRadialSpeed, minAngularSpeed,
                radialAccel(0), angularAccel(0), faceToMovingDirection, rotationAngularSpeed,
                rotationAngularAcceleration(0), limitRotationSpeed, minRotationSpeed, maxRotationSpeed);
        }

        #region Basic builders
        /// <summary>
        /// Creat the <see cref="UDEBulletMovement"/> instance that has no movement.
        /// </summary>
        public static UDEBulletMovement GetNoMovement()
        {
            return new UDEBulletMovement(MoveMode.CARTESIAN, 0, 0, false, false, false, Vector2.zero, Vector2.zero, Vector2.zero, 0, 0, t => new CartesianCoord(),
                0, 0, 0, 0, t => 0, t => 0, 0, 0, 0, 0, 0, 0, t => 0, t => 0, true, 0, t => 0, false, 0, 0);
        }
        #endregion

        /// <summary>
        /// Override of <see cref="object.ToString"/>.
        /// </summary>
        /// <returns>String contains informations of the movement</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder("Move Mode: ");
            str.Append(mode);
            str.Append("\nStart Time: ").Append(startTime);
            if (hasEndTime)
                str.Append("\nEnd Time: ").Append(endTime);
            switch (this.mode)
            {
                case MoveMode.CARTESIAN:
                    str.Append("\n\nVelocity: ").Append(velocity);
                    if (limitSpeed)
                    {
                        str.Append("\nMinimum x velocity: ").Append(minVelocity.x);
                        str.Append("\nMinimum y velocity: ").Append(minVelocity.y);
                        str.Append("\nMaximum x velocity: ").Append(maxVelocity.x);
                        str.Append("\nMaximum y velocity: ").Append(maxVelocity.y);
                        str.Append("\nMinimum speed: ").Append(minMagnitude);
                        str.Append("\nMaximum speed: ").Append(maxMagnitude);
                    }
                    break;
                case MoveMode.CARTESIAN_POLAR:
                    str.Append("\n\nSpeed: ").Append(speed);
                    if (limitSpeed)
                    {
                        str.Append("\nMinimum Speed: ").Append(minSpeed);
                        str.Append("\nMaximum Speed: ").Append(maxSpeed);
                    }
                    str.Append("\nMoving Angle: ").Append(angle);
                    break;
                case MoveMode.POLAR:
                    str.Append("\n\nRadial Speed: ").Append(radialSpeed);
                    str.Append("\nAngular Speed: ").Append(angularSpeed);
                    if (limitSpeed)
                    {
                        str.Append("\nMinimum Radial Speed: ").Append(minRadialSpeed);
                        str.Append("\nMaximum Radial Speed: ").Append(maxRadialSpeed);
                        str.Append("\nMinimum Angular Speed: ").Append(minAngularSpeed);
                        str.Append("\nMaximum Angular Speed: ").Append(maxAngularSpeed);
                    }
                    break;
            }
            if (faceToMovingDirection)
                str.Append("\n\nThe bullet is facing to moving direction.");
            else
            {
                str.Append("\n\nRotational Angular Speed: ").Append(rotationAngularSpeed);
                if (limitRotationSpeed)
                {
                    str.Append("\nMinimum Rotational Speed: ").Append(minRotationSpeed);
                    str.Append("\nMaximum Rotational Speed: ").Append(maxRotationSpeed);
                }
            }
            return str.ToString();
        }
    }
}