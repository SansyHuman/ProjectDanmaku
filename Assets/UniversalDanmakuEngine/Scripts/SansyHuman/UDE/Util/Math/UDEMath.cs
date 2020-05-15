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

using Unity.Mathematics;
using UnityEngine;

namespace SansyHuman.UDE.Util.Math
{
    /// <summary>
    /// The class of useful mathematics.
    /// </summary>
    public static partial class UDEMath
    {
        #region Degree in coordinate system
        /// <summary>
        /// An angle of the vector (x, y) in polar coordinates. The unit is degree and range is 0 - 360.
        /// </summary>
        /// <param name="x">x component of the vector</param>
        /// <param name="y">y component of the vector</param>
        /// <returns>Degree of (x, y) in polar system</returns>
        public static float Deg(float x, float y)
        {
            float deg = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            if (deg < 0)
                deg += 360;
            return deg;
        }

        /// <summary>
        /// An angle of the vector r in polar coordinates. The unit is degree and range is 0 - 360.
        /// </summary>
        /// <param name="r">2-dimensional vector</param>
        /// <returns>Degree of r in polar system</returns>
        public static float Deg(UnityEngine.Vector2 r)
        {
            return Deg(r.x, r.y);
        }
        #endregion

        #region Coordinate transformation
        /// <summary>
        /// Changes cartesian coordinates (x, y) to polar coordinates.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="r">Magnitude of the vector(output only)</param>
        /// <param name="deg">Degree of the vector(output only)</param>
        public static void Cartesian2Polar(float x, float y, out float r, out float deg)
        {
            r = Mathf.Sqrt((x * x) + (y * y));
            deg = Deg(x, y);
        }

        /// <summary>
        /// Changes cartesian coordinates (x, y) to polar coordinates.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>A tuple (r, deg) that represents polar coordinates</returns>
        public static (float r, float deg) Cartesian2Polar(float x, float y)
        {
            float r = Mathf.Sqrt((x * x) + (y * y));
            float deg = Deg(x, y);
            return (r, deg);
        }

        /// <summary>
        /// Changes cartesian coordinates of the vector d to polar coordinates.
        /// </summary>
        /// <param name="d">2-dimensional vector</param>
        /// <param name="r">Magnitude of the vector(output only)</param>
        /// <param name="deg">Degree of the vector(output only)</param>
        public static void Cartesian2Polar(UnityEngine.Vector2 d, out float r, out float deg)
        {
            Cartesian2Polar(d.x, d.y, out r, out deg);
        }

        /// <summary>
        /// Changes cartesian coordinates of the vector d to polar coordinates.
        /// </summary>
        /// <param name="d">2-dimensional vector</param>
        /// <returns>A tuple (r, deg) that represents polar coordinates</returns>
        public static (float r, float deg) Cartesian2Polar(UnityEngine.Vector2 d)
        {
            return Cartesian2Polar(d.x, d.y);
        }

        /// <summary>
        /// Changes polar coordinates to cartesian coordinates.
        /// </summary>
        /// <param name="r">Magnitude of the vector</param>
        /// <param name="deg">Degree of the vector</param>
        /// <param name="x">x coordinate(output only)</param>
        /// <param name="y">y coordinate(output only)</param>
        public static void Polar2Cartesian(float r, float deg, out float x, out float y)
        {
            float rad = deg * Mathf.Deg2Rad;
            x = r * Mathf.Cos(rad);
            y = r * Mathf.Sin(rad);
        }

        /// <summary>
        /// Changes polar coordinates to cartesian coordinates.
        /// </summary>
        /// <param name="r">Magnitude of the vector</param>
        /// <param name="deg">Degree of the vector</param>
        /// <returns>A tuple (x, y) that represents cartesian coordinates</returns>
        public static (float x, float y) Polar2Cartesian(float r, float deg)
        {
            float rad = deg * Mathf.Deg2Rad;
            float x = r * Mathf.Cos(rad);
            float y = r * Mathf.Sin(rad);
            return (x, y);
        }

        public static Vector2 ToVector2(this (float x, float y) tuple)
        {
            return new Vector2(tuple.x, tuple.y);
        }

        public static float2 Tofloat2(this Vector2 vector)
        {
            return new float2(vector.x, vector.y);
        }
        #endregion
    }
}