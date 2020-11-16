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

using System.Collections;
using UnityEngine;
using SansyHuman.UDE.Management;
using System;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Base class of all characters.
    /// <para>If you want to add a character who fires laser, implement <see cref="SansyHuman.UDE.Object.IUDELaserFirable"/>.</para>
    /// </summary>
    public abstract class UDEPlayer : UDEBaseCharacter, IUDEControllable
    {
        /// <summary>Movement speed of the player.</summary>
        [SerializeField] protected float speed;
        /// <summary>A value lower than 1 multiplied to speed when the character is in slow mode
        /// (Similar to slow mode in Touhou project when pressing left shift).</summary>
        [SerializeField] protected float slowModeSpeedMultiplier;
        /// <summary>Interval to fire the bullets.</summary>
        [SerializeField] protected float bulletFireInterval;

        /// <summary>Key assigned to moving up.</summary>
        [SerializeField] protected KeyCode moveUp = KeyCode.UpArrow;
        /// <summary>Key assigned to moving down.</summary>
        [SerializeField] protected KeyCode moveDown = KeyCode.DownArrow;
        /// <summary>Key assigned to moving left.</summary>
        [SerializeField] protected KeyCode moveLeft = KeyCode.LeftArrow;
        /// <summary>Key assigned to moving right.</summary>
        [SerializeField] protected KeyCode moveRight = KeyCode.RightArrow;
        /// <summary>Key assigned to shooting bullets.</summary>
        [SerializeField] protected KeyCode shoot = KeyCode.Z;
        /// <summary>Key assigned to slowing down the player.</summary>
        [SerializeField] protected KeyCode slowMode = KeyCode.LeftShift;

        /// <summary>Struct that contains informations of key mapping</summary>
        public struct KeyMappingInfo
        {
            public KeyCode moveUp;
            public KeyCode moveDown;
            public KeyCode moveLeft;
            public KeyCode moveRight;
            public KeyCode shoot;
            public KeyCode slowMode;
        }

        /// <summary>Whether the player cannot be damaged.</summary>
        protected bool invincible = false;
        private IEnumerator shotCoroutine;

        /// <summary>Implementation of <see cref="SansyHuman.UDE.Object.IUDEControllable.Speed"/>.</summary>
        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                if (speed < 0)
                    speed = 0;
            }
        }

        /// <summary>Implementation of <see cref="SansyHuman.UDE.Object.IUDEControllable.SlowModeSpeedMultiplier"/>.</summary>
        public float SlowModeSpeedMultiplier
        {
            get => slowModeSpeedMultiplier;
            set
            {
                slowModeSpeedMultiplier = value;
                if (slowModeSpeedMultiplier < 0)
                    slowModeSpeedMultiplier = 0;
                if (slowModeSpeedMultiplier > 1)
                    slowModeSpeedMultiplier = 1;
            }
        }

        /// <summary>Implementation of <see cref="SansyHuman.UDE.Object.IUDEControllable.BulletFireInterval"/>.</summary>
        public float BulletFireInterval
        {
            get => bulletFireInterval;
            set
            {
                bulletFireInterval = value;
                if (bulletFireInterval < 0)
                    bulletFireInterval = 0;
            }
        }

        /// <summary>
        /// Sets the key mapping of the player.
        /// </summary>
        /// <param name="info">Key mapping informations</param>
        public void SetKeyMapping(KeyMappingInfo info)
        {
            moveUp = info.moveUp;
            moveDown = info.moveDown;
            moveLeft = info.moveLeft;
            moveRight = info.moveRight;
            slowMode = info.slowMode;
            shoot = info.shoot;
        }

        /// <summary>
        /// Implementation of the <see cref="SansyHuman.UDE.Object.IUDEControllable.Move(float)"/>. Only called internally in <see cref="UDEPlayer.Update()"/>.
        /// </summary>
        /// <param name="deltaTime">Time passed from the last call</param>
        [Obsolete("This method is called only internally.")]
        public void Move(float deltaTime)
        {
            float realSpeed = Speed;
            if (Input.GetKey(slowMode))
                realSpeed *= slowModeSpeedMultiplier;

            Vector3 velocity = new Vector3();
            if (Input.GetKey(moveRight))
                velocity += Vector3.right * realSpeed;
            if (Input.GetKey(moveLeft))
                velocity += Vector3.left * realSpeed;
            if (Input.GetKey(moveUp))
                velocity += Vector3.up * realSpeed;
            if (Input.GetKey(moveDown))
                velocity += Vector3.down * realSpeed;

            characterTr.Translate(velocity * deltaTime * UDETime.Instance.PlayerTimeScale);

            Vector3 pos = Camera.main.WorldToViewportPoint(characterTr.position);
            if (pos.x < 0)
                pos.x = 0;
            if (pos.x > 1)
                pos.x = 1;
            if (pos.y < 0)
                pos.y = 0;
            if (pos.y > 1)
                pos.y = 1;
            characterTr.position = Camera.main.ViewportToWorldPoint(pos);
        }

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.IUDEControllable.ShootBullet()"/>.
        /// </summary>
        /// <returns>Coroutine that shoots the bullet every <see cref="BulletFireInterval"/> seconds when fire button is pressed</returns>
        public abstract IEnumerator ShootBullet();

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDEBaseCharacter.Awake()"/>.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            shotCoroutine = ShootBullet();
            StartCoroutine(shotCoroutine);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Move(Time.deltaTime);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (invincible)
                return;

            if (collision.CompareTag("Enemy"))
                StartCoroutine(DamageSelf(1));
            else if (collision.CompareTag("Bullet"))
            {
                UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEEnemy)
                {
                    UDEBulletPool.Instance.ReleaseBullet(bullet);
                    StartCoroutine(DamageSelf(1));
                }
            }
            else if (collision.CompareTag("Laser"))
            {
                UDELaser laser = collision.GetComponent<UDELaser>();
                if (laser != null && laser.OriginCharacter is UDEEnemy)
                {
                    StartCoroutine(DamageSelf(1));
                }
            }
        }

        private IEnumerator DamageSelf(float damage)
        {
            health -= damage;
            invincible = true;
            SpriteRenderer renderer = self.GetComponent<SpriteRenderer>();
            Color col = renderer.color;
            col.a = 0.5f;
            renderer.color = col;
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(3f, UDETime.TimeScale.PLAYER));
            col.a = 1f;
            renderer.color = col;
            invincible = false;
        }
    }
}