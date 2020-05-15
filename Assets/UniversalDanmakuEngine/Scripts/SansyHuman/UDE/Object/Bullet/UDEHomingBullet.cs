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
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util.Math;
using System.Collections.ObjectModel;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// The class of homing bullet. Only used by player(since if enemies have homing bullets, player cannot avoid them).
    /// </summary>
    [AddComponentMenu("UDE/Bullet/Homing Bullet")]
    public class UDEHomingBullet : UDEBaseBullet
    {
        /// <summary>Homing power of the bullet.</summary>
        [SerializeField] protected float homingPower = 0f;

        /// <value>Gets and sets the homing power of the bullet.</value>
        public float HomingPower
        { 
            get => homingPower;
            set
            {
                homingPower = value;
                if (homingPower < 0)
                    homingPower = 0;
            }
        }

        /// <summary>
        /// Initializes the bullet.
        /// </summary>
        /// <param name="initPos">The initial position of the bullet</param>
        /// <param name="originCharacter">Player who shot the bullet</param>
        /// <param name="initialMovement">Sets the initial direction and speed of the bullet</param>
        public void Initialize(Vector2 initPos, UDEBaseCharacter originCharacter, UDEBulletMovement initialMovement)
        {
            Initialize(initPos, initPos, 0, originCharacter, null, initialMovement);
        }

        private Camera mainCamera;

        protected override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            Vector3 pos = mainCamera.WorldToViewportPoint(bulletTr.position);
            if (pos.x < -0.1f || pos.x > 1.1f || pos.y < -0.1f || pos.y > 1.1f)
                UDEBulletPool.Instance.ReleaseBullet(this);
            if (time > 3f)
                UDEBulletPool.Instance.ReleaseBullet(this);
        }

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDEBaseBullet.MoveBullet(float)"/>.
        /// </summary>
        /// <param name="deltaTime">
        /// <para>The passed time from the previous frame.</para>
        /// <para>It is recommended to pass <see cref="UnityEngine.Time.deltaTime"/></para>
        /// </param>
        protected override void MoveBullet(float deltaTime)
        {
            ReadOnlyCollection<UDEEnemy> enemies = UDEObjectManager.Instance.GetAllEnemies();
            int closestEnemy = -1;
            Vector2 closestEnemyPosition = Vector2.zero;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].CanBeDamaged)
                    continue;

                Vector2 enemyPosition = new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.y);
                float distance = (this.position - enemyPosition).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = i;
                    closestEnemyPosition = enemyPosition;
                }
            }

            float scaledDeltaTime = deltaTime * UDETime.Instance.PlayerTimeScale;

            if (closestEnemy >= 0)
            {
                float enemyAngle = UDEMath.Deg(closestEnemyPosition - this.position);
                float extraHomingPower = enemyAngle > 90 ? (enemyAngle - 45) / 120f : 1;
                movements[0].angle = Mathf.LerpAngle(movements[0].angle, enemyAngle, homingPower * extraHomingPower * scaledDeltaTime);
            }

            float dx = movements[0].speed * Mathf.Cos(movements[0].angle * Mathf.Deg2Rad) * scaledDeltaTime;
            float dy = movements[0].speed * Mathf.Sin(movements[0].angle * Mathf.Deg2Rad) * scaledDeltaTime;
            Vector2 displacement = new Vector2(dx, dy);
            this.position += displacement;
            bulletTr.SetPositionAndRotation(this.position, Quaternion.Euler(0, 0, movements[0].angle));
            time += scaledDeltaTime;
            movements[0].updateCount++;
        }
    }
}