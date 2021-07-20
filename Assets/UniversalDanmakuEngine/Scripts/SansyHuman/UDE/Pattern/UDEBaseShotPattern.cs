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
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Exception;
using System;

namespace SansyHuman.UDE.Pattern
{
    /// <summary>
    /// Base class for shot patterns of enemy.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UDEBaseShotPattern : UDEInitializable
    {
        /// <summary>
        /// Enum of the type of the pattern. Use Special only when the pattern is a boss' special pattern.
        /// <list type="table">
        /// <item>
        /// <term><see cref="NORMAL"/></term>
        /// <description>Normal pattern.</description>
        /// </item>
        /// <item>
        /// <term><see cref="SPECIAL"/></term>
        /// <description>Special pattern.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum PatternType
        {
            /// <summary>Normal pattern.</summary>
            NORMAL,
            /// <summary>Special pattern.</summary>
            SPECIAL
        }

        /// <summary>
        /// Enum that represents whether bullets will be removed when the health of the pattern becomes below 0.
        /// <list type="table">
        /// <item>
        /// <term><see cref="NONE"/></term>
        /// <description>Do not remove bullets.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PATTERN"/></term>
        /// <description>Remove bullets from the pattern.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ENTIRE"/></term>
        /// <description>Remove entire bullets.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum RemoveBulletsOnDeath
        {
            /// <summary>Do not remove bullets when the pattern dies.</summary>
            NONE,
            /// <summary>Remove bullets only from the pattern.</summary>
            PATTERN,
            /// <summary>Remove all bullets in entire scene.</summary>
            ENTIRE
        }

        /// <summary>Bullets that are used in the pattern.
        /// Bullet objects should have <see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> or its children as a component.</summary>
        [SerializeField] protected List<UDEAbstractBullet> patternBullets;
        /// <summary>Type of the pattern.</summary>
        [SerializeField] protected PatternType type = PatternType.NORMAL;
        /// <summary>Whether the pattern ends after some time.</summary>
        [SerializeField] protected bool hasTimeLimit;
        /// <summary>Time limit of the pattern.</summary>
        [SerializeField] protected float timeLimit;
        /// <summary>Whether remove the bullet when the pattern dies.</summary>
        /// <seealso cref="RemoveBulletsOnDeath"/>
        [SerializeField] protected RemoveBulletsOnDeath removeBullets = RemoveBulletsOnDeath.NONE;

        /// <summary>Collection of bullets that are shot in this pattern.</summary>
        protected List<UDEAbstractBullet> shottedBullets;
        /// <summary>Enemy object that plays the pattern.</summary>
        /// <remarks>Enemy object should have <see cref="SansyHuman.UDE.Object.UDEEnemy"/> as its component.</remarks>
        protected UDEEnemy originEnemy;
        /// <summary>Passed time from the start of the pattern.</summary>
        protected float time;

        /// <summary>Coroutine of <see cref="UDEBaseShotPattern.ShotPattern()"/>.</summary>
        protected IEnumerator pattern;
        /// <summary>Coroutines of subpatterns.</summary>
        protected List<IEnumerator> subpatterns;
        /// <summary>Whether the pattern is active.</summary>
        protected bool shotPatternOn = false;

        /// <value>Gets the passed time of the pattern.</value>
        public float Time { get => time; }
        /// <value>Gets and sets the type the pattern.</value>
        public PatternType Type { get => type; }
        /// <value>Gets whether the pattern has a time limit.</value>
        public bool HasTimeLimit { get => hasTimeLimit; }
        /// <value>Gets the time limit of the pattern.</value>
        public float TimeLimit { get => timeLimit; }
        /// <value>Gets whether the pattern is running on.</value>
        public bool ShotPatternOn { get => shotPatternOn; }

        /// <summary>
        /// Initializes the pattern. This methods is called in <see cref="SansyHuman.UDE.Object.UDEEnemy.ManagePatterns()"/>.
        /// </summary>
        /// <param name="originEnemy">The enemy object which shots the pattern.</param>
        public virtual void Initialize(UDEEnemy originEnemy)
        {
            if (initialized)
                return;

            shottedBullets = new List<UDEAbstractBullet>(2048);
            subpatterns = new List<IEnumerator>();
            this.originEnemy = originEnemy;
            time = 0;
            pattern = ShotPattern();

            initialized = true;
        }

        private void FixedUpdate()
        {
            if (shotPatternOn)
                time += UnityEngine.Time.deltaTime * UDETime.Instance.EnemyTimeScale;
        }

        /// <summary>
        /// Starts the pattern. This methods is called in <see cref="SansyHuman.UDE.Object.UDEEnemy.ManagePatterns()"/>.
        /// If you have paused the pattern, it restarts from where you have paused.
        /// </summary>
        public virtual void StartPattern()
        {
            if (!initialized)
            {
                Debug.LogError("You did not initalized the shot pattern but you tried to start the pattern.");
                return;
            }
            if (shotPatternOn)
            {
                Debug.LogError("The shot pattern is already running on. StartPattern is ignored.");
                return;
            }

            shotPatternOn = true;
            StartCoroutine(pattern);
            for (int i = 0; i < subpatterns.Count; i++)
                StartCoroutine(subpatterns[i]);
        }

        /// <summary>
        /// Pauses the pattern. The progression of the pattern remains.
        /// </summary>
        public virtual void PausePattern()
        {
            StopCoroutine(pattern);
            for (int i = 0; i < subpatterns.Count; i++)
                StopCoroutine(subpatterns[i]);
            StopAllCoroutines();

            shotPatternOn = false;
        }

        /// <summary>
        /// Resets the pattern.
        /// </summary>
        public virtual void ResetPattern()
        {
            if (shotPatternOn)
            {
                Debug.LogError("The shot pattern is running on. Cannot reset the pattern.");
                return;
            }

            pattern = ShotPattern();
            subpatterns.Clear();
            time = 0;
        }

        /// <summary>
        /// Ends the pattern. This methods is called in <see cref="SansyHuman.UDE.Object.UDEEnemy.ManagePatterns()"/>.
        /// </summary>
        public virtual void EndPattern()
        {
            PausePattern();
            switch (removeBullets)
            {
                case RemoveBulletsOnDeath.NONE:
                    break;
                case RemoveBulletsOnDeath.PATTERN:
                    UDEBulletPool.Instance.ReleaseBullets(shottedBullets.ToArray());
                    break;
                case RemoveBulletsOnDeath.ENTIRE:
                    UDEObjectManager.Instance.DestroyAllBullets();
                    break;
            }

            initialized = false;
        }

        /// <summary>
        /// Adds shotted bullet by this pattern. Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet"/>.
        /// </summary>
        [Obsolete("This method is internal")]
        public void AddBullet(UDEAbstractBullet bullet)
        {
            shottedBullets.Add(bullet);
        }

        /// <summary>
        /// Operator version of <see cref="AddBullet(SansyHuman.UDE.Object.UDEAbstractBullet)"/>.
        /// Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet"/>.
        /// </summary>
        /// <param name="pattern">Pattern to which bullet is added</param>
        /// <param name="bullet">Bullet to add</param>
        /// <returns>0</returns>
        public static int operator +(UDEBaseShotPattern pattern, UDEAbstractBullet bullet)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            pattern.AddBullet(bullet);
#pragma warning restore CS0618 // Type or member is obsolete
            return 0;
        }

