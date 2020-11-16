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
    /// Random number generator that uses linear congruential.
    /// </summary>
    public sealed class UDEModRandom : IUDERandom
    {
        private readonly uint seed;
        private ulong x;
        private readonly ulong a;
        private readonly ulong c;
        private readonly ulong m;

        /// <summary>
        /// Initializes the generator. The next random number B is
        /// (<paramref name="a"/> * A + <paramref name="c"/>) mod <paramref name="m"/>
        /// where A is the current random number.
        /// The default value of <paramref name="a"/>, <paramref name="c"/>, and
        /// <paramref name="m"/> are 1103515245, 12345, and 4294967296(ANSI C standard)
        /// </summary>
        /// <param name="seed">Seed of the generator</param>
        /// <param name="a">a</param>
        /// <param name="c">c</param>
        /// <param name="m">m</param>
        public UDEModRandom(uint? seed = null, ulong a = 1103515245ul, ulong c = 12345ul, ulong m = 4294967296ul)
        {
            if (!seed.HasValue)
            {
                this.seed =
                    (uint)System.DateTime.UtcNow.Millisecond
                    * (uint)Thread.CurrentThread.ManagedThreadId
                    * (uint)Process.GetCurrentProcess().Id;
            }
            else
                this.seed = seed.Value;

            x = (ulong)this.seed;
            this.a = a;
            this.c = c;
            this.m = m;
        }

        private uint ExtractNumber()
        {
            x = x * a + c;
            x = x % m;
            return (uint)x;
        }

        /// <inheritdoc/>
        public int NextInt()
        {
            return (int)ExtractNumber();
        }

        /// <inheritdoc/>
        public int NextInt(int min, int max)
        {
            ulong ext = ExtractNumber();
            ulong interv = (ulong)((long)max - (long)min);
            ext = ext % interv;
            return min + (int)ext;
        }

        /// <inheritdoc/>
        public long NextLong()
        {
            return (long)ExtractNumber() + ((long)ExtractNumber() << 32);
        }

        /// <inheritdoc/>
        public long NextLong(long min, long max)
        {
            ulong ext = (ulong)ExtractNumber() + ((ulong)ExtractNumber() << 32);
            ulong interv = (ulong)((long)max - (long)min);
            if (interv == 0)
                return (long)ext;

            ext = ext % interv;
            return min + (long)ext;
        }

        /// <inheritdoc/>
        public float NextFloat()
        {
            uint ext = ExtractNumber();
            return BitConverter.ToSingle(BitConverter.GetBytes(ext), 0);
        }

        /// <inheritdoc/>
        public float NextFloat(float min, float max)
        {
            uint ext = ExtractNumber();
            float interv = max - min;
            float extf = interv * (float)ext / (float)uint.MaxValue;
            return min + extf;
        }
    }
}