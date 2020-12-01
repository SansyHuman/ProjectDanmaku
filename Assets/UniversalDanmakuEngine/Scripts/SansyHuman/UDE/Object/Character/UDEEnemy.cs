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
using UnityEngine;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Management;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// Class of all enemies.
    /// </summary>
    [AddComponentMenu("UDE/Character/Enemy")]
    public class UDEEnemy : UDEBaseCharacter
    {
        /// <summary>
        /// Struct contains a shot pattern and it's health.
        /// </summary>
        [Serializable]
        public struct ShotPattern
        {
            public int health;
            public UDEBaseShotPattern shotPattern;
        }

        /// <summary>List of shot patterns enemy will shot.</summary>
        [SerializeField] protected List<ShotPattern> shotPatterns;
        /// <summary>Score when the enemy dies.</summary>
        [SerializeField] protected int scoreOnDeath;
        /// <summary>Whether the enemy is killable.</summary>
        [SerializeField] protected bool canBeDamaged;

        /// <summary>Index of the current shot pattern.</summary>
        protected int currentPhase;

        /// <value>Gets and sets the score added when the enemy dies by a player.</value>
        public int ScoreOnDeath
        {
            get => scoreOnDeath;
            set
            {
                scoreOnDeath = value;
                if (scoreOnDeath < 0)
                    scoreOnDeath = 0;
            }
        }

        /// <value>Gets whether the enemy can be killed.</value>
        public bool CanBeDamaged { get => canBeDamaged; set => canBeDamaged = value; }

        /// <summary>
        /// Initializes the enemy.
        /// </summary>
        public virtual void Initialize()
        {
            if (initialized)
            {
                Debug.LogWarning("You tried to initalize enemy that already has initialized. The initialization is ignored.");
                return;
            }

            alive = true;
            initialized = true;
            StartCoroutine(ManagePatterns());
        }

        /// <summary>
        /// Gets current shot pattern.
        /// </summary>
        /// <returns><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance currently running on.</returns>
        public UDEBaseShotPattern GetCurrentPattern()
        {
            return shotPatterns[currentPhase].shotPattern;
        }

        // Plays registered patterns in order.
        private IEnumerator ManagePatterns()
        {
            for (int i = 0; i < shotPatterns.Count; i++)
            {
                currentPhase = i;
                health = shotPatterns[i].health;
                shotPatterns[i].shotPattern.gameObject.SetActive(true);
                UDEBaseShotPattern pattern = shotPatterns[i].shotPattern;
                pattern.Initialize(this);
                pattern.StartPattern();
                yield return new WaitUntil(() =>
                    (health < 0 || (pattern.HasTimeLimit && pattern.Time > pattern.TimeLimit)));
                pattern.EndPattern();
                OnPatternEnd(pattern);
            }
            alive = false;
            OnDeath();
        }

        /// <summary>
        /// Called when one pattern ends. Override this to do something when pattern ended.
        /// </summary>
        /// <param name="endedPattern">Pattern that has ended</param>
        protected virtual void OnPatternEnd(UDEBaseShotPattern endedPattern)
        {

        }

        /// <summary>
        /// Override method of <see cref="SansyHuman.UDE.Object.UDEBaseCharacter.OnDeath()"/>
        /// </summary>
        public override void OnDeath()
        {
            base.OnDeath();
            StopAllCoroutines();
        }

        // Registers itself to object manager.
        protected virtual void OnEnable()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.AddEnemy(this);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // Deregisters itself from object manager.
        protected virtual void OnDisable()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UDEObjectManager.Instance.RemoveEnemy(this);
#pragma warning restore CS0618 // Type or member is obsolete
            Destroy(self);
        }

        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (!canBeDamaged)
                return;

            if (collision.CompareTag("Bullet"))
            {
                UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEPlayer)
                {
                    health -= bullet.Damage;
                    UDEBulletPool.Instance.ReleaseBullet(bullet);
                }
            }
            if (collision.CompareTag("Laser"))
            {
                UDELaser laser = collision.GetComponent<UDELaser>();
                if (laser != null && laser.OriginCharacter is UDEPlayer)
                {
                    health -= laser.Dps * Time.deltaTime * UDETime.Instance.PlayerTimeScale;
                }
            }
        }
    }
}