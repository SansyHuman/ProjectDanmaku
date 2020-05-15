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
using Unity.Mathematics;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Base class of all characters, such as enemies and player.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UDEBaseCharacter : UDEInitializable
    {
        /// <summary>Health of the character.</summary>
        [SerializeField] protected float health;
        /// <summary>Whether the character is alive.</summary>
        protected bool alive;

        /// <value>Gets whether the character is alive.</value>
        public bool Alive { get => alive; }
        /// <value>Gets the health of the character.</value>
        public float Health { get => health; }

        /// <summary><see cref="GameObject"/> of the character.</summary>
        protected GameObject self;
        /// <summary><see cref="Transform"/> of the character.</summary>
        protected Transform characterTr;

        /// <summary>Position of the character. This value is only used internally by ECS.</summary>
        public float3 position;

        /// <summary>
        /// Initializes the character when it summoned.
        /// </summary>
        protected virtual void Awake()
        {
            self = gameObject;
            characterTr = transform;
            alive = true;
        }

        private void FixedUpdate()
        {
            position = characterTr.position;
        }

        /// <summary>
        /// Called when the character's health becomes below 0.
        /// </summary>
        public virtual void OnDeath()
        {
            self.SetActive(false);
        }
    }
}