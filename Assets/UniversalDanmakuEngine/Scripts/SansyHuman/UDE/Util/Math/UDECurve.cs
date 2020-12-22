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

using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.UDE.Util.Math
{
    /// <summary>
    /// Class that contains informations of curve.
    /// </summary>
    [AddComponentMenu("UDE/Math/Curve")]
    public class UDECurve : MonoBehaviour
    {
        [SerializeField] private string curveName;
        [SerializeField] private Vector2[] points;
        [SerializeField] private int precision = 100;

        /// <summary>
        /// Enum of type of curve.
        /// <list type="table">
        /// <item>
        /// <term><see cref="BEZIER"/></term>
        /// <description>Bezier curve.</description>
        /// </item>
        /// <item>
        /// <term><see cref="CUBIC_SPLINE"/></term>
        /// <description>Natural cubic spline curve.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum CurveType
        {
            /// <summary>Bezier curve.</summary>
            BEZIER,
            /// <summary>Natural cubic spline curve.</summary>
            CUBIC_SPLINE,
            /// <summary>Catmull-Rom spline curve.</summary>
            CATMULL_ROM
        }

        [SerializeField] private CurveType type;

        // All curves in the project.
        private static Dictionary<string, UDECurve> curves = new Dictionary<string, UDECurve>();

        private void Awake()
        {
            curves.Add(curveName, this);
        }

        private void OnDestroy()
        {
            curves.Remove(curveName);
        }

        /// <summary>
        /// Gets the curve of name.
        /// </summary>
        /// <param name="name">Name of the curve to get</param>
        /// <returns><see cref="UDECurve"/> instance of name</returns>
        /// <exception cref="KeyNotFoundException">Thrown when there is no curve of the name</exception>
        public static UDECurve GetCurveByName(string name)
        {
            return curves[name];
        }

        /// <summary>
        /// Gets the <see cref="SansyHuman.UDE.Util.Math.UDEMath.CartesianTimeFunction"/> of the curve.
        /// </summary>
        /// <returns>Cartesian function of the curve</returns>
        public UDEMath.CartesianTimeFunction GetFunctionOfCurve()
        {
            switch (type)
            {
                case CurveType.BEZIER:
                    return UDEMath.GetBezierCurve(points);
                case CurveType.CUBIC_SPLINE:
                    return UDEMath.GetNaturalCubicSplineCurve(points);
                case CurveType.CATMULL_ROM:
                    return UDEMath.GetCatmullRomSplineCurve(points);
                default:
                    return null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (points == null)
                return;

            Gizmos.color = Color.white;

            try
            {
                switch (type)
                {
                    case CurveType.BEZIER:
                        var bezier = UDEMath.GetBezierCurve(points);
                        for (int i = 0; i < precision; i++)
                            Gizmos.DrawLine(bezier((float)i / precision).ToVector2(), bezier((float)(i + 1) / precision).ToVector2());
                        break;
                    case CurveType.CUBIC_SPLINE:
                        var spline = UDEMath.GetNaturalCubicSplineCurve(points);
                        for (int i = 0; i < precision; i++)
                            Gizmos.DrawLine(spline((float)i / precision).ToVector2(), spline((float)(i + 1) / precision).ToVector2());
                        break;
                    case CurveType.CATMULL_ROM:
                        var catmull = UDEMath.GetCatmullRomSplineCurve(points);
                        for (int i = 0; i < precision; i++)
                            Gizmos.DrawLine(catmull((float)i / precision).ToVector2(), catmull((float)(i + 1) / precision).ToVector2());
                        break;
                }
            }
            catch (System.Exception) { }

            Gizmos.color = Color.yellow;
            for (int i = 0; i < points.Length - 1; i++)
                Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}