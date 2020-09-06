using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Util.Builder;

public class TestBossPattern : UDEBaseShotPattern
{
    [SerializeField]
    private int NumberOfBullets = 10;
    [SerializeField]
    private float BulletSpeed = 2f;
    [SerializeField] UDECurveLaser laser;

    protected override IEnumerator ShotPattern()
    {
        /*
        float AngleDeg = (360f / NumberOfBullets);
        float AngleRef = 0f;
        float RefOmega = 0f;
        while (true)
        {
            for (int i = 0; i < NumberOfBullets; i++)
            {
                UDEBaseBullet bullet = UDEBulletPool.Instance.GetBullet(baseBullets[0]);
                UDEBulletMovement movement = UDEPolarMovementBuilder.Create().RadialSpeed(BulletSpeed).InitialAngle(i * AngleDeg + AngleRef).Build();
                Vector2 origin = originEnemy.transform.position;
                var formLocTuple = UDEMath.Polar2Cartesian(0.7f, movement.angle);
                Vector2 formLocation = new Vector2(formLocTuple.x, formLocTuple.y) + origin;
                bullet.Initialize(formLocation, origin, 0, originEnemy, this, true, movement);
            }
            AngleRef += RefOmega;
            RefOmega += 0.2f;
            if (RefOmega >= 360)
                RefOmega -= 360;
            if (AngleRef >= 360)
                AngleRef -= 360;
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.07f, UDETime.TimeScale.ENEMY));
        }
        */

        float angle = 360f / NumberOfBullets;

        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.85f)), 1.5f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => result.EndTransition);
        yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

        for (int i = 0; true; i++)
        {           
            float extraAngle = Random.Range(-3f, 3f);

            UDECartesianPolarMovementBuilder builder = UDECartesianPolarMovementBuilder.Create().Speed(BulletSpeed);
            UDECartesianPolarMovementBuilder builderSlower = UDECartesianPolarMovementBuilder.Create(true).TangentialAccel(-8).MinSpeed(BulletSpeed * 0.6f).StartTime(0.3f);
            for (int j = 0; j < NumberOfBullets; j++)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                UDEBulletMovement movement = builder.Angle(extraAngle + angle * j).Build();
                UDEBulletMovement movementSlow = builderSlower.Angle(extraAngle + angle * j).Build();
                Vector2 origin = originEnemy.transform.position;
                Vector2 formLocation = UDEMath.Polar2Cartesian(0.7f, movement.angle).ToVector2() + origin;
                bullet.Initialize(formLocation, origin, 0, originEnemy, this, new UDEBulletMovement[] { movement, movementSlow });
                if (i % 4 == 0)
                {
                    UDEAbstractBullet extraBullet = UDEBulletPool.Instance.GetBullet(patternBullets[1]);
                    UDEBulletMovement movement2 = builder.Speed(BulletSpeed * 0.35f).Angle(extraAngle + angle * j).Build();
                    builder.Speed(BulletSpeed);
                    extraBullet.Initialize(formLocation, origin, 0, originEnemy, this, movement2);
                }
            }
            if (i % 4 == 0)
            {
                switch ((i % 16) / 4)
                {
                    case 0:
                    case 3:
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, new Vector2(2, 0), 0.95f, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
                        break;
                    case 1:
                    case 2:
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, new Vector2(-2, 0), 0.95f, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
                        break;
                }
            }
            

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.25f, UDETime.TimeScale.ENEMY));
        }
    }
}
