using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.Pattern
{
    public class SheroSubPattern1 : ShotPatternBase
    {
        // Bullet codes
        public const int GreenLuminusBullet = 0;
        public const int LightBlueLuminusBullet = 1;

        [SerializeField]
        private int numberOfGreenBullets = 25;

        [SerializeField]
        private float greenBulletSpeed = 6f;

        [SerializeField]
        private float blueBulletSpeed = 9f;

        [SerializeField]
        private float blueBulletAngle = 15f;

        protected override IEnumerator ShotPattern()
        {
            originEnemy.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 0)), Quaternion.Euler(0, 0, 0));
            var trResult = UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.5f, 0)), 0.5f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
            originEnemy.CanBeDamaged = false;
            yield return new WaitUntil(() => trResult.EndTransition);

            originEnemy.CanBeDamaged = true;

            IUDERandom random = new UDEXORRandom();
            Vector2 moveVector = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.65f, 0));
            moveVector = new Vector2(0, moveVector.y);

            while (true)
            {
                float dtheta = 360f / numberOfGreenBullets;
                for (int i = 0; i < numberOfGreenBullets; i++)
                {
                    UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[GreenLuminusBullet]);
                    bullet.transform.localScale = new Vector3(0.7f, 0.7f);
                    bullet.SummonTime = 0.08f;
                    (float x, float y) = UDEMath.Polar2Cartesian(greenBulletSpeed, i * dtheta);
                    bullet.MoveBulletToDirection(originEnemy, this, originEnemy.transform.position, 0, new Vector2(x, y));
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.35f, UDETime.TimeScale.ENEMY));

                Vector2 playerPos = GameManager.player.transform.position;
                Vector2 direction = playerPos - (Vector2)originEnemy.transform.position;
                (_, float angle) = UDEMath.Cartesian2Polar(direction);

                Vector2 vel1 = UDEMath.Polar2Cartesian(blueBulletSpeed, angle).ToVector2();
                Vector2 vel2 = UDEMath.Polar2Cartesian(blueBulletSpeed, angle + blueBulletAngle).ToVector2();
                Vector2 vel3 = UDEMath.Polar2Cartesian(blueBulletSpeed, angle - blueBulletAngle).ToVector2();

                for (int i = 0; i < 5; i++)
                {
                    UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[LightBlueLuminusBullet]);
                    bullet.transform.localScale = new Vector3(0.7f, 0.7f);
                    bullet.SummonTime = 0.08f;
                    bullet.MoveBulletToDirection(originEnemy, this, originEnemy.transform.position, 0, vel1);

                    bullet = UDEBulletPool.Instance.GetBullet(patternBullets[LightBlueLuminusBullet]);
                    bullet.transform.localScale = new Vector3(0.7f, 0.7f);
                    bullet.SummonTime = 0.08f;
                    bullet.MoveBulletToDirection(originEnemy, this, originEnemy.transform.position, 0, vel2);

                    bullet = UDEBulletPool.Instance.GetBullet(patternBullets[LightBlueLuminusBullet]);
                    bullet.transform.localScale = new Vector3(0.7f, 0.7f);
                    bullet.SummonTime = 0.08f;
                    bullet.MoveBulletToDirection(originEnemy, this, originEnemy.transform.position, 0, vel3);

                    yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.15f, UDETime.TimeScale.ENEMY));
                }

                int flag = random.NextInt(0, 100);
                Vector2 curPos = Camera.main.WorldToViewportPoint(originEnemy.transform.position);
                if (flag < 50) // Move up
                {
                    if (curPos.y >= 0.79f) // Move down
                    {
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, -moveVector, 1f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
                    }
                    else
                    {
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, moveVector, 1f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
                    }
                }
                else // Move down
                {
                    if (curPos.y <= 0.21f) // Move up
                    {
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, moveVector, 1f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
                    }
                    else
                    {
                        UDETransitionHelper.MoveAmount(originEnemy.gameObject, -moveVector, 1f, UDETransitionHelper.easeOutCubic, UDETime.TimeScale.ENEMY, true);
                    }
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.3f, UDETime.TimeScale.ENEMY));

                for (int i = 0; i < numberOfGreenBullets; i++)
                {
                    UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[GreenLuminusBullet]);
                    bullet.transform.localScale = new Vector3(0.7f, 0.7f);
                    bullet.SummonTime = 0.08f;
                    (float x, float y) = UDEMath.Polar2Cartesian(greenBulletSpeed, i * dtheta);
                    bullet.MoveBulletToDirection(originEnemy, this, originEnemy.transform.position, 0, new Vector2(x, y));
                }

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.7f, UDETime.TimeScale.ENEMY));
            }
        }
    }
}