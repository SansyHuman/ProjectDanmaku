using System.Collections;
using System.Collections.Generic;

using SansyHuman.Enemy;
using SansyHuman.Pattern;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

using UnityEngine;

namespace SansyHuman.Stage
{
    public class Stage1 : UDEBaseStagePattern
    {
        // Enemy codes
        public const int WhiteSpirit = 0;
        public const int Fairy = 1;

        // Bullet codes
        public const int GreenLuminusBullet = 0;
        public const int LightBlueLuminusBullet = 0;

        [SerializeField]
        private UDEAbstractBullet[] bullets;

        protected override IEnumerator StagePattern()
        {
            // Intro

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    LambdaPatternEnemy spirit = SummonEnemy(enemies[WhiteSpirit]) as LambdaPatternEnemy;
                    spirit.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.05f + 0.2f * j + 0.1f * i, 0)), Quaternion.Euler(0, 0, 0));
                    spirit.Initialize(IntroWhiteSpiritPattern, 1, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenLuminusBullet]);
                    yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.1f, UDETime.TimeScale.ENEMY));
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.6f, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(9f, UDETime.TimeScale.ENEMY));

            // Wave 1

            Vector3[] fairySummonPositions =
            {
                new Vector3(1.1f, 0.7f),
                new Vector3(1.1f, 0.2f),
                new Vector3(1.1f, 0.8f),
                new Vector3(1.1f, 0.5f),
                new Vector3(1.1f, 0.25f),
                new Vector3(1.1f, 0.69f),
                new Vector3(1.1f, 0.73f),
                new Vector3(1.1f, 0.35f)
            };

            for (int i = 0; i < fairySummonPositions.Length; i++)
            {
                LambdaPatternEnemy fairy = SummonEnemy(enemies[Fairy]) as LambdaPatternEnemy;
                fairy.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(fairySummonPositions[i]), Quaternion.Euler(0, 0, 0));
                fairy.Initialize(Wave1FairyPattern, 9, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[LightBlueLuminusBullet]);
                fairy.ShowHealthAndSpecllCount();
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(2.25f, UDETime.TimeScale.ENEMY));
            }
        }

        private IEnumerator IntroWhiteSpiritPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            var trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-7, 0.6f), 6, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            for (int i = 0; i < 5; i++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.MoveBulletToDirection(enemy, pattern, enemy.transform.position, 0, new Vector2(-5, 0), true);

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.8f, UDETime.TimeScale.ENEMY));
            }

            yield return new WaitUntil(() => trResult.EndTransition);

            trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-6, 15), 6, UDETransitionHelper.easeInQuad, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => trResult.EndTransition);

            ((EnemyBase)enemy).OnDestroy();
            Destroy(enemy);

            yield return null;
        }

        private IEnumerator Wave1FairyPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            var trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-3, 0), 0.3f, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => trResult.EndTransition);

            Transform enemyTr = enemy.transform;
            Transform player = GameManager.player.transform;
            Vector2 direction = player.position - enemyTr.position;
            direction /= direction.magnitude;

            (_, float angle) = UDEMath.Cartesian2Polar(direction);

            UDECartesianPolarMovementBuilder builder = UDECartesianPolarMovementBuilder.Create().Speed(9.0f).MinSpeed(3.0f).TangentialAccel(-17.0f);

            int bulletCnt = 25;
            float delta = 360f / bulletCnt;

            for (int i = 0; i < bulletCnt; i++)
            {
                float moveAngle = angle + delta * i;
                (float x, float y) = UDEMath.Polar2Cartesian(0.4f, moveAngle);

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.Initialize(enemyTr.position + new Vector3(x, y), enemyTr.position, 0, enemy, pattern, builder.Angle(moveAngle).Build());
            }

            builder.Angle(angle);
            for (int i = 1; i < 6; i++)
            {
                (float x, float y) = UDEMath.Polar2Cartesian(0.4f, angle);

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.Initialize(enemyTr.position + new Vector3(x, y), enemyTr.position, 0, enemy, pattern, builder.MinSpeed(3.0f + 0.6f * i).Build());
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            Vector2 movePoint = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0));
            movePoint.y = enemyTr.position.y;

            trResult = UDETransitionHelper.MoveTo(enemy.gameObject, movePoint, 10f, UDETransitionHelper.easeInCubic, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => trResult.EndTransition);

            ((EnemyBase)enemy).OnDestroy();
            Destroy(enemy);

            yield return null;
        }
    }
}