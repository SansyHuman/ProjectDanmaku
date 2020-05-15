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
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SansyHuman.UDE.Util.Math
{
    public static partial class UDEMath
    {
        #region Coordinates
        /// <summary>
        /// Struct to express the polar coordinate.
        /// </summary>
        public struct PolarCoord : IEquatable<PolarCoord>
        {
            /// <summary>Distance from the origin.</summary>
            public float radius;
            /// <summary>Angle from the x+ axis in unit degree.</summary>
            public float degree;

            /// <summary>
            /// Basic constructor.
            /// </summary>
            /// <param name="radius">Distance from the origin</param>
            /// <param name="degree">Angle from the x+ axis in unit degree</param>
            public PolarCoord(float radius, float degree) { this.radius = radius; this.degree = degree; }

            public PolarCoord(PolarCoord coord) { radius = coord.radius; degree = coord.degree; }

            /// <summary>
            /// Deconstructor of the struct.
            /// </summary>
            /// <param name="radius">Distance from the origin</param>
            /// <param name="degree">Angle from the x+ axis in unit degree</param>
            public void Deconstruct(out float radius, out float degree) { radius = this.radius; degree = this.degree; }

            /// <summary>
            /// Override of <see cref="object.Equals(object)"/>.
            /// </summary>
            /// <param name="obj"><see cref="PolarCoord"/> instance to compare</param>
            /// <returns><see langword="true"/> if <paramref name="obj"/> equals the instance</returns>
            public override bool Equals(object obj)
            {
                return obj is PolarCoord coord && Equals(coord);
            }

            /// <summary>
            /// Implementation of <see cref="IEquatable{PolarCoord}.Equals(PolarCoord)"/>.
            /// </summary>
            /// <param name="other"><see cref="PolarCoord"/> instance to compare</param>
            /// <returns><see langword="true"/> if <paramref name="other"/> equals the instance</</returns>
            public bool Equals(PolarCoord other)
            {
                return this == other;
            }

            /// <summary>
            /// Override of <see cref="object.GetHashCode"/>.
            /// </summary>
            /// <returns>Hashcode of the instance</returns>
            public override int GetHashCode()
            {
                var hashCode = 1822;
                hashCode = hashCode + radius.GetHashCode() * -111;
                hashCode = hashCode + degree.GetHashCode() * 159;
                return hashCode;
            }

            /// <summary>
            /// Overload of operator <c>==</c>.
            /// </summary>
            /// <param name="c1">Insance on the left side of <c>==</c></param>
            /// <param name="c2">Insance on the right side of <c>==</c></param>
            /// <returns><see langword="true"/> if two instance represents same coordinate</returns>
            public static bool operator ==(PolarCoord c1, PolarCoord c2)
            {
                if (c1.radius == c2.radius)
                {
                    if (Mathf.FloorToInt(c1.degree - c2.degree) == (c1.degree - c2.degree) && (int)(c1.degree - c2.degree) % 360 == 0)
                        return true;
                    else
                        return false;
                }
                else if (c1.radius == -c2.radius)
                {
                    if (Mathf.FloorToInt(c1.degree - c2.degree) == (c1.degree - c2.degree) && (int)(c1.degree - c2.degree) % 360 == 180)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            /// <summary>
            /// Overload of operator <c>!=</c>.
            /// </summary>
            /// <param name="c1">Insance on the left side of <c>!=</c></param>
            /// <param name="c2">Insance on the right side of <c>!=</c></param>
            /// <returns><see langword="true"/> if two instance represents different coordinate</returns>
            public static bool operator !=(PolarCoord c1, PolarCoord c2)
            {
                return !(c1 == c2);
            }

            /// <summary>
            /// Implicit type conversion to <c>(float r, float deg)</c> tuple.
            /// </summary>
            /// <param name="c"><see cref="PolarCoord"/> instance</param>
            public static implicit operator (float r, float deg) (PolarCoord c)
            {
                return (c.radius, c.degree);
            }

            /// <summary>
            /// Explicit type conversion to <see cref="UnityEngine.Vector2"/>.
            /// </summary>
            /// <param name="c"><see cref="PolarCoord"/> instance</param>
            public static explicit operator UnityEngine.Vector2(PolarCoord c)
            {
                float radian = c.degree * Mathf.Deg2Rad;
                return new UnityEngine.Vector2(c.radius * Mathf.Cos(radian), c.radius * Mathf.Sin(radian));
            }

            /// <summary>
            /// Explicit type conversion to <see cref="CartesianCoord"/>.
            /// </summary>
            /// <param name="c"><see cref="PolarCoord"/> instance</param>
            public static explicit operator CartesianCoord(PolarCoord c)
            {
                float radian = c.degree * Mathf.Deg2Rad;
                return new CartesianCoord(c.radius * Mathf.Cos(radian), c.radius * Mathf.Sin(radian));
            }
        }

        /// <summary>
        /// Struct to express the cartesian coordinate.
        /// </summary>
        public struct CartesianCoord : IEquatable<CartesianCoord>
        {
            /// <summary>x coordinate.</summary>
            public float x;
            /// <summary>y coordinate.</summary>
            public float y;

            /// <value>Gets the distance from the origin.</value>
            public float Magnitude
            {
                get => Mathf.Sqrt((x * x) + (y * y));
            }

            /// <value>Gets the square of the distance from the origin.</value>
            public float SquareMagnitude
            {
                get => (x * x) + (y * y);
            }

            /// <summary>
            /// Basic constructor.
            /// </summary>
            /// <param name="x">x coordinate</param>
            /// <param name="y">y coordinate</param>
            public CartesianCoord(float x, float y) { this.x = x; this.y = y; }

            public CartesianCoord(CartesianCoord coord) { x = coord.x; y = coord.y; }

            public CartesianCoord(UnityEngine.Vector2 vector) { x = vector.x; y = vector.y; }

            /// <summary>
            /// Change the struct to <see cref="UnityEngine.Vector2"/>.
            /// </summary>
            /// <returns><see cref="UnityEngine.Vector2"/> instance</returns>
            public UnityEngine.Vector2 ToVector2() { return new UnityEngine.Vector2(x, y); }

            /// <summary>
            /// Deconstructor of the struct.
            /// </summary>
            /// <param name="x">x coordinate</param>
            /// <param name="y">y coordinate</param>
            public void Deconstruct(out float x, out float y) { x = this.x; y = this.y; }

            /// <summary>
            /// Override of <see cref="object.Equals(object)"/>.
            /// </summary>
            /// <param name="obj"><see cref="CartesianCoord"/> instance to compare</param>
            /// <returns><see langword="true"/> if <paramref name="obj"/> equals the instance</returns>
            public override bool Equals(object obj)
            {
                return obj is CartesianCoord coord && Equals(coord);
            }

            /// <summary>
            /// Implementation of <see cref="IEquatable{CartesianCoord}.Equals(CartesianCoord)"/>.
            /// </summary>
            /// <param name="other"><see cref="CartesianCoord"/> instance to compare</param>
            /// <returns><see langword="true"/> if <paramref name="other"/> equals the instance</returns>
            public bool Equals(CartesianCoord other)
            {
                return x == other.x && y == other.y;
            }

            /// <summary>
            /// Override of <see cref="object.GetHashCode"/>.
            /// </summary>
            /// <returns>Hashcode of the instance</returns>
            public override int GetHashCode()
            {
                var hashCode = 15023;
                hashCode = hashCode + x.GetHashCode() * -152;
                hashCode = hashCode + y.GetHashCode() * 537;
                return hashCode;
            }

            /// <summary>
            /// Overload of binary operator <c>+</c>.
            /// </summary>
            /// <param name="c1">Instance on the left side of <c>+</c></param>
            /// <param name="c2">Instance on the right side of <c>+</c></param>
            /// <returns>Sum of two coordinates</returns>
            public static CartesianCoord operator +(CartesianCoord c1, CartesianCoord c2)
            {
                return new CartesianCoord(c1.x + c2.x, c1.y + c2.y);
            }

            /// <summary>
            /// Overload of unary operator <c>+</c>.
            /// </summary>
            /// <param name="c">Instance on the right side of <c>+</c></param>
            /// <returns>Unchanged coordinate</returns>
            public static CartesianCoord operator +(CartesianCoord c)
            {
                return new CartesianCoord(c.x, c.y);
            }

            /// <summary>
            /// Overload of binary operator <c>-</c>.
            /// </summary>
            /// <param name="c1"></param>
            /// <param name="c2"></param>
            /// <returns></returns>
            public static CartesianCoord operator -(CartesianCoord c1, CartesianCoord c2)
            {
                return new CartesianCoord(c1.x - c2.x, c1.y - c2.y);
            }

            /// <summary>
            /// Overload of unary operator <c>-</c>.
            /// </summary>
            /// <param name="c">Instance on the right side of <c>-</c></param>
            /// <returns>Coordinate symmetric to origin</returns>
            public static CartesianCoord operator -(CartesianCoord c)
            {
                return new CartesianCoord(-c.x, -c.y);
            }

            /// <summary>
            /// Overload of binary operator <c>*</c>.
            /// </summary>
            /// <param name="multiplier">Number on the left side of <c>*</c></param>
            /// <param name="c">Instance on the right side of <c>*</c></param>
            /// <returns>Coordinate of which each coordinate multiplied by multiplier</returns>
            public static CartesianCoord operator *(float multiplier, CartesianCoord c)
            {
                return new CartesianCoord(multiplier * c.x, multiplier * c.y);
            }

            /// <summary>
            /// Overload of binary operator <c>*</c>.
            /// </summary>
            /// <param name="c">Instance on the left side of <c>*</c></param>
            /// <param name="multiplier">Number on the right side of <c>*</c></param>
            /// <returns>Coordinate of which each coordinate multiplied by multiplier</returns>
            public static CartesianCoord operator *(CartesianCoord c, float multiplier)
            {
                return new CartesianCoord(multiplier * c.x, multiplier * c.y);
            }

            /// <summary>
            /// Overload of operator <c>==</c>.
            /// </summary>
            /// <param name="c1">Insance on the left side of <c>==</c></param>
            /// <param name="c2">Insance on the right side of <c>==</c></param>
            /// <returns><see langword="true"/> if two instance represents same coordinate</returns>
            public static bool operator ==(CartesianCoord c1, CartesianCoord c2)
            {
                return (c1.x == c2.x) && (c1.y == c2.y);
            }

            /// <summary>
            /// Overload of operator <c>!=</c>.
            /// </summary>
            /// <param name="c1">Insance on the left side of <c>!=</c></param>
            /// <param name="c2">Insance on the right side of <c>!=</c></param>
            /// <returns><see langword="true"/> if two instance represents different coordinate</returns>
            public static bool operator !=(CartesianCoord c1, CartesianCoord c2)
            {
                return (c1.x != c2.x) || (c1.y != c2.y);
            }

            /// <summary>
            /// Implicit type conversion to <see cref="UnityEngine.Vector2"/>.
            /// </summary>
            /// <param name="c"><see cref="CartesianCoord"/> instance</param>
            public static implicit operator UnityEngine.Vector2(CartesianCoord c)
            {
                return c.ToVector2();
            }

            /// <summary>
            /// Implicit type conversion to <c>(float x, float y)</c> tuple.
            /// </summary>
            /// <param name="c"><see cref="CartesianCoord"/> instance</param>
            public static implicit operator (float x, float y) (CartesianCoord c)
            {
                return (c.x, c.y);
            }

            /// <summary>
            /// Explicit type conversion to <see cref="PolarCoord"/>.
            /// </summary>
            /// <param name="c"><see cref="CartesianCoord"/> instance</param>
            public static explicit operator PolarCoord(CartesianCoord c)
            {
                return new PolarCoord(c.Magnitude, Deg(c));
            }
        }
        #endregion
    }
}