        /// <summary>
        /// Removes shotted bullet by this pattern. Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet"/>.
        /// </summary>
        public void RemoveBullet(UDEAbstractBullet bullet)
        {
            shottedBullets.Remove(bullet);
        }

        /// <summary>
        /// Operator version of <see cref="RemoveBullet(SansyHuman.UDE.Object.UDEAbstractBullet)"/>.
        /// Only used internally in <see cref="SansyHuman.UDE.Object.UDEBaseBullet"/>.
        /// </summary>
        /// <param name="pattern">Pattern from which bullet is removed</param>
        /// <param name="bullet">Bullet to remove</param>
        /// <returns>0</returns>
        public static int operator -(UDEBaseShotPattern pattern, UDEAbstractBullet bullet)
        {
            pattern.RemoveBullet(bullet);
            return 0;
        }

        /// <summary>
        /// Starts a subpattern running simultaneously with the main pattern.
        /// The subpattern is registered in <see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern.subpatterns"/> list.
        /// Use this method only inside the <see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern.ShotPattern()"/> coroutine.
        /// </summary>
        /// <param name="subpattern">Subpattern to start</param>
        protected void StartSubpattern(IEnumerator subpattern)
        {
            subpatterns.Add(subpattern);
            StartCoroutine(subpattern);
        }

        /// <summary>
        /// Stops the subpattern and remove it from the list.
        /// Use this method only inside the <see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern.ShotPattern()"/> coroutine.
        /// </summary>
        /// <param name="subpattern">Subpattern to end</param>
        protected void StopSubpattern(IEnumerator subpattern)
        {
            StopCoroutine(subpattern);
            subpatterns.Remove(subpattern);
        }

        /// <summary>
        /// Method that represent the pattern. You can create various custom patterns
        /// implementing this method.
        /// </summary>
        /// <example>
        /// This example shots the pattern similar to 境符「波と粒の境界」 of Yukari Yakumo
        /// from Touhou Project TH09.5 Shoot the Bullet.
        /// <code>
        /// public class TestBossPattern : UDEBaseShotPattern
        /// {
        ///     [SerializeField]
        ///     private int NumberOfBullets = 10;
        ///     [SerializeField]
        ///     private float BulletSpeed = 2f;
        /// 
        ///     protected override IEnumerator ShotPattern()
        ///     {
        ///         float AngleDeg = (360f / NumberOfBullets);
        ///         float AngleRef = 0f;
        ///         float RefOmega = 0f;
        ///         while (true)
        ///         {
        ///             for (int i = 0; i < NumberOfBullets; i++)
        ///             {
        ///                 UDEBaseBullet bullet = UDEBulletPool.Instance.GetBullet(baseBullets[0]);
        ///                 UDEBulletMovement movement = UDEPolarMovementBuilder.Create().RadialSpeed(BulletSpeed).InitialAngle(i * AngleDeg + AngleRef).Build();
        ///                 Vector2 origin = originEnemy.transform.position;
        ///                 var formLocTuple = UDEMath.Polar2Cartesian(0.7f, movement.angle);
        ///                 Vector2 formLocation = new Vector2(formLocTuple.x, formLocTuple.y) + origin;
        ///                 bullet.Initialize(formLocation, origin, 0, originEnemy, this, true, movement);
        ///             }
        ///             AngleRef += RefOmega;
        ///             RefOmega += 0.2f;
        ///             if (RefOmega >= 360)
        ///                 RefOmega -= 360;
        ///             if (AngleRef >= 360)
        ///                 AngleRef -= 360;
        ///             yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.07f, UDETime.TimeScale.ENEMY));
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>Enumerator that represents the pattern</returns>
        protected abstract IEnumerator ShotPattern();
    }
}