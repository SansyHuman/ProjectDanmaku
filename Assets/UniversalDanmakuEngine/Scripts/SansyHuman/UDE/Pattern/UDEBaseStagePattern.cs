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
using SansyHuman.UDE.Exception;

namespace SansyHuman.UDE.Pattern
{
    /// <summary>
    /// Base class of stage patterns. In this class, enemies are summoned and their patterns start.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UDEBaseStagePattern : MonoBehaviour
    {
        /// <summary>All enemies who will be summoned in this stage.</summary>
        [SerializeField] protected List<UDEEnemy> enemies;
        /// <summary>All sub boss characters who will be summoned in this stage.</summary>
        [SerializeField] protected List<UDEEnemy> subBoss;
        /// <summary>All boss characters who will be summoned in this stage.</summary>
        [SerializeField] protected List<UDEEnemy> boss;

        /// <summary>Stage pattern coroutine.</summary>
        protected IEnumerator pattern = null;
        /// <summary>Whether the pattern is active.</summary>
        protected bool stagePatternOn = false;

        /// <value>Gets whether the pattern is running on.</value>
        public bool StagePatternOn { get => stagePatternOn; }

        /// <summary>
        /// Starts the stage. If you have paused the pattern, it restarts from where you have paused.
        /// </summary>
        public virtual void StartStage()
        {
            if (stagePatternOn)
            {
                Debug.LogError("The stage pattern is already running on. StartStage is ignored.");
                return;
            }

            if (pattern == null)
                pattern = StagePattern();
            StartCoroutine(pattern);
            stagePatternOn = true;
        }

        /// <summary>
        /// Pauses the stage.
        /// </summary>
        public virtual void PauseStage()
        {
            StopCoroutine(pattern);
            stagePatternOn = false;
        }

        /// <summary>
        /// Resets the stage.
        /// </summary>
        public virtual void ResetStage()
        {
            if (stagePatternOn)
            {
                Debug.LogError("The stage pattern is running on. Cannot reset the pattern.");
                return;
            }

            pattern = StagePattern();
        }

        /// <summary>
        /// Method that represents the stage pattern. You can create various custom stage patterns by implementing this method.
        /// </summary>
        /// <returns>Enumerator that represents the stage pattern</returns>
        protected abstract IEnumerator StagePattern();

        /// <summary>
        /// Summons the enemy.
        /// </summary>
        /// <param name="enemy">Enemy prefab to summon</param>
        /// <returns>Summoned enemy</returns>
        protected UDEEnemy SummonEnemy(UDEEnemy enemy)
        {
            UDEEnemy summonedEnemy = Instantiate<UDEEnemy>(enemy);
            summonedEnemy.gameObject.SetActive(true);
            return summonedEnemy;
        }
    }
}