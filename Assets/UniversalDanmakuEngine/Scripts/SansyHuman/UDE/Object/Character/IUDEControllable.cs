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

using System.Collections;
using UnityEngine;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Interface for objects that player can control
    /// </summary>
    public interface IUDEControllable
    {
        /// <value>Gets the speed of the object.</value>
        float Speed
        {
            get; set;
        }

        /// <value>Gets the multifiler to speed when the object is in slow mode(when player pressed slow button).</value>
        float SlowModeSpeedMultiplier
        {
            get; set;
        }

        /// <value>Gets the time interval to shoot the bullets.</value>
        float BulletFireInterval
        {
            get; set;
        }

        /// <summary>
        /// Moves the object. It is recommended to call this method in <c>FixedUpdate()</c>.
        /// </summary>
        /// <param name="deltaTime"><see cref="Time.deltaTime"/></param>
        void Move(float deltaTime);

        /// <summary>
        /// Shoots the bullets.
        /// </summary>
        /// <returns>Coroutine that shoots the bullet every <see cref="BulletFireInterval"/> seconds when fire button is pressed</returns>
        IEnumerator ShootBullet();
    }
}