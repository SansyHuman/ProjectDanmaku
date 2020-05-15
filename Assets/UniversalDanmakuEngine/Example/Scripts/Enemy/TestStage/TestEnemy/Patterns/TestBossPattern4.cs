using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

public class TestBossPattern4 : UDEBaseShotPattern
{
    private List<Transform> moveIn = new List<Transform>(256);

    private Transform enemyTr;

    [SerializeField] private int numberOfBullets = 8;
    [SerializeField] private float radialSpeed = 2f;
    [SerializeField] private float angularSpeed = 45f;

    public override void Initialize(UDEEnemy originEnemy)
    {
        base.Initialize(originEnemy);
        enemyTr = originEnemy.transform;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < moveIn.Count; i++)
        {
            if (!moveIn[i].gameObject.activeSelf)
            {
                moveIn.RemoveAt(i);
                i--;
                continue;
            }

            if ((moveIn[i].position - enemyTr.position).sqrMagnitude < 0.09f)
            {
                UDEBulletPool.Instance.ReleaseBullet(moveIn[i].gameObject.GetComponent<UDEAbstractBullet>());
                moveIn.RemoveAt(i);
                i--;
            }
        }
    }

    protected override IEnumerator ShotPattern()
    {
        originEnemy.CanBeDamaged = false;
        var result = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), 2f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => result.EndTransition);
        originEnemy.CanBeDamaged = true;

        UDEPolarMovementBuilder builder = UDEPolarMovementBuilder.Create().DoNotFaceToMovingDirection();
        float angleDiff = 360f / numberOfBullets;

        bool lowHealth = false;
        float summonInterval = 2f;

        while (true)
        {
            if (!lowHealth && originEnemy.Health < 200f)
            {
                lowHealth = true;
                summonInterval = 1.4f;
                moveIn.RemoveAll(tr => !tr.gameObject.activeSelf);
                UDEBulletPool.Instance.ReleaseBullets(shottedBullets.ToArray());
                moveIn.Clear();
            }

            for (int i = 0; i < numberOfBullets; i++)
            {
                float angle = angleDiff * i;

                UDEAbstractBullet moveOut = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                UDEBulletMovement movementOut = builder.RadialSpeed(radialSpeed).AngularSpeed(angularSpeed).RotationAngularSpeed(angularSpeed).Build();

                Vector2 origin = enemyTr.position;
                Vector2 formLoc = origin + UDEMath.Polar2Cartesian(0.3f, angle).ToVector2();

                moveOut.Initialize(formLoc, origin, angle, originEnemy, this, movementOut, setOriginToCharacter: true);

                UDEAbstractBullet moveIn = UDEBulletPool.Instance.GetBullet(patternBullets[0]);
                UDEBulletMovement movementIn = builder.RadialSpeed(-radialSpeed).AngularSpeed(-angularSpeed).RotationAngularSpeed(-angularSpeed).Build();

                formLoc = origin + UDEMath.Polar2Cartesian(8.3f, angle).ToVector2();

                moveIn.Initialize(formLoc, origin, angle, originEnemy, this, movementIn, setOriginToCharacter: true);
                this.moveIn.Add(moveIn.transform);

                Vector3 moveOutScale = moveOut.transform.localScale;
                Vector3 moveInScale = moveIn.transform.localScale;
                moveOutScale *= 2.5f;
                moveInScale *= 2.5f;
                moveOut.transform.localScale = moveOutScale;
                moveIn.transform.localScale = moveInScale;
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(summonInterval, UDETime.TimeScale.ENEMY));
        }
    }
}
