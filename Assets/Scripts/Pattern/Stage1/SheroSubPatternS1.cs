using System.Collections;
using System.Collections.Generic;
using SansyHuman.Enemy;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

using UnityEngine;

namespace SansyHuman.Pattern
{
    public class SheroSubPatternS1 : UDEBaseShotPattern
    {
        // Bullet codes
        public const int GreenLuminusBullet = 0;
        public const int LightBlueCircleBullet = 1;

        [SerializeField]
        private float angleDifference = 20f;

        [SerializeField]
        private int numberOfBullets = 8;

        [SerializeField]
        private float radialSpeed = 5.5f;

        [SerializeField]
        private GameObject background;

        [SerializeField]
        private Color changeBackgroundColorTo;

        private Color originalBackgroundColor;

        protected override IEnumerator ShotPattern()
        {
            UDETransitionHelper.StopAllTransitions(originEnemy.gameObject);

            originalBackgroundColor = background.GetComponent<SpriteRenderer>().color;

            originEnemy.CanBeDamaged = false;
            UDETransitionHelper.MoveTo(originEnemy.gameObject, Camera.main.ViewportToWorldPoint(new Vector3(0.77f, 0.5f, 0)), 0.75f, UDETransitionHelper.easeOutCubic, UDE.Management.UDETime.TimeScale.ENEMY, true);
            UDETransitionHelper.ChangeScaleTo(background, new Vector3(1.2f, 1.2f, 1f), 1f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, false);
            UDETransitionHelper.ChangeColorTo(background, changeBackgroundColorTo, 1f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, false);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(2f, UDETime.TimeScale.ENEMY));

            originEnemy.CanBeDamaged = true;

            IEnumerator leafFallPattern = LeafFallSubPattern();
            StartSubpattern(leafFallPattern);

            UDEPolarMovementBuilder builder = UDEPolarMovementBuilder.Create().RadialSpeed(radialSpeed);
            UDEBulletMovement movement = builder.Build();

            float currAngle = 0f;
            float dtheta = 360f / numberOfBullets;

            while (true)
            {
                for (int i = 0; i < numberOfBullets; i++)
                {
                    UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[LightBlueCircleBullet]);
                    bullet.SummonTime = 0.1f;
                    Vector2 origin = originEnemy.transform.position;
                    Vector2 initPos = UDEMath.Polar2Cartesian(0.1f, currAngle + dtheta * i).ToVector2() + origin;

                    bullet.Initialize(initPos, origin, 0, originEnemy, this, movement);
                }

                currAngle += angleDifference;
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1.5f, UDETime.TimeScale.ENEMY));
            }
        }

        private IEnumerator LeafFallSubPattern()
        {
            IUDERandom rand = new UDEXORRandom();

            UDECartesianMovementBuilder builder = UDECartesianMovementBuilder.Create();
            builder.Velocity(Vector2.zero)
                .Accel(t => new UDEMath.CartesianCoord(
                6f * Mathf.Log10(t + 1f) * Mathf.Cos(2 * Mathf.PI * t / 2.0f),
                -2.0f
                ))
                .MinVelocity(new Vector2(float.MinValue, -6.0f))
                .DoNotFaceToMovingDirection();

            UDEBulletMovement leafFall = builder.Build();

            while (true)
            {
                UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(patternBullets[GreenLuminusBullet]);
                bullet.transform.localScale = new Vector3(0.82f, 0.82f);
                bullet.SummonTime = 0f;

                Vector2 initPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(rand.NextFloat(0.01f, 0.99f), 1.05f, 0));
                bullet.Initialize(initPos, initPos, 0, originEnemy, this, leafFall);

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.25f, UDETime.TimeScale.ENEMY));
            }
        }

        public override void EndPattern()
        {
            base.EndPattern();

            UDETransitionHelper.ChangeScaleTo(background, new Vector3(1f, 1f, 1f), 0.5f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, false);
            UDETransitionHelper.ChangeColorTo(background, originalBackgroundColor, 0.5f, UDETransitionHelper.easeLinear, UDETime.TimeScale.ENEMY, false);
        }
    }
}