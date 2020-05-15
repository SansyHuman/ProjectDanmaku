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
using TestMySpline;

namespace SansyHuman.UDE.Util.Math
{
    public static partial class UDEMath
    {
        #region Function delegates
        /// <summary>
        /// Delegate of polar parametric function.
        /// <para>Mathmatically, it equals to <c>r = f(p, t), deg = g(p, t)</c></para>.
        /// </summary>
        /// <param name="param">Time-independent parameter</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns><see cref="PolarCoord"/> instance with given parameters</returns>
        public delegate PolarCoord PolarParametricFunction(float param, float time);

        /// <summary>
        /// Delegate of polar function.
        /// <para>Mathmatically, it equals to <c>r = f(deg, t)</c></para>.
        /// </summary>
        /// <param name="degree">Angle parameter in degrees</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns>Radius with given parameters</returns>
        public delegate float PolarFunction(float degree, float time);

        /// <summary>
        /// Delegate of polar inverse function.
        /// <para>Mathmatically, it equals to <c>deg = f(r, t)</c></para>.
        /// </summary>
        /// <param name="radius">Radius parameter</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns>Angle with given parameters in degrees</returns>
        public delegate float PolarInverseFunction(float radius, float time);

        /// <summary>
        /// Delegate of cartesian parametric function.
        /// <para>Mathmatically, it equals to <c>x = f(p, t), y = g(p, t)</c></para>.
        /// </summary>
        /// <param name="param">Time-independent parameter</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns><see cref="CartesianCoord"/> instance with given parameters</returns>
        public delegate CartesianCoord CartesianParametricFunction(float param, float time);

        /// <summary>
        /// Delegate of cartesian function.
        /// <para>Mathmatically, it equals to <c>y = f(x, t)</c></para>.
        /// </summary>
        /// <param name="x">x parameter</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns>y coordinate with given parameters</returns>
        public delegate float CartesianFunction(float x, float time);

        /// <summary>
        /// Delegate of cartesian inverse function.
        /// <para>Mathmatically, it equals to <c>x = f(y, t)</c></para>.
        /// </summary>
        /// <param name="y">y parameter</param>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns>x coordinate with given parameters</returns>
        public delegate float CartesianInverseFunction(float y, float time);
        #endregion

        #region Function calculation

