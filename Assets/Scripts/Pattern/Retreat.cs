using System.Collections;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util;
using UnityEngine;

namespace SansyHuman.Pattern
{
    public class Retreat : ShotPatternBase
    {
        [SerializeField]
        [Tooltip("Position to first transition in viewport space")]
        private Vector2 firstTransitionPosition;

        [SerializeField]
        [Tooltip("First transition time")]
        private float firstTransitionTime;

        [SerializeField]
        [Tooltip("First transition ease type")]
        private UDETransitionHelper.EaseType firstTransitionEaseType;

        [SerializeField]
        [Tooltip("Time between first and second transition")]
        private float restTime;

        [SerializeField]
        [Tooltip("Position to second transition in viewport space")]
        private Vector2 secondTransitionPosition;

        [SerializeField]
        [Tooltip("Second transition time")]
        private float secondTransitionTime;

        [SerializeField]
        [Tooltip("Second transition ease type")]
        private UDETransitionHelper.EaseType secondTransitionEaseType;

        protected override IEnumerator ShotPattern()
        {
            originEnemy.CanBeDamaged = false;
            var transitionResult = UDETransitionHelper.MoveTo(
                originEnemy.gameObject,
                Camera.main.ViewportToWorldPoint(firstTransitionPosition),
                firstTransitionTime,
                UDETransitionHelper.EaseTypeOf(firstTransitionEaseType),
                UDETime.TimeScale.ENEMY,
                false);
            yield return new WaitUntil(() => transitionResult.EndTransition);

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(restTime, UDETime.TimeScale.ENEMY));

            transitionResult = UDETransitionHelper.MoveTo(
                originEnemy.gameObject,
                Camera.main.ViewportToWorldPoint(secondTransitionPosition),
                secondTransitionTime,
                UDETransitionHelper.EaseTypeOf(secondTransitionEaseType),
                UDETime.TimeScale.ENEMY,
                false);
            yield return new WaitUntil(() => transitionResult.EndTransition);

            GameManager.player.AddScore(originEnemy.ScoreOnDeath);

            Destroy(originEnemy.gameObject);
        }
    }
}