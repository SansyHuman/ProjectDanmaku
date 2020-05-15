using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

public class TestBossPattern3 : UDEBaseShotPattern
{
    [SerializeField] private int groupCount = 8;
    [SerializeField] private int bulletsPerGroup = 6;

    protected override IEnumerator ShotPattern()
    {
        originEnemy.CanBeDamaged = false;
        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.85f)), 2f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => result.EndTransition);
        originEnemy.CanBeDamaged = true;

        UDEPolarMovementBuilder[] builders = new UDEPolarMovementBuilder[3];

        builders[0] = UDEPolarMovementBuilder.Create().RadialSpeed(4.5f);
        builders[1] = UDEPolarMovementBuilder.Create(true).RadialAccel(25f).MaxRadialSpeed(10f).StartTime(0.3f);
        builders[2] = UDEPolarMovementBuilder.Create(true).RadialAccel(-22f).MinRadialSpeed(0.6f).StartTime(0.65f);

        float angleInterval = 360f / groupCount;
        float smallAngleInterval = angleInterval / bulletsPerGroup * 1.25f;
        int flag = 0;
        while (true)
        {
            float deviation = Random.Range(-15f, 15f);
            for (int i = 0; i < bulletsPerGroup; i++)
            {
                UDEAbstractBullet[] bullets = new UDEAbstractBullet[groupCount];
                for (int n = 0; n < bullets.Length; n++)
                    bullets[n] = UDEBulletPool.Instance.GetBullet(patternBullets[0]);

                for (int j = 0; j < groupCount; j++)
                {
                    float angle;
                    if (flag % 2 == 0)
                        angle = deviation + angleInterval * j + smallAngleInterval * i;
                    else
                        angle = deviation + angleInterval * j + smallAngleInterval * (bulletsPerGroup - 1 - i);

                    UDEBulletMovement[] movements = new UDEBulletMovement[builders.Length];
                    for (int k = 0; k < builders.Length; k++)
                    {
                        builders[k].InitialAngle(angle);
                        movements[k] = builders[k].Build();
                    }

                    Vector2 origin = originEnemy.transform.position;
                    Vector2 formLoc = origin + UDEMath.Polar2Cartesian(0.45f, angle).ToVector2();

                    bullets[j].Initialize(formLoc, origin, 0, originEnemy, this, movements);
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.18f, UDETime.TimeScale.ENEMY));
            }

            flag++;
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.7f, UDETime.TimeScale.ENEMY));
        }
    }
}
