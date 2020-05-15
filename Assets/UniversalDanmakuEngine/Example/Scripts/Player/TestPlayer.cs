using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util.Builder;

public class TestPlayer : UDEPlayer, IUDELaserFirable
{
    public UDEBaseBullet bullet;
    public UDEStraightLaser laser;

    private List<UDEStraightLaser> firedLasers;

    private bool laserFiring = false;

    protected override void Awake()
    {
        base.Awake();
        firedLasers = new List<UDEStraightLaser>(3);
    }

    public void FireLaser()
    {
        UDEStraightLaser laser = Instantiate<UDEStraightLaser>(this.laser);
        laser.Initialize(characterTr.position, this, true, Vector2.zero, UDETime.TimeScale.PLAYER, new Vector2(0, 100), 15);
        firedLasers.Add(laser);
        laserFiring = true;
    }

    public void RemoveLaser()
    {
        for (int i = 0; i < firedLasers.Count; i++)
            firedLasers[i].DestroyLaser();
        firedLasers.Clear();
        laserFiring = false;
    }

    public override IEnumerator ShootBullet()
    {
        UDEBulletMovement move1 = UDECartesianPolarMovementBuilder.Create().Speed(10).Angle(90).TangentialAccel(-10).Build();
        UDEBulletMovement move2_1 = UDECartesianPolarMovementBuilder.Create().Speed(10).Angle(45).TangentialAccel(-10).NormalAccel(-30).Build();
        UDEBulletMovement move3_1 = UDECartesianPolarMovementBuilder.Create().Speed(10).Angle(135).TangentialAccel(-10).NormalAccel(30).Build();
        UDEBulletMovement move2_2 = UDECartesianPolarMovementBuilder.Create().Speed(10).Angle(67).TangentialAccel(-10).NormalAccel(-50).Build();
        UDEBulletMovement move3_2 = UDECartesianPolarMovementBuilder.Create().Speed(10).Angle(113).TangentialAccel(-10).NormalAccel(50).Build();

        UDEBulletPool bulletPool = UDEBulletPool.Instance;
        Transform tr = gameObject.transform;

        while (true)
        {
            if (Input.GetKey(shoot))
            {
                
                UDEHomingBullet bullet1 = (UDEHomingBullet)bulletPool.GetBullet(bullet);

                bullet1.Initialize(tr.position, this, move1);

                UDEHomingBullet bullet2 = (UDEHomingBullet)bulletPool.GetBullet(bullet);
                UDEHomingBullet bullet3 = (UDEHomingBullet)bulletPool.GetBullet(bullet);
                if (Input.GetKey(slowMode))
                {

                    bullet2.Initialize(tr.position, this, move2_2);
                    bullet3.Initialize(tr.position, this, move3_2);
                }
                else
                {

                    bullet2.Initialize(tr.position, this, move2_1);
                    bullet3.Initialize(tr.position, this, move3_1);
                }
                

                if (!laserFiring)
                    FireLaser();
            }
            else
            {
                if (laserFiring)
                    RemoveLaser();
            }
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(BulletFireInterval, UDETime.TimeScale.PLAYER));
        }
    }
}
