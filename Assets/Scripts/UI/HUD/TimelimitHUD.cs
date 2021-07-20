using System.Collections;
using System.Collections.Generic;
using SansyHuman.UDE.Pattern;
using TMPro;
using UnityEngine;

namespace SansyHuman.UI.HUD
{
    [DisallowMultipleComponent]
    public class TimelimitHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI timeLimitText;

        [SerializeField]
        [Tooltip("Outline color of time limit when the remaining time is longer than threshold")]
        private Color normalTimeLimitOutlineColor;

        [SerializeField]
        [Tooltip("Outline width of time limit when the remaining time is longer than threshold")]
        [Range(0, 1)]
        private float normalTimeLimitOutlineWidth = 0.25f;

        [SerializeField]
        [Tooltip("Outline color of time limit when the remaining time is shorter than threshold")]
        private Color warningTimeLimitOutlineColor;

        [SerializeField]
        [Tooltip("OUtline width of time limit when the remaining time is shorter than threshold")]
        [Range(0, 1)]
        private float warningTimeLimitOutlineWidth = 0.35f;

        [SerializeField]
        [Tooltip("Threshold that the outline of the time limit changes from normal color to warning color")]
        [Range(0, 20)]
        private float threshold;

        private UDEBaseShotPattern pattern = null;

        private void Awake()
        {
            StopShowingTimeLimit();
        }

        /// <summary>
        /// Shows the time limit of the shot pattern.
        /// </summary>
        /// <param name="pattern">Shot pattern</param>
        public void ShowTimeLimit(UDEBaseShotPattern pattern)
        {
            if (!pattern.HasTimeLimit)
                return;

            this.pattern = pattern;
            StartCoroutine(TimeLimitUpdate());
        }

        public void StopShowingTimeLimit()
        {
            pattern = null;
            timeLimitText.text = "";
            StopAllCoroutines();
        }

        private IEnumerator TimeLimitUpdate()
        {
            timeLimitText.outlineColor = normalTimeLimitOutlineColor;
            timeLimitText.outlineWidth = normalTimeLimitOutlineWidth;

            while (true)
            {
                float remainingTime = pattern.TimeLimit - pattern.Time;
                if (remainingTime < threshold)
                    break;

                timeLimitText.text = remainingTime.ToString("00.00");
                yield return null;
            }

            timeLimitText.outlineColor = warningTimeLimitOutlineColor;
            timeLimitText.outlineWidth = warningTimeLimitOutlineWidth;

            while (true)
            {
                float remainingTime = pattern.TimeLimit - pattern.Time;
                if (remainingTime < 0)
                    break;

                timeLimitText.text = remainingTime.ToString("00.00");
                yield return null;
            }

            StopShowingTimeLimit();
            yield return null;
        }
    }
}