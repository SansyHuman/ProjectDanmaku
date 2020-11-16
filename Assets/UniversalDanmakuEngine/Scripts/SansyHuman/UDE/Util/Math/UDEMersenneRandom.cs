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
    /// Random number generator that uses Mersenne twister. It uses 32-bit Mersenne twister.
    /// </summary>
    public sealed class UDEMersenneRandom : IUDERandom
    {
        /// <summary>
        /// The coeffitients of Mersenne twister.
        /// </summary>
        public struct MersenneRandomInit
        {
            /// <summary>
            /// Degree of recurrence.
            /// </summary>
            public uint n;

            /// <summary>
            /// Middle word, an offset used in the recurrence relation defining the series.
            /// </summary>
            public uint m;

            /// <summary>
            /// Separation point of one word, or the number of bits of the lower bitmask.
            /// </summary>
            public int r;

            /// <summary>
            /// Coefficients of the rational normal form twist matrix
            /// </summary>
            public uint a;

            /// <summary>
            /// TGFSR(R) tempering bitmask.
            /// </summary>
            public uint b;

            /// <summary>
            /// TGFSR(R) tempering bitmask.
            /// </summary>
            public uint c;

            /// <summary>
            /// TGFSR(R) tempering bit shift.
            /// </summary>
            public int s;

            /// <summary>
            /// TGFSR(R) tempering bit shift.
            /// </summary>
            public int t;

            /// <summary>
            /// Additional Mersenne Twister tempering bit shift/mask.
            /// </summary>
            public int u;

            /// <summary>
            /// Additional Mersenne Twister tempering bit shift/mask.
            /// </summary>
            public uint d;

            /// <summary>
            /// Additional Mersenne Twister tempering bit shift/mask.
            /// </summary>
            public uint l;

            /// <summary>
            /// Coefficient used when initializing the generator.
            /// </summary>
            public uint f;
        }

        /// <summary>
        /// Default Mersenne coefficients setting. The default setting is MT19937, where
        /// <para>(n, m, r) = (624, 397, 31)</para>
        /// <para>a = 0x9908b0df</para>
        /// <para>(u, d) = (11, 0xffffffff)</para>
        /// <para>(s, b) = (7, 0x9d2c5680)</para>
        /// <para>(t, c) = (15, 0xefc60000)</para>
        /// <para>l = 18</para>
        /// <para>f = 1812433253</para>
        /// </summary>
        public static readonly MersenneRandomInit DEFAULT;

        static UDEMersenneRandom()
        {
            DEFAULT = new MersenneRandomInit()
            {
                n = 624,
                m = 397,
                r = 31,
                a = 0x9908b0df,
                b = 0x9d2c5680,
                c = 0xefc60000,
                s = 7,
                t = 15,
                u = 11,
                d = 0xffffffff,
                l = 18,
                f = 1812433253u
            };
        }

        private const int w = 32;
        private readonly uint seed;
        private readonly MersenneRandomInit init;

        private uint[] mt;
        private uint index;
        private readonly uint lowerMask;
        private readonly uint upperMask;

        /// <summary>
        /// Initializes the generator.
        /// </summary>
        /// <param name="seed">Seed of the generator</param>
        /// <param name="init">Settings of the Mersenne twister. The default value is
        /// MT19937.</param>
        public UDEMersenneRandom(uint? seed = null, MersenneRandomInit? init = null)
        {
            if (!init.HasValue)
                this.init = DEFAULT;
            else
                this.init = init.Value;

            if (!seed.HasValue)
            {
                this.seed =
                    (uint)System.DateTime.UtcNow.Millisecond
                    * (uint)Thread.CurrentThread.ManagedThreadId
                    * (uint)Process.GetCurrentProcess().Id;
            }
            else
                this.seed = seed.Value;

            mt = new uint[this.init.n];
            index = this.init.n + 1;
            lowerMask = (1u << this.init.r) - 1;
            upperMask = ~lowerMask;

            SeedMT();
        }

        private void SeedMT()
        {
            index = init.n;
            mt[0] = seed;
            for (uint i = 1; i < init.n; i++)
                mt[i] = init.f * (mt[i - 1] ^ (mt[i - 1] >> (w - 2))) + i;
        }

        private uint ExtractNumber()
        {
            if (index >= init.n)
            {
                if (index > init.n)
                    throw new InvalidOperationException("Generator was never seeded");

                Twist();
            }

            uint y = mt[index];
            y = y ^ ((y >> init.u) & init.d);
            y = y ^ ((y << init.s) & init.b);
            y = y ^ ((y << init.t) & init.c);
            y = y ^ (y >> 1);

            index++;
            return y;
        }

        private void Twist()
        {
            for (uint i = 0; i < init.n; i++)
            {
                uint x = (mt[i] & upperMask) + (mt[(i + 1) % init.n] & lowerMask);
                uint xA = x >> 1;
                if ((x % 2) != 0)
                    xA = xA ^ init.a;
                mt[i] = mt[(i + init.m) % init.n] ^ xA;
            }

            index = 0;
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