        #region Polar function calculation
        /// <summary>
        /// Gets list of polar coordinate tuples with polar parametric function.
        /// </summary>
        /// <param name="function">Polar parametric function to use</param>
        /// <param name="minParam">Minimum parameter</param>
        /// <param name="maxParam">Maximum parameter</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of polar coordinate tuples</returns>
        public static (float r, float deg)[] GetPolarCoordList(PolarParametricFunction function, float minParam, float maxParam, float time, int count)
        {
            var coords = new (float r, float deg)[count];
            float interval = (maxParam - minParam) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                coords[i] = function(minParam + (interval * i), time);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="PolarCoord"/> with polar parametric function.
        /// </summary>
        /// <param name="function">Polar parametric function to use</param>
        /// <param name="minParam">Minimum parameter</param>
        /// <param name="maxParam">Maximum parameter</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="PolarCoord"/></returns>
        public static PolarCoord[] GetPolarCoordStructs(PolarParametricFunction function, float minParam, float maxParam, float time, int count)
        {
            var coords = new PolarCoord[count];
            float interval = (maxParam - minParam) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                coords[i] = function(minParam + (interval * i), time);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of polar coordinate tuples with polar function.
        /// </summary>
        /// <param name="function">Polar function to use</param>
        /// <param name="minDegree">Minimum angle in degrees</param>
        /// <param name="maxDegree">Maximum angle in degrees</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of polar coordinate tuples</returns>
        public static (float r, float deg)[] GetPolarCoordList(PolarFunction function, float minDegree, float maxDegree, float time, int count)
        {
            var coords = new (float r, float deg)[count];
            float interval = (maxDegree - minDegree) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float degree = minDegree + (interval * i);
                coords[i] = (function(degree, time), degree);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="PolarCoord"/> with polar function.
        /// </summary>
        /// <param name="function">Polar function to use</param>
        /// <param name="minDegree">Minimum angle in degrees</param>
        /// <param name="maxDegree">Maximum angle in degrees</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="PolarCoord"/></returns>
        public static PolarCoord[] GetPolarCoordStructs(PolarFunction function, float minDegree, float maxDegree, float time, int count)
        {
            var coords = new PolarCoord[count];
            float interval = (maxDegree - minDegree) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float degree = minDegree + (interval * i);
                coords[i] = new PolarCoord(function(degree, time), degree);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of polar coordinate tuples with polar inverse function.
        /// </summary>
        /// <param name="function">Polar inverse function to use</param>
        /// <param name="minRadius">Minimum radius</param>
        /// <param name="maxRadius">Maximum radius</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of polar coordinate tuples</returns>
        public static (float r, float deg)[] GetPolarCoordList(PolarInverseFunction function, float minRadius, float maxRadius, float time, int count)
        {
            var coords = new (float r, float deg)[count];
            float interval = (maxRadius - minRadius) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float radius = minRadius + (interval * i);
                coords[i] = (radius, function(radius, time));
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="PolarCoord"/> with polar inverse function.
        /// </summary>
        /// <param name="function">Polar inverse function to use</param>
        /// <param name="minRadius">Minimum radius</param>
        /// <param name="maxRadius">Maximum radius</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="PolarCoord"/></returns>
        public static PolarCoord[] GetPolarCoordStructs(PolarInverseFunction function, float minRadius, float maxRadius, float time, int count)
        {
            var coords = new PolarCoord[count];
            float interval = (maxRadius - minRadius) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float radius = minRadius + (interval * i);
                coords[i] = new PolarCoord(radius, function(radius, time));
            }
            return coords;
        }
        #endregion

        #region Cartesian function calculation
        /// <summary>
        /// Gets list of cartesian coordinate tuples with cartesian parametric function.
        /// </summary>
        /// <param name="function">Cartesian parametric function to use</param>
        /// <param name="minParam">Minimum parameter</param>
        /// <param name="maxParam">Maximum parameter</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of Cartesian coordinate tuples</returns>
        public static (float x, float y)[] GetCartesianCoordList(CartesianParametricFunction function, float minParam, float maxParam, float time, int count)
        {
            var coords = new (float x, float y)[count];
            float interval = (maxParam - minParam) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                coords[i] = function(minParam + (interval * i), time);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="CartesianCoord"/> with cartesian parametric function.
        /// </summary>
        /// <param name="function">Cartesian parametric function to use</param>
        /// <param name="minParam">Minimum parameter</param>
        /// <param name="maxParam">Maximum parameter</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="CartesianCoord"/></returns>
        public static CartesianCoord[] GetCartesianCoordStructs(CartesianParametricFunction function, float minParam, float maxParam, float time, int count)
        {
            var coords = new CartesianCoord[count];
            float interval = (maxParam - minParam) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                coords[i] = function(minParam + (interval * i), time);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of cartesian coordinate tuples with cartesian function.
        /// </summary>
        /// <param name="function">Cartesian function to use</param>
        /// <param name="minX">Minimum x coordinate</param>
        /// <param name="maxX">Maximum x coordinate</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of Cartesian coordinate tuples</returns>
        public static (float x, float y)[] GetCartesianCoordList(CartesianFunction function, float minX, float maxX, float time, int count)
        {
            var coords = new (float x, float y)[count];
            float interval = (maxX - minX) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float xCoord = minX + (interval * i);
                coords[i] = (xCoord, function(minX + (interval * i), time));
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="CartesianCoord"/> with cartesian function.
        /// </summary>
        /// <param name="function">Cartesian function to use</param>
        /// <param name="minX">Minimum x coordinate</param>
        /// <param name="maxX">Maximum x coordinate</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="CartesianCoord"/></returns>
        public static CartesianCoord[] GetCartesianCoordStructs(CartesianFunction function, float minX, float maxX, float time, int count)
        {
            var coords = new CartesianCoord[count];
            float interval = (maxX - minX) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float xCoord = minX + (interval * i);
                coords[i] = new CartesianCoord(xCoord, function(xCoord, time));
            }
            return coords;
        }

        /// <summary>
        /// Gets list of cartesian coordinate tuples with cartesian inverse function.
        /// </summary>
        /// <param name="function">Cartesian inverse function to use</param>
        /// <param name="minY">Minimum y coordinate</param>
        /// <param name="maxY">Maximum y coordinate</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of Cartesian coordinate tuples</returns>
        public static (float x, float y)[] GetCartesianCoordList(CartesianInverseFunction function, float minY, float maxY, float time, int count)
        {
            var coords = new (float x, float y)[count];
            float interval = (maxY - minY) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float yCoord = minY + (interval * i);
                coords[i] = (function(yCoord, time), yCoord);
            }
            return coords;
        }

        /// <summary>
        /// Gets list of <see cref="CartesianCoord"/> with cartesian inverse function.
        /// </summary>
        /// <param name="function">Cartesian inverse function to use</param>
        /// <param name="minY">Minimum y coordinate</param>
        /// <param name="maxY">Maximum y coordinate</param>
        /// <param name="time">Time</param>
        /// <param name="count">Number of coordinates to create</param>
        /// <returns>List of <see cref="CartesianCoord"/></returns>
        public static CartesianCoord[] GetCartesianCoordStructs(CartesianInverseFunction function, float minY, float maxY, float time, int count)
        {
            var coords = new CartesianCoord[count];
            float interval = (maxY - minY) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                float yCoord = minY + (interval * i);
                coords[i] = new CartesianCoord(function(yCoord, time), yCoord);
            }
            return coords;
        }
        #endregion

        #endregion

        #region Functions only dependent on time
        /// <summary>
        /// Delegate of polar function only dependent on time.
        /// <para>Mathmatically, it equals to <c>r = f(t), deg = g(t)</c></para>.
        /// </summary>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns><see cref="PolarCoord"/> instance with given parameter</returns>
        public delegate PolarCoord PolarTimeFunction(float time);

        /// <summary>
        /// Delegate of cartesian function only dependent on time.
        /// <para>Mathmatically, it equals to <c>x = f(t), y = g(t)</c></para>.
        /// </summary>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns><see cref="CartesianCoord"/> instance with given parameter</returns>
        public delegate CartesianCoord CartesianTimeFunction(float time);

        /// <summary>
        /// Delegate of function only dependent on time.
        /// <para>Mathmatically, it equals to <c>T = f(t)</c></para>.
        /// </summary>
        /// <param name="time">Time-dependent parameter</param>
        /// <returns>A float value with given parameter</returns>
        public delegate float TimeFunction(float time);
        #endregion

        #region Time-independent functions
        /// <summary>
        /// Delegate of polar parametric function that independent to time.
        /// <para>Mathmatically, it equals to <c>r = f(p), deg = g(p)</c></para>.
        /// </summary>
        /// <param name="param">Time-independent parameter</param>
        /// <returns><see cref="PolarCoord"/> instance with given parameter</returns>
        public delegate PolarCoord PolarParametricFunctionWithNoTime(float param);

        /// <summary>
        /// Delegate of polar function that independent to time.
        /// <para>Mathmatically, it equals to <c>r = f(deg)</c></para>.
        /// </summary>
        /// <param name="degree">Angle parameter in degrees</param>
        /// <returns>Radius with given parameter</returns>
        public delegate float PolarFunctionWithNoTime(float degree);

        /// <summary>
        /// Delegate of polar inverse function that independent to time.
        /// <para>Mathmatically, it equals to <c>deg = f(r)</c></para>.
        /// </summary>
        /// <param name="radius">Radius parameter</param>
        /// <returns>Angle with given parameter in degrees</returns>
        public delegate float PolarInverseFunctionWithNoTime(float radius);

        /// <summary>
        /// Delegate of cartesian parametric function that independent to time.
        /// <para>Mathmatically, it equals to <c>x = f(p), y = g(p)</c></para>.
        /// </summary>
        /// <param name="param">Time-independent parameter</param>
        /// <returns><see cref="CartesianCoord"/> instance with given parameter</returns>
        public delegate CartesianCoord CartesianParametricFunctionWithNoTime(float param);

        /// <summary>
        /// Delegate of cartesian function that independent to time.
        /// <para>Mathmatically, it equals to <c>y = f(x)</c></para>.
        /// </summary>
        /// <param name="x">x parameter</param>
        /// <returns>y coordinate with given parameter</returns>
        public delegate float CartesianFunctionWithNoTime(float x);

        /// <summary>
        /// Delegate of cartesian inverse function that independent to time.
        /// <para>Mathmatically, it equals to <c>x = f(y)</c></para>.
        /// </summary>
        /// <param name="y">y parameter</param>
        /// <returns>x coordinate with given parameter</returns>
        public delegate float CartesianInverseFunctionWithNoTime(float y);
        #endregion

        #region Function composition
        /// <summary>
        /// Extension of <see cref="PolarTimeFunction"/>. Composites f and g.
        /// <para>Range of g should be in domain of f.</para>
        /// </summary>
        /// <param name="f">First polar time function</param>
        /// <param name="g">Second time function</param>
        /// <returns>Function <c>f(g(t))</c></returns>
        public static PolarTimeFunction Composite(this PolarTimeFunction f, TimeFunction g)
        {
            return new PolarTimeFunction((t) => f(g(t)));
        }

        /// <summary>
        /// Extension of <see cref="CartesianTimeFunction"/>. Composites f and g.
        /// <para>Range of g should be in domain of f.</para>
        /// </summary>
        /// <param name="f">First cartesian time function</param>
        /// <param name="g">Second time function</param>
        /// <returns>Function <c>f(g(t))</c></returns>
        public static CartesianTimeFunction Composite(this CartesianTimeFunction f, TimeFunction g)
        {
            return new CartesianTimeFunction((t) => f(g(t)));
        }

        /// <summary>
        /// Extension of <see cref="TimeFunction"/>. Composites f and g.
        /// <para>Range of g should be in domain of f.</para>
        /// </summary>
        /// <param name="f">First time function</param>
        /// <param name="g">Second time function</param>
        /// <returns>Function <c>f(g(t))</c></returns>
        public static TimeFunction Composite(this TimeFunction f, TimeFunction g)
        {
            return new TimeFunction((t) => f(g(t)));
        }

        /// <summary>
        /// Extension of <see cref="CartesianFunctionWithNoTime"/>. Composites f and g.
        /// <para>Range of g should be in domain of f.</para>
        /// </summary>
        /// <param name="f">First cartesian function</param>
        /// <param name="g">Second cartesian function</param>
        /// <returns>Function <c>f(g(x))</c></returns>
        public static CartesianFunctionWithNoTime Composite(this CartesianFunctionWithNoTime f, CartesianFunctionWithNoTime g)
        {
            return new CartesianFunctionWithNoTime((x) => f(g(x)));
        }

        /// <summary>
        /// Extension of <see cref="CartesianInverseFunctionWithNoTime"/>. Composites f and g.
        /// <para>Range of g should be in domain of f.</para>
        /// </summary>
        /// <param name="f">First cartesian inverse function</param>
        /// <param name="g">Second cartesian inverse function</param>
        /// <returns>Function <c>f(g(y))</c></returns>
        public static CartesianInverseFunctionWithNoTime Composite(this CartesianInverseFunctionWithNoTime f, CartesianInverseFunctionWithNoTime g)
        {
            return new CartesianInverseFunctionWithNoTime((y) => f(g(y)));
        }
        #endregion

        #region Some Basic Functions
        /// <summary>
        /// Time function whose value is always zero.
        /// </summary>
        public static readonly TimeFunction zeroTimeFunction = new TimeFunction(t => 0);
        /// <summary>
        /// Cartesian time function whose value is always (0, 0).
        /// </summary>
        public static readonly CartesianTimeFunction zeroCartesianTimeFunction = new CartesianTimeFunction(t => new CartesianCoord(0, 0));

        /// <summary>
        /// Gets coordinate on bezier curve of given control points.
        /// </summary>
        /// <param name="t">Parameter between 0 and 1. 0 returns start point and 1 returns end point</param>
        /// <param name="points">Control points</param>
        /// <returns><see cref="CartesianCoord"/> on the bezier curve</returns>
        public static CartesianCoord GetPointOnBezierCurve(float t, params UnityEngine.Vector2[] points)
        {
            int length = points.Length;
            if (length == 0)
                return new CartesianCoord(0, 0);
            else if (length == 1)
                return new CartesianCoord(points[0]);
            else
            {
                UnityEngine.Vector2[] newPoints = new UnityEngine.Vector2[length - 1];
                for (int i = 0; i < length - 1; i++)
                    newPoints[i] = points[i] * (1 - t) + points[i + 1] * t;
                return GetPointOnBezierCurve(t, newPoints);
            }
        }

        /// <summary>
        /// Gets <see cref="CartesianTimeFunction"/> of the bezier curve with given control points whose domain is from 0 to 1.
        /// </summary>
        /// <param name="points">Control points</param>
        /// <returns>Function of the bezier curve. Parameter of the function should be between 0 and 1</returns>
        public static CartesianTimeFunction GetBezierCurve(params UnityEngine.Vector2[] points)
        {
            return t => GetPointOnBezierCurve(t, points);
        }

        /// <summary>
        /// Gets <see cref="CartesianTimeFunction"/> of the natural cubic spline curve with given control points whose domain is from 0 to 1.
        /// </summary>
        /// <param name="points">Control points</param>
        /// <returns>Function of the natural cubic spline curve. Parameter of the function should be between 0 and 1</returns>
        public static CartesianTimeFunction GetNaturalCubicSplineCurve(params UnityEngine.Vector2[] points)
        {
            int count = points.Length;
            if (count == 0)
                return new CartesianTimeFunction((t) => new CartesianCoord());

            float[] distances = new float[count - 1];
            float totalLength = 0;
            for (int i = 0; i < count - 1; i++)
            {
                distances[i] = (points[i + 1] - points[i]).magnitude;
                totalLength += distances[i];
            }

            float[] parameters = new float[count];
            parameters[0] = 0;
            for (int i = 0; i < count - 2; i++)
                parameters[i + 1] = parameters[i] + (distances[i] / totalLength);
            parameters[count - 1] = 1;

            float interval = 1f / (count - 1);

            (float t, float x)[] xPoints = new (float t, float x)[count];
            (float t, float y)[] yPoints = new (float t, float y)[count];
            for (int i = 0; i < count; i++)
            {
                xPoints[i] = (parameters[i], points[i].x);
                yPoints[i] = (parameters[i], points[i].y);
            }

            var xFunc = GetNaturalCubicSplineFunction(xPoints);
            var yFunc = GetNaturalCubicSplineFunction(yPoints);

            return t => new CartesianCoord(xFunc(t), yFunc(t));
        }

        /// <summary>
        /// Gets the function of natural cubic spline which connects given points.
        /// </summary>
        /// <param name="points">Points to connect</param>
        /// <returns>Function of natural cubic spline which connects the points</returns>
        public static CartesianFunctionWithNoTime GetNaturalCubicSplineFunction(params (float x, float y)[] points)
        {
            if (points.Length == 0)
                return new CartesianFunctionWithNoTime((x) => 0);
            else if (points.Length == 1)
                return new CartesianFunctionWithNoTime((x) => points[0].y);
            else if (points.Length == 2)
                return new CartesianFunctionWithNoTime((x) => points[0].y * (1 - x) + points[1].y * x);

            int count = points.Length;

            float[] xs = new float[count];
            float[] ys = new float[count];

            for (int i = 0; i < count; i++)
            {
                xs[i] = points[i].x;
                ys[i] = points[i].y;
            }

            CubicSpline spline = new CubicSpline(xs, ys);
            return x => spline.Eval(x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the number of points is less than 4</exception>
        public static CartesianTimeFunction GetCatmullRomSplineCurve(params UnityEngine.Vector2[] points)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            CatmullRomSpline spline = new CatmullRomSpline(points);
#pragma warning restore CS0618 // Type or member is obsolete

            return t => spline.Eval(t);
        }

        /// <summary>
        /// Gets linear time function whose domain and range are both <c>[0, 1]</c>.
        /// </summary>
        /// <returns>Linear <see cref="TimeFunction"/> whose domain and range are both <c>[0, 1]</c></returns>
        public static TimeFunction GetLinear()
        {
            return new TimeFunction((t) => t);
        }

        /// <summary>
        /// Gets linear time function which decreases. Domain and range of the function are both <c>[0, 1]</c>.
        /// </summary>
        /// <returns>Function <c>f(t) = 1 - t</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetLinearDecrease()
        {
            return new TimeFunction((t) => 1 - t);
        }

        /// <summary>
        /// Gets sine time function whose domain and range are both <c>[0, 1]</c>.
        /// </summary>
        /// <returns>Function <c>f(t) = sqrt(2)*sin((t - 1)*pi/4) + 1</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetSine()
        {
            return new TimeFunction((t) => Mathf.Sin((t - 1) * Mathf.PI / 4) * Mathf.Sqrt(2) + 1);
        }

        /// <summary>
        /// Gets quadratic time function whose domain and range are both <c>[0, 1]</c>.
        /// </summary>
        /// <returns>Function <c>f(t) = t^2</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetQuadratic()
        {
            return new TimeFunction((t) => t * t);
        }

        /// <summary>
        /// Gets cubic time function whose domain and range are both <c>[0, 1]</c>.
        /// </summary>
        /// <returns>Function <c>f(t) = t^3</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetCubic()
        {
            return new TimeFunction((t) => Mathf.Pow(t, 3));
        }

        /// <returns>Function <c>f(t) = 2^t - 1</c> where <c>0 <= t <= 1</c></returns>
        /// <summary>
        /// Gets exponential time function whose domain and range are both <c>[0, 1]</c>.
        /// </summary>
        /// <param name="inclination">Inclination of the exponential function</param>
        /// <returns>Function <c>f(t) = (2 ^ (t * inclination) - 1) / (2 ^ inclination - 1)</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetExponential(int inclination)
        {
            return new TimeFunction((t) => (Mathf.Pow(2, t * inclination) - 1) / (Mathf.Pow(2, inclination) - 1));
        }

        /// <summary>
        /// Gets function which is a quarter of circle.
        /// <para>The function increases slowly at small t and increases rapidly at t close to 1.</para>
        /// </summary>
        /// <returns>Function <c>f(t) = 1 - sqrt(1 - t^2)</c> where <c>0 <= t <= 1</c></returns>
        public static TimeFunction GetCircularArc()
        {
            return new TimeFunction((t) => 1 - Mathf.Sqrt(1 - t * t));
        }

        /// <summary>
        /// Gets function of elastic oscillation.
        /// </summary>
        /// <param name="oscillCount">The number of oscillation</param>
        /// <param name="inclination">Rate of increase of amplitude</param>
        /// <returns>Oscillation whose amplitude increases</returns>
        public static TimeFunction GetElasticOscillation(int oscillCount, int inclination)
        {
            TimeFunction oscillation = new TimeFunction((t) => Mathf.Sin(t * Mathf.PI)); // 0 <= t <= 2 * oscillTime - 0.5f
            TimeFunction exponential = new TimeFunction((t) => GetExponential(4)(2 * t) * 2 - 1); // 0 <= t <= 0.5f
            TimeFunction sum = new TimeFunction((t) =>
            {
                if (t < 2 * oscillCount - 0.5f)
                    return oscillation(t);
                else
                    return exponential(t - (2 * oscillCount - 0.5f));
            }).Composite((t) => t * 2 * oscillCount);
            TimeFunction amplitude = GetExponential(inclination);
            return new TimeFunction((t) => amplitude(t) * sum(t));
        }

        /// <summary>
        /// Gets function of bounce movement.
        /// </summary>
        /// <param name="bounceCount">The number of bounce</param>
        /// <returns>Function bounce on 0 and ends at 1</returns>
        public static TimeFunction GetBounce(int bounceCount)
        {
            float[] amplitudes = new float[bounceCount];
            float[] intervals = new float[bounceCount];
            float[] intervalSum = new float[bounceCount + 1];
            intervalSum[0] = 0;
            for (int i = 0; i < bounceCount; i++)
            {
                amplitudes[i] = Mathf.Pow(2, i);
                intervals[i] = Mathf.Pow(2, i);
                intervalSum[i + 1] = intervalSum[i] + intervals[i];
                if (i == bounceCount - 1)
                    intervalSum[i + 1] -= intervals[i] * 0.5f;
            }

            TimeFunction sum = new TimeFunction((t) =>
            {
                if (t < 0)
                    return 0;
                for (int i = 0; i < bounceCount; i++)
                {
                    if (intervalSum[i] <= t && t < intervalSum[i + 1])
                    {
                        TimeFunction sin = new TimeFunction((q) => Mathf.Sin(q * Mathf.PI / intervals[i]) * amplitudes[i]);
                        return sin(t - intervalSum[i]);
                    }
                }
                return Mathf.Pow(2, bounceCount - 1);
            });
            return new TimeFunction((t) => sum(t) / Mathf.Pow(2, bounceCount - 1)).Composite((t) => t * intervalSum[bounceCount]);
        }
        #endregion
    }
}