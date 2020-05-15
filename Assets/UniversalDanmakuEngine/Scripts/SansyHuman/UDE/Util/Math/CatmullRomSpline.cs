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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
using static SansyHuman.UDE.Util.Math.UDEMath;

namespace SansyHuman.UDE.Util.Math
{
    [Obsolete("Only used internally.")]
    internal class CatmullRomSpline
    {
        private Vector2[] points;
        private float[] t;
        private float tRange;
        private int numPt;

        private readonly float alpha = 0.5f;

        internal CatmullRomSpline(params Vector2[] points)
        {
            if (points.Length < 4)
                throw new ArgumentException("The number of points is less than 4.", nameof(points));
            this.points = points;
            numPt = this.points.Length;

            t = new float[numPt];
            t[0] = 0;
            for (int i = 1; i < numPt; i++)
                t[i] = t[i - 1] + Pow(Sqrt((points[i] - points[i - 1]).sqrMagnitude), alpha);

            tRange = t[numPt - 2] - t[1];
        }

        internal CartesianCoord Eval(float t) // t is between 0 and 1.
        {
            if (t < 0 || t > 1)
                throw new ArgumentException("t is not in range", nameof(t));

            float realT = this.t[1] + t * tRange;
            int zero = 0;
            for (int i = 2; i < numPt - 2; i++)
            {
                if (realT <= this.t[i])
                    break;

                zero++;
            }

            int one = zero + 1;
            int two = zero + 2;
            int three = zero + 3;

            float int10 = this.t[one] - this.t[zero];
            float int21 = this.t[two] - this.t[one];
            float int32 = this.t[three] - this.t[two];

            Vector2 A1 = (this.t[one] - realT) * points[zero] + (realT - this.t[zero]) * points[one];
            A1 /= int10;
            Vector2 A2 = (this.t[two] - realT) * points[one] + (realT - this.t[one]) * points[two];
            A2 /= int21;
            Vector2 A3 = (this.t[three] - realT) * points[two] + (realT - this.t[two]) * points[three];
            A3 /= int32;

            float int20 = this.t[two] - this.t[zero];
            float int31 = this.t[three] - this.t[one];

            Vector2 B1 = (this.t[two] - realT) * A1 + (realT - this.t[zero]) * A2;
            B1 /= int20;
            Vector2 B2 = (this.t[three] - realT) * A2 + (realT - this.t[one]) * A3;
            B2 /= int31;

            Vector2 C = (this.t[two] - realT) * B1 + (realT - this.t[one]) * B2;
            C /= int21;

            return new CartesianCoord(C.x, C.y);
        }
    }
}