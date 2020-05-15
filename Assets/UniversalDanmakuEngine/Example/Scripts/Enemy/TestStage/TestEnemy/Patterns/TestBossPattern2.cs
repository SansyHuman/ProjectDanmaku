using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

public class TestBossPattern2 : UDEBaseShotPattern
{
    protected override IEnumerator ShotPattern()
    {
        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), 2, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
        originEnemy.CanBeDamaged = false;
        yield return new WaitUntil(() => result.EndTransition);
        originEnemy.CanBeDamaged = true;

        StartCoroutine(GravityBullets());

        UDECartesianPolarMovementBuilder builder = UDECartesianPolarMovementBuilder.Create().Speed(4f);
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                Vector2 enemyPosition = originEnemy.transform.position;
                float angle = UDEMath.Deg(playerPosition - enemyPosition);

                Vector2 origin = enemyPosition;
                Vector2 formOriginDispl = UDEMath.Polar2Cartesian(0.3f, angle).ToVector2();
                Vector2 formOrigin = origin + formOriginDispl;
                builder.Angle(angle);
                UDEBulletMovement movement = builder.Build();

                Vector2 formHalfDispl = UDEMath.Polar2Cartesian(0.105f, angle + 90).ToVector2();

                for (int n = 1; n <= 6; n++)
                {
                    if (n % 2 == 0)
                    {
                        for (int j = 0; j < n / 2; j++)
                        {
                            UDEAbstractBullet negBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                            UDEAbstractBullet posBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                            negBullet.Initialize(formOrigin - (formHalfDispl * (j * 2 + 1)), origin, 0, originEnemy, this, movement);
                            posBullet.Initialize(formOrigin + (formHalfDispl * (j * 2 + 1)), origin, 0, originEnemy, this, movement);
                        }
                    }
                    else
                    {
                        UDEAbstractBullet middleBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                        middleBullet.Initialize(formOrigin, origin, 0, originEnemy, this, movement);
                        for (int j = 0; j < n / 2; j++)
                        {
                            UDEAbstractBullet negBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                            UDEAbstractBullet posBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                            negBullet.Initialize(formOrigin - (formHalfDispl * (j + 1) * 2), origin, 0, originEnemy, this, movement);
                            posBullet.Initialize(formOrigin + (formHalfDispl * (j + 1) * 2), origin, 0, originEnemy, this, movement);
                        }
                    }

                    yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.11f, UDETime.TimeScale.ENEMY));
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.24f, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(2f, UDETime.TimeScale.ENEMY));
        }
    }

    private IEnumerator GravityBullets()
    {
        while (true)
        {
            UDEMath.CartesianCoord accelDown = new UDEMath.CartesianCoord(0, -3);
            UDEMath.CartesianCoord accelUp = new UDEMath.CartesianCoord(0, 3);
            UDECartesianMovementBuilder builder = UDECartesianMovementBuilder.Create().MaxMagnitude(6f);

            for (int i = 0; i < 23; i++)
            {
                float angle = 90f + Random.Range(-55f, 55f);
                var velTuple = UDEMath.Polar2Cartesian(3f, angle);
                Vector2 velocity = new Vector2(velTuple.x, velTuple.y);
                

                Vector2 origin = originEnemy.transform.position;
                Vector2 player = GameObject.FindGameObjectWithTag("Player").transform.position;
                if (player.y > origin.y)
                    builder.Velocity(-velocity).Accel(accelUp);
                else
                    builder.Velocity(velocity).Accel(accelDown);
                UDEBulletMovement movement = builder.Build();

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                bullet.Initialize(origin, origin, 0, originEnemy, this, movement);
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.75f, UDETime.TimeScale.ENEMY));
        }
    }
}