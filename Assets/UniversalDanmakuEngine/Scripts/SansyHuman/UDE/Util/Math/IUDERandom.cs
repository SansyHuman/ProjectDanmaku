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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SansyHuman.UDE.Util.Math
{
    /// <summary>
    /// Interface of all random classes.
    /// </summary>
    public interface IUDERandom
    {
        /// <summary>
        /// Gets a next random integer.
        /// </summary>
        /// <returns>Random integer</returns>
        int NextInt();

        /// <summary>
        /// Gets a next random integer in range <paramref name="min"/> <= value < <paramref name="max"/>.
        /// </summary>
        /// <param name="min">Underbound of the random number</param>
        /// <param name="max">Upperbound of the random number</param>
        /// <returns>Random integer</returns>
        int NextInt(int min, int max);

        /// <summary>
        /// Gets a next random long integer.
        /// </summary>
        /// <returns>Random long integer</returns>
        long NextLong();

        /// <summary>
        /// Gets a next long random integer in range <paramref name="min"/> <= value < <paramref name="max"/>.
        /// </summary>
        /// <param name="min">Underbound of the random number</param>
        /// <param name="max">Upperbound of the random number</param>
        /// <returns>Random long integer</returns>
        long NextLong(long min, long max);

        /// <summary>
        /// Gets a next random float.
        /// </summary>
        /// <returns>Random float</returns>
        float NextFloat();

        /// <summary>
        /// Gets a next random float in range <paramref name="min"/> <= value < <paramref name="max"/>.
        /// </summary>
        /// <param name="min">Underbound of the random number</param>
        /// <param name="max">Upperbound of the random number</param>
        /// <returns>Random float</returns>
        float NextFloat(float min, float max);
    }
}