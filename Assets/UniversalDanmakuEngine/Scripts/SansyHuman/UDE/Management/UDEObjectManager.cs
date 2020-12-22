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
using SansyHuman.UDE.Object;
using System;
using System.Collections.ObjectModel;

namespace SansyHuman.UDE.Management
{
    /// <summary>
    /// Manager class of in-game objects.
    /// </summary>
    [AddComponentMenu("UDE/Management/Object Manager")]
    public class UDEObjectManager : UDESingleton<UDEObjectManager>
    {
        /// <summary>
        /// Delegate of <see cref="SansyHuman.UDE.Object.UDEBaseBullet.MoveBullet(float)"/>
        /// and <see cref="SansyHuman.UDE.Object.UDELaser.ExtendLaser(float)"/>.
        /// </summary>
        /// <param name="deltaTime">Passed time from the last call</param>
        public delegate void ObjectMoveHandler(float deltaTime);

        /// <summary>
        /// Event handler of <see cref="ObjectMoveHandler"/>.
        /// </summary>
        public event ObjectMoveHandler MoveObjects;

        private List<UDEEnemy> enemies;
        private List<UDEAbstractBullet> bullets;
        private List<UDELaser> lasers;

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Management.UDESingleton{UDEObjectManager}.Awake()"/>.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            MoveObjects += delegate (float deltaTime) { };

            enemies = new List<UDEEnemy>(30);
            bullets = new List<UDEAbstractBullet>(2048);
            lasers = new List<UDELaser>(20);
        }

        // Moves all activated bullets.
        private void FixedUpdate()
        {
            MoveObjects(Time.deltaTime);
        }

        /// <summary>
        /// Adds bullet object to manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet.OnEnable()"/>.
        /// </summary>
        /// <param name="bullet">Bullet to add</param>
        [Obsolete("Only used internally")]
        public void AddBullet(UDEAbstractBullet bullet)
        {
            bullets.Add(bullet);
        }

        /// <summary>
        /// Removes bullet object from manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet.OnDisable()"/>.
        /// </summary>
        /// <param name="bullet">Bullet to remove</param>
        [Obsolete("Only used internally")]
        public void RemoveBullet(UDEAbstractBullet bullet)
        {
            bullets.Remove(bullet);
        }

        /// <summary>
        /// Adds enemy object to manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDEEnemy.OnEnable()"/>.
        /// </summary>
        /// <param name="enemy">Enemy to add</param>
        [Obsolete("Only used internally")]
        public void AddEnemy(UDEEnemy enemy)
        {
            enemies.Add(enemy);
        }

        /// <summary>
        /// Removes enemy object from manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDEEnemy.OnDisable()"/>.
        /// </summary>
        /// <param name="enemy">Enemy to remove</param>
        [Obsolete("Only used internally")]
        public void RemoveEnemy(UDEEnemy enemy)
        {
            enemies.Remove(enemy);
        }

        /// <summary>
        /// Adds laser object to manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDELaser.OnEnable()"/>.
        /// </summary>
        /// <param name="laser">Laser to add</param>
        [Obsolete("Only used internally")]
        public void AddLaser(UDELaser laser)
        {
            lasers.Add(laser);
        }

        /// <summary>
        /// Removes laser object from manager. Only used internally in <see cref="SansyHuman.UDE.Object.UDELaser.OnDisable()"/>.
        /// </summary>
        /// <param name="laser"></param>
        [Obsolete("Only used internally")]
        public void RemoveLaser(UDELaser laser)
        {
            lasers.Remove(laser);
        }

        /// <summary>
        /// Disables all bullets that are enabled.
        /// </summary>
        /// <param name="destroyPlayerBullets">
        /// If <see langword="true"/>, disables player bullets also. Else, disables
        /// only enemy bullets. The default value is <see langword="false"/>.
        /// </param>
        public void DestroyAllBullets(bool destroyPlayerBullets = false)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!destroyPlayerBullets && bullets[i].OriginCharacter is UDEPlayer)
                    continue;

                UDEBulletPool.Instance.ReleaseBullet(bullets[i]);
                i--;
            }
        }

        /// <summary>
        /// Destroys all enemies that are alive.
        /// </summary>
        public void DestroyAllEnemies()
        {
            while (enemies.Count > 0)
                enemies[0].OnDeath();
        }

        /// <summary>
        /// Gets all bullets as read only collection.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{UDEBaseBullet}"/></returns>
        public ReadOnlyCollection<UDEAbstractBullet> GetAllBullets() => bullets.AsReadOnly();

        /// <summary>
        /// Gets all enemies as read only collection.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{UDEEnemy}"/></returns>
        public ReadOnlyCollection<UDEEnemy> GetAllEnemies() => enemies.AsReadOnly();

        /// <summary>
        /// Gets all lasers as read only collection.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{UDELaser}"/></returns>
        public ReadOnlyCollection<UDELaser> GetAllLasers() => lasers.AsReadOnly();
    }
}