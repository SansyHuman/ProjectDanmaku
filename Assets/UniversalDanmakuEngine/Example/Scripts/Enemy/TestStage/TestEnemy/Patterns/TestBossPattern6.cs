using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

public class TestBossPattern6 : UDEBaseShotPattern
{
    List<UDEAbstractBullet> bullets = new List<UDEAbstractBullet>(800);

    protected override IEnumerator ShotPattern()
    {
        originEnemy.CanBeDamaged = false;
        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), 2f, UDETransitionHelper.easeInOutCubic, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => result.EndTransition);
        originEnemy.CanBeDamaged = true;

        UDEMath.PolarFunction bulletPosition = (deg, err) => 0.3f + 2.5f * (deg - err) / 360f;
        UDEMath.PolarFunction bulletPositionInverse = (deg, err) => 0.3f + 2.5f * (err - deg) / 360f;

        UDEPolarMovementBuilder first = UDEPolarMovementBuilder.Create().DoNotFaceToMovingDirection();
        UDEPolarMovementBuilder second = UDEPolarMovementBuilder.Create(true).MinAngularSpeed(-30f).MinRadialSpeed(-2.2f).AngularAccel(-25f).RadialAccel(-1.8f).StartTime(1000);

        Vector2 origin = originEnemy.transform.position;

        int cnt = 0;
        while (true)
        {
            float error = Random.Range(-10f, 10f);
            UDEMath.PolarCoord[] coords = cnt % 2 == 0 ? 
                UDEMath.GetPolarCoordStructs(bulletPosition, error, 3 * 360 + error, error, 500) : 
                UDEMath.GetPolarCoordStructs(bulletPositionInverse, -error, -error - 3 * 360, -error, 500);
            for (int i = 0; i < coords.Length; i++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                Vector2 formLoc = origin + (UDEMath.CartesianCoord)coords[i];
                float angle = coords[i].degree - 90f;
                UDEBulletMovement[] movements = new UDEBulletMovement[] { first.Build(), second.Build() };
                bullet.Initialize(formLoc, origin, angle, originEnemy, this, movements);
                bullets.Add(bullet);
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.012f, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1f, UDETime.TimeScale.ENEMY));
            
            for (int i = bullets.Count - 1; i > -1; i--)
            {
                ((dynamic)bullets[i]).ForceMoveToPhase(1);
                bullets.RemoveAt(i);
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(7f, UDETime.TimeScale.ENEMY));
            cnt++;
        }
    }
}