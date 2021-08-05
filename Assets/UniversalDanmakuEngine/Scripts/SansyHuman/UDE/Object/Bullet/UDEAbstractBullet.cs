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
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Management;
using UnityEngine;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Abstract class of all bullets.
    /// </summary>
    public abstract class UDEAbstractBullet : UDEInitializable
    {
        /// <summary><see cref="GameObject"/> of the bullet. Only assigned internally.</summary>
        protected GameObject self;
        /// <summary><see cref="Transform"/> of the bullet. Only assigned internally.</summary>
        protected Transform bulletTr;
        /// <summary>Character who shot the bullet.</summary>
        [SerializeField] protected UDEBaseCharacter originCharacter;
        /// <summary>Pattern where summoned the bullet.</summary>
        [SerializeField] protected UDEBaseShotPattern originShotPattern;

        /// <summary>Damage dealt to enemy. 
        /// <para>This value only used for player's bullets. All enemy bullets deals 1 damage to player.</para>
        /// </summary>
        [SerializeField] protected float damage;

        private UDEAbstractBullet originPrefab; // Original prefab of the bullet.

        [Obsolete("Only used internally.")]
        internal UDEAbstractBullet OriginPrefab
        {
            get => originPrefab;
            set => originPrefab = value;
        }

        /// <value>Gets and sets the damage of the bullet.</value>
        public float Damage
        {
            get => damage;
            set
            {
                damage = value;
                if (damage < 0)
                    damage = 0;
            }
        }

        /// <value>Sets the summon time.</value>
        public abstract float SummonTime
        {
            get; set;
        }
        /// <value>Gets and sets the timescale of the bullet.</value>
        public abstract UDETime.TimeScale TimeScale 
        {
            get; set; 
        }

        /// <value>Gets the character that shot the bullet.</value>
        public UDEBaseCharacter OriginCharacter { get => originCharacter; }

        protected virtual void Awake()
        {
            self = gameObject;
            bulletTr = transform;
        }

        protected virtual void OnEnable()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.AddBullet(this);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected virtual void OnDisable()
        {
            if (originShotPattern != null)
                _ = originShotPattern - this;
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.RemoveBullet(this);
#pragma warning restore CS0618 // Type or member is obsolete

            bulletTr.localScale = new Vector3(1, 1, 1);
            initialized = false;
        }

        /// <summary>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// </summary>
        /// <param name="initPos">Initial position of the bullet</param>
        /// <param name="origin">Origin in polar coordinate</param>
        /// <param name="initRotation">Initial rotation of the bullet</param>
        /// <param name="originCharacter"><see cref="SansyHuman.UDE.Object.UDEBaseCharacter"/> instance which shot the bullet</param>
        /// <param name="originShotPattern"><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance which summoned the bullet. It is nullable</param>
        /// <param name="movements">Movements of the bullet. All <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> should be in the order of phase</param>
        /// <param name="setOriginToCharacter">Whether set the origin in polar coordinate to origin character's position</param>
        /// <param name="loop">Whether turn back to first movement when the last movement end</param>
        public virtual void Initialize(Vector2 initPos, Vector2 origin, float initRotation, UDEBaseCharacter originCharacter, UDEBaseShotPattern originShotPattern, UDEBulletMovement[] movements, bool setOriginToCharacter = false, bool loop = false)
        {
            initialized = true;
        }

        /// <summary>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// </summary>
        /// <param name="initPos">Initial position of the bullet</param>
        /// <param name="origin">Origin in polar coordinate</param>
        /// <param name="initRotation">Initial rotation of the bullet</param>
        /// <param name="originCharacter"><see cref="SansyHuman.UDE.Object.UDEBaseCharacter"/> instance which shot the bullet</param>
        /// <param name="originShotPattern"><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance which summoned the bullet. It is nullable</param>
        /// <param name="movement">Movement of the bullet.</param>
        /// <param name="setOriginToCharacter">Whether set the origin in polar coordinate to origin character's position</param>
        /// <param name="loop">Whether turn back to first movement when the last movement end</param>
        public abstract void Initialize(Vector2 initPos, Vector2 origin, float initRotation, UDEBaseCharacter originCharacter, UDEBaseShotPattern originShotPattern, in UDEBulletMovement movement, bool setOriginToCharacter = false, bool loop = false);
    }
}