using System.Collections;
using System.Collections.Generic;

using SansyHuman.Enemy;
using SansyHuman.Experiment.Lua;
using SansyHuman.Pattern;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UI.HUD;
using UnityEngine;

namespace SansyHuman.Stage
{
    public class Stage1 : UDEBaseStagePattern
    {
        // Enemy codes
        public const int WhiteSpirit = 0;
        public const int Fairy = 1;
        public const int FairyRed = 2;
        public const int RedSpirit = 3;
        public const int BlueSpirit = 4;
        public const int GreenSpirit = 5;

        // Sub boss codes
        public const int SheroSub = 0;

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
            Intro, Wave1, Wave2, Wave3, Wave4, Wave5
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
                case Wave.Wave3:
                    goto WAVE_3;
                case Wave.Wave4:
                    goto WAVE_4;
                case Wave.Wave5:
                    goto WAVE_5;
            }

        INTRO:
            // Intro

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1f, UDETime.TimeScale.ENEMY));

            EnemyBase.DropPowerItem[] whiteSpiritPowers = new EnemyBase.DropPowerItem[2];
            whiteSpiritPowers[0] = new EnemyBase.DropPowerItem() { power = 0.01f, number = 4, initSpeed = 2 };
            whiteSpiritPowers[1] = new EnemyBase.DropPowerItem() { power = 0.05f, number = 1, initSpeed = 2 };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    LambdaPatternEnemy spirit = SummonEnemy(enemies[WhiteSpirit]) as LambdaPatternEnemy;
                    spirit.DropPowerItems = whiteSpiritPowers;
                    spirit.ScoreOnDeath = 1500;
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

            EnemyBase.DropPowerItem[] fairyPowers = new EnemyBase.DropPowerItem[2];
            fairyPowers[0] = new EnemyBase.DropPowerItem() { power = 0.01f, number = 3, initSpeed = 2 };
            fairyPowers[1] = new EnemyBase.DropPowerItem() { power = 0.03f, number = 2, initSpeed = 2 };

            for (int i = 0; i < fairySummonPositions.Length; i++)
            {
                LambdaPatternEnemy fairy = SummonEnemy(enemies[Fairy]) as LambdaPatternEnemy;
                fairy.DropPowerItems = fairyPowers;
                fairy.ScoreOnDeath = 3000;
                fairy.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(fairySummonPositions[i]), Quaternion.Euler(0, 0, 0));
                fairy.Initialize(Wave1FairyPattern, 9, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[LightBlueLuminusBullet]);
                fairy.ShowHealthAndSpecllCount();
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(2.25f, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1.5f, UDETime.TimeScale.ENEMY));


        WAVE_2:
            // Wave 2

            LambdaPatternEnemy fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.ScoreOnDeath = 6500;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(5f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.ScoreOnDeath = 6500;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.7f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(5f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.ScoreOnDeath = 6500;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.3f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(4f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.ScoreOnDeath = 6500;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.6f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(4f, UDETime.TimeScale.ENEMY));

            fairyRed = SummonEnemy(enemies[FairyRed]) as LambdaPatternEnemy;
            fairyRed.ScoreOnDeath = 6500;
            fairyRed.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.4f, 0)), Quaternion.Euler(0, 0, 0));
            fairyRed.Initialize(Wave2FairyPattern, 45, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullets[GreenCircleBullet], bullets[RedCircleBullet], bullets[LightBlueCircleBullet]);
            fairyRed.ShowHealthAndSpecllCount();

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(8f, UDETime.TimeScale.ENEMY));

            UDEObjectManager.Instance.DestroyAllEnemies();

        WAVE_3:
            // Wave 3

            WarningHUD warning = GameObject.Find("WarningHUD").GetComponent<WarningHUD>();
            warning.Warn();

            EnemyBase sheroSub = SummonEnemy(subBoss[SheroSub]) as EnemyBase;
            sheroSub.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.4f, 0)), Quaternion.Euler(0, 0, 0));
            sheroSub.Initialize();
            sheroSub.ShowHealthAndSpecllCount();

            yield return StartCoroutine(new WaitUntil(() => sheroSub.SpecialSpellCount == 0));

        WAVE_4:
            // Wave 4

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1.5f, UDETime.TimeScale.ENEMY));

            var fairySummonPosAndWaitTimes = new[]
            {
                new {pos = new Vector3(1.1f, 0.5f), time = 1.6f},
                new {pos = new Vector3(1.1f, 0.65f), time = 2.6f},
                new {pos = new Vector3(1.1f, 0.4f), time = 1.0f},
                new {pos = new Vector3(1.1f, 0.28f), time = 1.4f},
                new {pos = new Vector3(1.1f, 0.75f), time = 3.0f},
                new {pos = new Vector3(1.1f, 0.89f), time = 0.8f},
                new {pos = new Vector3(1.1f, 0.66f), time = 1.4f},
                new {pos = new Vector3(1.1f, 0.32f), time = 1.8f},
                new {pos = new Vector3(1.1f, 0.18f), time = 2.4f},
                new {pos = new Vector3(1.1f, 0.46f), time = 1.8f},
                new {pos = new Vector3(1.1f, 0.55f), time = 0.6f},
                new {pos = new Vector3(1.1f, 0.5f), time = 0f}
            };

            EnemyBase.DropPowerItem[] fairyPowersWave4 = new EnemyBase.DropPowerItem[2];
            fairyPowersWave4[0] = new EnemyBase.DropPowerItem() { power = 0.01f, number = 3, initSpeed = 2 };
            fairyPowersWave4[1] = new EnemyBase.DropPowerItem() { power = 0.03f, number = 1, initSpeed = 2 };

            for (int i = 0; i < fairySummonPosAndWaitTimes.Length; i++)
            {
                LambdaPatternEnemy fairy = SummonEnemy(enemies[Fairy]) as LambdaPatternEnemy;
                fairy.DropPowerItems = fairyPowersWave4;
                fairy.ScoreOnDeath = 3500;
                fairy.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(fairySummonPosAndWaitTimes[i].pos), Quaternion.Euler(0, 0, 0));
                fairy.Initialize(Wave4FairyPattern, 10, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, BulletMap.Instance["DarkGreenEllipseBullet"]);
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(fairySummonPosAndWaitTimes[i].time, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(8f, UDETime.TimeScale.ENEMY));

        WAVE_5:
            for (int i = 0; i < 40; i++)
            {
                LambdaPatternEnemy redSpirit = SummonEnemy(enemies[RedSpirit]) as LambdaPatternEnemy;
                redSpirit.ScoreOnDeath = 1500;
                redSpirit.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(0.9f + 0.12f * Mathf.Sin(i), 1.1f)), Quaternion.Euler(0, 0, 0));
                redSpirit.Initialize(
                    (LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy) => Wave5RedSpiritPattern(pattern, bullets, enemy, i),
                    0, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, BulletMap.Instance["OrangeBrightLight"]
                    );

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.6f, UDETime.TimeScale.ENEMY));
            }

            yield return null;
        }

        private IEnumerator IntroWhiteSpiritPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            var trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-7, 0.6f), 6, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            for (int i = 0; i < 5; i++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.SummonTime = 0.08f;
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
                bullet.SummonTime = 0.12f;
                bullet.Initialize(enemyTr.position + new Vector3(x, y), enemyTr.position, 0, enemy, pattern, builder.Angle(moveAngle).Build());
            }

            builder.Angle(angle);
            for (int i = 1; i < 6; i++)
            {
                (float x, float y) = UDEMath.Polar2Cartesian(0.4f, angle);

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.SummonTime = 0.12f;
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
                bullet.SummonTime = 0.05f;
                bullet.transform.localScale = new Vector3(0.85f, 0.85f);
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, upper);

                bullet = UDEBulletPool.Instance.GetBullet(bullets[1]); // Lower
                bullet.SummonTime = 0.05f;
                bullet.transform.localScale = new Vector3(0.85f, 0.85f);
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
                bullet.SummonTime = 0.05f;
                bullet.transform.localScale = new Vector3(0.85f, 0.85f);
                bullet.Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, upper);

                lower.angle = 180 + angle * 2f;
                bullet = UDEBulletPool.Instance.GetBullet(bullets[2]);
                bullet.SummonTime = 0.05f;
                bullet.transform.localScale = new Vector3(0.85f, 0.85f);
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

        private IEnumerator Wave4FairyPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
        {
            var transitionResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-3, 0), 1f, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => transitionResult.EndTransition);

            UDECartesianMovementBuilder builder1 = UDECartesianMovementBuilder.Create()
                .Velocity(new Vector2(-3f, 0)).EndTime(0.7f);
            UDEPolarMovementBuilder builder2 = UDEPolarMovementBuilder.Create(true)
                .StartTime(0.7f).EndTime(1.5f)
                .MaxAngularSpeed(12).MinAngularSpeed(-12);
            UDECartesianMovementBuilder builder3 = UDECartesianMovementBuilder.Create(true)
                .StartTime(1.5f);

            UDEBulletMovement[] movement1 =
            {
                builder1.Build(),
                builder2.AngularAccel(60).Build(),
                builder3.Build()
            };

            UDEBulletMovement[] movement2 =
            {
                builder1.Build(),
                builder2.AngularAccel(0).Build(),
                builder3.Build()
            };

            UDEBulletMovement[] movement3 =
            {
                builder1.Build(),
                builder2.AngularAccel(-60).Build(),
                builder3.Build()
            };

            Transform enemyTr = enemy.transform;

            for (int i = 0; i < 5; i++)
            {
                UDEBulletPool pool = UDEBulletPool.Instance;

                UDEAbstractBullet[] summonBullets =
                {
                    pool.GetBullet(bullets[0]), pool.GetBullet(bullets[0]), pool.GetBullet(bullets[0])
                };
                summonBullets[0].SummonTime = 0;
                summonBullets[1].SummonTime = 0;
                summonBullets[2].SummonTime = 0;
                summonBullets[0].transform.localScale = new Vector3(0.7f, 0.7f);
                summonBullets[1].transform.localScale = new Vector3(0.7f, 0.7f);
                summonBullets[2].transform.localScale = new Vector3(0.7f, 0.7f);

                summonBullets[0].Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, movement1);
                summonBullets[1].Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, movement2);
                summonBullets[2].Initialize(enemyTr.position, enemyTr.position, 0, enemy, pattern, movement3);

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.3f, UDETime.TimeScale.ENEMY));
            }

            Vector2 movePoint = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0));
            movePoint.y = enemyTr.position.y;

            var trResult = UDETransitionHelper.MoveTo(enemy.gameObject, movePoint, 8f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, true);
            yield return new WaitUntil(() => trResult.EndTransition);

            ((EnemyBase)enemy).OnDestroy();
            Destroy(enemy);

            yield return null;
        }

        private IEnumerator Wave5RedSpiritPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy, int enemyIndex)
        {
            Vector3 initViewPos = Camera.main.WorldToViewportPoint(enemy.transform.position);
            Vector3 finalViewPos = initViewPos;
            finalViewPos.x -= 0.13f;
            finalViewPos.y = -0.1f;

            var transitionResult = UDETransitionHelper.MoveTo(enemy.gameObject, (Vector2)Camera.main.ViewportToWorldPoint(finalViewPos), 3f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, true);

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            UDEMersenneRandom random = new UDEMersenneRandom(seed: (uint)(351237 * enemyIndex));

            for (int i = 0; i < 3; i++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
                bullet.SummonTime = 0.05f;

                Vector3 enemyPos = enemy.gameObject.transform.position;
                Vector3 playerPos = GameManager.player.gameObject.transform.position;
                Vector2 velocity = (Vector2)(playerPos - enemyPos);
                velocity = velocity.normalized;
                velocity.y += random.NextFloat(-0.1f, 0.1f);
                velocity *= 4f;
                bullet.MoveBulletToDirection(enemy, pattern, enemyPos, 0, velocity);

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.66f, UDETime.TimeScale.ENEMY));
            }

            yield return new WaitUntil(() => transitionResult.EndTransition);


            ((EnemyBase)enemy).OnDestroy();
            Destroy(enemy);

            yield return null;
        }
    }
}