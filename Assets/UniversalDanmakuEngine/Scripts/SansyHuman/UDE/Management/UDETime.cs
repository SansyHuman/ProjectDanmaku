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

namespace SansyHuman.UDE.Management
{
    /// <summary>
    /// Class to control the time scale.
    /// </summary>
    [AddComponentMenu("UDE/Management/Time")]
    public class UDETime : UDESingleton<UDETime>
    {

        [SerializeField] private float enemyTimeScale = 1f;
        [SerializeField] private float playerTimeScale = 1f;
        private bool paused = false;

        /// <value>Gets whether the game has paused.</value>
        public bool Paused => paused;

        /// <value>Gets and sets the time scale of all enemies and bullets from enemies.</value>
        public float EnemyTimeScale
        {
            get => enemyTimeScale;
            set
            {
                if (value < 0)
                    enemyTimeScale = 0;
                else
                    enemyTimeScale = value;
            }
        }

        /// <value>Gets and sets the time scale of the player and bullets from the player.</value>
        public float PlayerTimeScale
        {
            get => playerTimeScale;
            set
            {
                if (value < 0)
                    playerTimeScale = 0;
                else
                    playerTimeScale = value;
            }
        }

        /// <summary>
        /// Enum of time scale.
        /// <list type="table">
        /// <item>
        /// <term><see cref="ENEMY"/></term>
        /// <description>Time scale of enemies.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PLAYER"/></term>
        /// <description>Time scale of players.</description>
        /// </item>
        /// <item>
        /// <term><see cref="UNSCALED"/></term>
        /// <description>Unscaled time.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum TimeScale
        {
            /// <summary>Enemy time scale.</summary>
            ENEMY,
            /// <summary>Player time scale.</summary>
            PLAYER,
            /// <summary>Unscaled time.</summary>
            UNSCALED
        }

        /// <summary>
        /// Gets the time scale of the type.
        /// </summary>
        /// <param name="scaleType">Type of the time scale</param>
        /// <returns>Time scale of the type</returns>
        public float GetTimeScale(TimeScale scaleType)
        {
            switch(scaleType)
            {
                case TimeScale.ENEMY:
                    return enemyTimeScale;
                case TimeScale.PLAYER:
                    return playerTimeScale;
                case TimeScale.UNSCALED: default:
                    return 1;
            }
        }

        /// <summary>
        /// Returns coroutine that pauses for given time in given time scale.
        /// To use, write <c>yield return StartCoroutine(UDETime.WaitForScaledSeconds(time, scale))</c> in a coroutine to pause.
        /// </summary>
        /// <param name="time">Time to pause coroutine in seconds</param>
        /// <param name="scale">Time scale to use</param>
        /// <returns>Coroutine that pauses for <paramref name="time"/> seconds in <paramref name="scale"/> time scale</returns>
        public IEnumerator WaitForScaledSeconds(float time, TimeScale scale)
        {
            float remainingTime = time;

            while (true)
            {
                switch (scale)
                {
                    case TimeScale.ENEMY:
                        remainingTime -= Time.deltaTime * enemyTimeScale;
                        break;
                    case TimeScale.PLAYER:
                        remainingTime -= Time.deltaTime * playerTimeScale;
                        break;
                    case TimeScale.UNSCALED:
                        remainingTime -= Time.deltaTime;
                        break;
                    default:
                        remainingTime -= Time.deltaTime;
                        break;
                }
                yield return null;

                if (remainingTime <= 0)
                    break;
            }
        }

        private float enemyTimeScaleFormal = 1f;
        private float playerTimeScaleFormal = 1f;

        /// <summary>
        /// Pauses the game. Sets all time scales 0.
        /// <para>If you pause the game when the game is paused, the pause will be ignored.</para>
        /// </summary>
        public void PauseGame()
        {
            if (paused)
                return;

            enemyTimeScaleFormal = enemyTimeScale; playerTimeScaleFormal = playerTimeScale;
            paused = true; enemyTimeScale = 0f; playerTimeScale = 0f;
        }

        /// <summary>
        /// Resumes the game. Sets all time scales to values before pause.
        /// <para>If you resume the game when the game is running, the resume will be ignored.</para>
        /// </summary>
        public void ResumeGame()
        {
            if (!paused)
                return;

            paused = false; enemyTimeScale = enemyTimeScaleFormal; playerTimeScale = playerTimeScaleFormal;
        }

        private float enemyTimeScaleTmp = 1f;
        private float playerTimeScaleTmp = 1f;

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                enemyTimeScaleTmp = enemyTimeScale; playerTimeScaleTmp = playerTimeScale;
                enemyTimeScale = 0f; playerTimeScale = 0f;
            }
            else
            {
                enemyTimeScale = enemyTimeScaleTmp; playerTimeScale = playerTimeScaleTmp;
            }
        }
    }
}