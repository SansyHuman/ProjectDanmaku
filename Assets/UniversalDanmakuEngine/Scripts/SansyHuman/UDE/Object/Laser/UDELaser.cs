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
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Exception;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Base class of all lasers.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UDELaser : UDEInitializable
    {
        /// <summary><see cref="LineRenderer"/> which draws the path of the laser.</summary>
        protected LineRenderer laser;
        /// <summary>Collider of the laser.</summary>
        protected EdgeCollider2D laserCollider;

        /// <summary>Origin of local position of the laser in world space.
        /// Positions of the laser's points are relative positions from the origin.</summary>
        protected Vector2 origin;
        /// <summary>Character who fired the laser.</summary>
        protected UDEBaseCharacter originCharacter;
        /// <summary>Whether set the origin of the laser to the origin of the origin character.</summary>
        protected bool followOriginCharacter;

        /// <summary>Head of the laser.
        /// The parent of the laser head is the laser object.
        /// For every call of <seealso cref="ExtendLaser(float)"/>, the local position of the laser head
        /// is added to the positions of <see cref="LineRenderer"/>.</summary>
        protected GameObject laserHead;
        /// <summary>Positions of the <see cref="LineRenderer"/>.</summary>
        protected List<Vector2> points;
        /// <summary>Time scale to use.</summary>
        protected UDETime.TimeScale timeScale;

        /// <summary>Time passed from the initialization.</summary>
        protected float time;

        /// <summary>Ratio of width of the collider to the width of the laser.</summary>
        [SerializeField] protected float colliderWidthMultiplier = 0.7f;
        /// <summary>Damage per second.</summary>
        [SerializeField] protected float dps = 1f;

        /// <summary>Delegate of ExtendLaser(float).</summary>
        protected UDEObjectManager.ObjectMoveHandler extendHandler;
        /// <summary>Transform of the laser.</summary>
        protected Transform laserTr;

        /// <value>Gets the character who fired the laser.</value>
        public UDEBaseCharacter OriginCharacter { get => originCharacter; }

        /// <value>Gets and sets the ratio of width of the collider to the width of the laser.</value>
        public float ColliderWidthMultiplier
        {
            get => colliderWidthMultiplier;
            set
            {
                colliderWidthMultiplier = value;
                if (colliderWidthMultiplier < 0)
                    colliderWidthMultiplier = 0;
            }
        }

        /// <value>Gets and sets the damage per second(dps) of the laser.</value>
        public float Dps
        {
            get => dps;
            set
            {
                dps = value;
                if (dps < 0)
                    dps = 0;
            }
        }

        private void Awake()
        {
            laser = GetComponent<LineRenderer>();
            laserCollider = GetComponent<EdgeCollider2D>();
            laser.useWorldSpace = false;
            laserCollider.edgeRadius = laser.widthMultiplier * colliderWidthMultiplier * 0.5f;

            laserTr = transform;
            laserTr.position = Vector3.zero;

            extendHandler = new UDEObjectManager.ObjectMoveHandler(ExtendLaser);
        }

        /// <summary>
        /// Initializes the laser.
        /// </summary>
        /// <param name="origin">Origin of the local position of the laser in world space</param>
        /// <param name="originCharacter">Character who fired the laser</param>
        /// <param name="followOriginCharacter">If true, changes the origin of the laser to origin character's position</param>
        /// <param name="initialLocalHeadLocation">Start position of the laser in local space</param>
        /// <param name="timeScale">Time scale to use</param>
        protected virtual void Initialize(Vector2 origin,
                                          UDEBaseCharacter originCharacter,
                                          bool followOriginCharacter,
                                          Vector2 initialLocalHeadLocation,
                                          UDETime.TimeScale timeScale)
        {
            if (initialized)
            {
                Debug.LogWarning("You tried to initialize laser that already initialized. Initialization is ignored.");
                return;
            }

            laserTr.position = origin;
            this.origin = origin;
            this.originCharacter = originCharacter;
            this.followOriginCharacter = followOriginCharacter;

            laserHead = new GameObject("Laser Head");
            laserHead.transform.parent = transform;
            laserHead.transform.localPosition = initialLocalHeadLocation;

            points = new List<Vector2>(128);
            points.Add(initialLocalHeadLocation);
            laser.positionCount = 1;
            laser.SetPosition(0, points[0]);
            laserCollider.points = points.ToArray();

            UDEObjectManager.Instance.MoveObjects += extendHandler;

            this.timeScale = timeScale;
            time = 0;

            initialized = true;
        }

        /// <summary>
        /// Destroys the laser.
        /// </summary>
        public virtual void DestroyLaser()
        {
            UDEObjectManager.Instance.MoveObjects -= extendHandler;
            Destroy(gameObject);
        }

        /// <summary>
        /// Enables and disables the collider of the laser.
        /// </summary>
        /// <param name="enabled">Whether the collider will be enabled</param>
        public void SetColliderEnabled(bool enabled)
        {
            laserCollider.enabled = enabled;
        }

        /// <summary>
        /// Sets the width of the laser.
        /// </summary>
        /// <param name="width">Width of the laser</param>
        public void SetLaserWidth(float width)
        {
            laser.widthMultiplier = width;
        }

        /// <summary>
        /// Only used internally in <see cref="SansyHuman.UDE.Management.UDEObjectManager.FixedUpdate()"/>. Update the laser.
        /// </summary>
        /// <param name="deltaTime">Delta time from the last call</param>
        protected virtual void ExtendLaser(float deltaTime)
        {
            if (followOriginCharacter)
                laserTr.position = originCharacter.transform.position;

            laserCollider.edgeRadius = laser.widthMultiplier * colliderWidthMultiplier * 0.5f;
        }

        private void OnEnable()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.AddLaser(this);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void OnDisable()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.RemoveLaser(this);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}