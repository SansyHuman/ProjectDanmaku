using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

public class TestBossPattern5 : UDEBaseShotPattern
{
    [SerializeField] private int bulletNumber = 4;
    [SerializeField] private float angularSpeed = 10;
    [SerializeField] private float bulletSpeed = 6f;

    protected override IEnumerator ShotPattern()
    {
        originEnemy.CanBeDamaged = false;
        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.85f)), 2f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => result.EndTransition);
        originEnemy.CanBeDamaged = true;

        float angleDiff = 360f / (float)bulletNumber;

        float angleClockwise = 0;
        float angleAntiClockwise = 0;

        UDEPolarMovementBuilder builder = UDEPolarMovementBuilder.Create().RadialSpeed(bulletSpeed);

        while (true)
        {
            for (int i = 0; i < bulletNumber; i++)
            {
                float angle = angleDiff * i + angleClockwise;
                builder.InitialAngle(angle);

                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                Vector2 origin = originEnemy.transform.position;
                Vector2 formLoc = origin + (UDEMath.CartesianCoord)new UDEMath.PolarCoord(0.35f, angle);
                bullet.Initialize(formLoc, origin, 0, originEnemy, this, builder.Build());

                angle = angleDiff * i + angleAntiClockwise;
                builder.InitialAngle(angle);

                UDEAbstractBullet bullet2 = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                formLoc = origin + (UDEMath.CartesianCoord)new UDEMath.PolarCoord(0.35f, angle);
                bullet2.Initialize(formLoc, origin, 0, originEnemy, this, builder.Build());
            }

            angleClockwise -= angularSpeed;
            angleAntiClockwise += angularSpeed;
            while (angleClockwise < 0)
                angleClockwise += 360f;
            while (angleAntiClockwise > 360)
                angleAntiClockwise -= 360f;

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.12f, UDETime.TimeScale.ENEMY));
        }
    }
}
