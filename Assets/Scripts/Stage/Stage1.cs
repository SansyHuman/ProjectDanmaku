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
        public const int FairyRed = 2;

        // Bullet codes
        public const int GreenLuminusBullet = 0;
        public const int LightBlueLuminusBullet = 1;
        public const int GreenCircleBullet = 2;
        public const int RedCircleBullet = 3;
        public const int LightBlueCircleBullet = 4;

        [SerializeField]
        private UDEAbstractBullet[] bullets;

        public enum Wave
        {
            Intro, Wave1, Wave2
        }

        [SerializeField]
        private Wave startWave = Wave.Intro;

        protected override IEnumerator StagePattern()
        {
            switch (startWave)
            {
                case Wave.Intro:
                    goto INTRO;
                case Wave.Wave1:
                    goto WAVE_1;
                case Wave.Wave2:
                    goto WAVE_2;
            }

            INTRO:
            // Intro

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1f, UDETime.TimeScale.ENEMY));

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

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(13.5f, UDETime.TimeScale.ENEMY));

            WAVE_1:
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

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1.5f, UDETime.TimeScale.ENEMY));


            WAVE_2:
            // Wave 2

            LambdaPatternEnemy fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(5f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.7f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(5f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.3f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(4f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.6f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(4f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.4f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();
        }

        private IEnumerator IntroWhiteSpiritPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            var trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-7, 0.6f), 6, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            for (int i = 0; i < 5; i++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                ((UDEBaseBullet)bullet).SummonTime = 0.08f;
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
                ((UDEBaseBullet)bullet).SummonTime = 0.12f;
                bullet.Initialize(enemyTr.position + new Vector3(x, y), enemyTr.position, 0, enemy, pattern, builder.Angle(moveAngle).Build());
            }

            builder.Angle(angle);
            for (int i = 1; i < 6; i++)
            {
                (float x, float y) = UDEMath.Polar2Cartesian(0.4f, angle);

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                ((UDEBaseBullet)bullet).SummonTime = 0.12f;
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

        private IEnumerator Wave2FairyPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-3, 0), 1f, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.3f, UDETime.TimeScale.ENEMY));

            Transform enemyTr = enemy.transform;

            float angle = 25;
            var easeInOut = UDETransitionHelper.easeInOutQuad.Composite(t => t / 4);
            float dt = 0.4f;

            UDECartesianPolarMovementBuilder builder = UDECartesianPolarMovementBuilder.Create().Speed(4.5f).Angle(180);
            UDEBulletMovement central = builder.Build();

            float accTime = 0;
            for (int i = 0; i < (int)(8 / dt); i++)
            {
                if (accTime <= 4)
                    angle = 25 - 50 * easeInOut(accTime);
                else
                    angle = -25 + 50 * easeInOut(accTime - 4);

                UDEBulletMovement upper = builder.Angle(180 - angle).Build();
                UDEBulletMovement lower = builder.Angle(180 + angle).Build();

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]); // Upper
                ((UDEBaseBullet)bullet).SummonTime = 0.05f;
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, upper);

                bullet = UDEBulletPool.Instance.GetBullet(bullets[1]); // Lower
                ((UDEBaseBullet)bullet).SummonTime = 0.05f;
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, lower);

                float accTimeD = accTime + 1f;
                if (accTimeD >= 8)
                    accTimeD -= 8;

                if (accTimeD <= 4)
                    angle = 25 - 50 * easeInOut(accTimeD);
                else
                    angle = -25 + 50 * easeInOut(accTimeD - 4);

                upper.angle = 180 - angle * 2f;
                bullet = UDEBulletPool.Instance.GetBullet(bullets[2]);
                ((UDEBaseBullet)bullet).SummonTime = 0.05f;
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, upper);

                lower.angle = 180 + angle * 2f;
                bullet = UDEBulletPool.Instance.GetBullet(bullets[2]);
                ((UDEBaseBullet)bullet).SummonTime = 0.05f;
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, lower);

                accTime += dt;
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(dt, UDETime.TimeScale.ENEMY));
            }

            Vector2 movePoint = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0));
            movePoint.y = enemyTr.position.y;

            var trResult = UDETransitionHelper.MoveTo(enemy.gameObject, movePoint, 6f, UDETransitionHelper.easeInQuad, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => trResult.EndTransition);

            ((EnemyBase)enemy).OnDestroy();
            Destroy(enemy);

            yield return null;
        }
    }
}