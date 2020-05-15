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
using Unity.Entities;
using SansyHuman.UDE.Management;
using UnityEngine;

namespace SansyHuman.UDE.ECS.Object
{
    /// <summary>
    /// Component that contains informations of movement of the bullet.
    /// </summary>
    [Serializable]
    public struct UDEBulletMovements : IComponentData
    {
        /// <summary>Movement of the bullet.</summary>
        public UDEBulletMovementECS Movement;
        /// <summary>Phase of the bullet.</summary>
        public int Phase;
        /// <summary>Total phase of the bullet.</summary>
        public int TotalPhase;

        /// <summary>Time passed from the initialization.</summary>
        public float Time;
        /// <summary>Time scale the bullet uses.</summary>
        public UDETime.TimeScale UsingTimeScale;

        /// <summary>Whether the bullet is in summon phase.</summary>
        public bool IsSummoning;
        /// <summary>Time the bullet is in summon phase.</summary>
        public float SummonTime;

        /// <summary>Whether go back to first phase when last phase ends.</summary>
        public bool Loop;
    }

    /// <summary>
    /// Proxy of <see cref="UDEBulletMovements"/>.
    /// </summary>
    [AddComponentMenu("UDE/Experimental/ECS/Component Data/Bullet Movements Proxy")]
    public class UDEBulletMovementsComponent : ComponentDataProxy<UDEBulletMovements> { }
}