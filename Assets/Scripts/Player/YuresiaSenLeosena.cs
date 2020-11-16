using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Util.Math;

using UnityEngine;
using UnityEngine.Assertions.Must;

namespace SansyHuman.Player
{
    public class YuresiaSenLeosena : PlayerBase
    {
        [SerializeField]
        [Tooltip("The distance from the character to sub weapons.")]
        private float subWeaponsDistance = 1.0f;

        [SerializeField]
        [Tooltip("The angle between sub weapons in the player's local coordinates.")]
        private float subWeaponAngle = 30.0f;

        [SerializeField]
        [Tooltip("The multiplier to sub weapon angle when the character is in the slow mode.")]
        [Range(0.0f, 1.0f)]
        private float subWeaponAngleSlowMultiplier = 0.5f;

        [SerializeField]
        [Tooltip("The deviation in the shot angle from the sub weapons.")]
        private float subWeaponShotAngle = 15.0f;

        [SerializeField]
        [Tooltip("The multiplier to sub weapon shot angle when character is in the slow mode.")]
        [Range(0.0f, 1.0f)]
        private float subWeaponShotAngleSlowMultiplier = 0.15f;

        [SerializeField]
        [Tooltip("The speed of the arrow bullet of the player.")]
        private float arrowBulletSpeed = 8.0f;

        [SerializeField]
        [Tooltip("The angle between velicity of arrows from the sub weapons.")]
        private float subArrowAngle = 15.0f;

        [SerializeField]
        [Tooltip("The multiplier to the sub arrow angle when the character is in the slow mode.")]
        [Range(0.0f, 1.0f)]
        private float subArrowAngleSlowMultiplier = 0.3f;

        [SerializeField]
        [Tooltip("The multiplier to the speed of the sub homing bullet when the character is in the slow mode.")]
        [Range(0.0f, 1.0f)]
        private float subHomingBulletSpeedSlowMultiplier = 0.7f;

        [SerializeField]
        [Tooltip("The sub weapons of the character. The sub weapon's index is 2, 0, 1, and 3 from the top to bottom")]
        private List<Transform> subWeapons;

        [SerializeField]
        [Tooltip("The main bullet prefab.")]
        private UDEAbstractBullet mainBullet;

        [SerializeField]
        [Tooltip("The sub weapon bullet prefab.")]
        private UDEAbstractBullet subBullet;

        [SerializeField]
        [Tooltip("The sub weapon homing bullet prefab.")]
        private UDEHomingBullet subHomingBullet;

        private int shotCnt = 0;

        public override IEnumerator ShootBullet()
        {
            float accTime = bulletFireInterval;

            while (true)
            {
                if (Input.GetKey(shoot))
                    accTime += Time.deltaTime * UDETime.Instance.PlayerTimeScale;
                if (accTime >= bulletFireInterval && Input.GetKey(shoot))
                {
                    accTime = 0;
                    ShotBullet();
                    if (shotCnt == 0)
                        shotCnt = 1;
                    else
                        shotCnt = 0;
                }

                yield return null;
            }
        }

        private IEnumerator subWeaponTransition = null;

        protected override void Awake()
        {
            base.Awake();

            subWeapons[0].gameObject.SetActive(false);
            subWeapons[1].gameObject.SetActive(false);
            subWeapons[2].gameObject.SetActive(false);
            subWeapons[3].gameObject.SetActive(false);

            float innerAngle = subWeaponAngle * 0.5f;
            float outerAngle = subWeaponAngle * 1.5f;

            var sb1 = UDEMath.Polar2Cartesian(subWeaponsDistance, innerAngle);
            var sb3 = UDEMath.Polar2Cartesian(subWeaponsDistance, outerAngle);

            subWeapons[0].localPosition = new Vector3(-sb1.x, sb1.y, 0);
            subWeapons[1].localPosition = new Vector3(-sb1.x, -sb1.y, 0);
            subWeapons[2].localPosition = new Vector3(-sb3.x, sb3.y, 0);
            subWeapons[3].localPosition = new Vector3(-sb3.x, -sb3.y, 0);

            UDECartesianPolarMovementBuilder builder = UDECartesianPolarMovementBuilder.Create();
            builder.Speed(arrowBulletSpeed * subHomingBulletSpeedSlowMultiplier).Angle(subWeaponShotAngle * 2);
            homingMove1 = builder.Build();
            homingMoveSlow1 = builder.Angle(subWeaponShotAngle * 2 * subWeaponShotAngleSlowMultiplier).Build();
            homingMove2 = builder.Angle(-subWeaponShotAngle * 2).Build();
            homingMoveSlow2 = builder.Angle(-subWeaponShotAngle * 2 * subWeaponShotAngleSlowMultiplier).Build();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(slowMode))
            {
                if (subWeaponTransition != null)
                {
                    StopCoroutine(subWeaponTransition);
                }
                subWeaponTransition = SubWeaponTransit();
                StartCoroutine(subWeaponTransition);
            }

            if (Input.GetKeyUp(slowMode))
            {
                if (subWeaponTransition != null)
                {
                    StopCoroutine(subWeaponTransition);
                }
                subWeaponTransition = SubWeaponTransit();
                StartCoroutine(subWeaponTransition);
            }
        }

        protected override void OnPowerLevelChange()
        {
            if (powerLevel == 0 || powerLevel == 1)
            {
                subWeapons[0].gameObject.SetActive(false);
                subWeapons[1].gameObject.SetActive(false);
                subWeapons[2].gameObject.SetActive(false);
                subWeapons[3].gameObject.SetActive(false);
            }
            else if (powerLevel == 2)
            {
                subWeapons[0].gameObject.SetActive(true);
                subWeapons[1].gameObject.SetActive(true);
                subWeapons[2].gameObject.SetActive(false);
                subWeapons[3].gameObject.SetActive(false);
            }
            else if (powerLevel == 3)
            {
                subWeapons[0].gameObject.SetActive(true);
                subWeapons[1].gameObject.SetActive(true);
                subWeapons[2].gameObject.SetActive(true);
                subWeapons[3].gameObject.SetActive(true);
            }
        }

        private UDEBulletMovement homingMove1;
        private UDEBulletMovement homingMoveSlow1;
        private UDEBulletMovement homingMove2;
        private UDEBulletMovement homingMoveSlow2;

        private void ShotBullet()
        {
            UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(mainBullet);
            bullet.MoveBulletToDirection(this, null, mainShotPoint.position, 0, new Vector2(arrowBulletSpeed, 0));
            if (powerLevel >= 1)
            {
                bullet = UDEBulletPool.Instance.GetBullet(mainBullet);
                bullet.MoveBulletToDirection(this, null, mainShotPoint.position - new Vector3(0, 0.15f), 0, new Vector2(arrowBulletSpeed, 0));
            }
            if (powerLevel >= 2)
            {
                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                var vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[0].position, 0, new Vector2(vel.x, vel.y));

                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f) + subArrowAngle * (isSlowMode ? subArrowAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[0].position, 0, new Vector2(vel.x, vel.y));

                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f) - subArrowAngle * (isSlowMode ? subArrowAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[0].position, 0, new Vector2(vel.x, vel.y));

                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, -subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[1].position, 0, new Vector2(vel.x, vel.y));

                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, -subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f) + subArrowAngle * (isSlowMode ? subArrowAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[1].position, 0, new Vector2(vel.x, vel.y));

                bullet = UDEBulletPool.Instance.GetBullet(subBullet);
                vel = UDEMath.Polar2Cartesian(arrowBulletSpeed, -subWeaponShotAngle * (isSlowMode ? subWeaponShotAngleSlowMultiplier : 1.0f) - subArrowAngle * (isSlowMode ? subArrowAngleSlowMultiplier : 1.0f));
                bullet.MoveBulletToDirection(this, null, subWeapons[1].position, 0, new Vector2(vel.x, vel.y));
            }
            if (powerLevel >= 3 && shotCnt == 0)
            {
                UDEHomingBullet homingBullet = UDEBulletPool.Instance.GetBullet(subHomingBullet) as UDEHomingBullet;
                homingBullet.Initialize(subWeapons[2].position, this, isSlowMode ? homingMoveSlow1 : homingMove1);

                homingBullet = UDEBulletPool.Instance.GetBullet(subHomingBullet) as UDEHomingBullet;
                homingBullet.Initialize(subWeapons[3].position, this, isSlowMode ? homingMoveSlow2 : homingMove2);
            }
        }

        private IEnumerator SubWeaponTransit()
        {
            if (isSlowMode)
            {
                float dstAngleInner = subWeaponAngle * subWeaponAngleSlowMultiplier * 0.5f;
                float dstAngleOuter = subWeaponAngle * subWeaponAngleSlowMultiplier * 1.5f;

                bool sub1Fin = false, sub2Fin = false, sub3Fin = false, sub4Fin = false;

                while (true)
                {
                    Vector2 sub1 = subWeapons[0].localPosition;
                    Vector2 sub2 = subWeapons[1].localPosition;
                    Vector2 sub3 = subWeapons[2].localPosition;
                    Vector2 sub4 = subWeapons[3].localPosition;

                    float sub1Angle = UDEMath.Deg(-sub1.x, sub1.y);
                    float sub2Angle = UDEMath.Deg(-sub2.x, -sub2.y);
                    float sub3Angle = UDEMath.Deg(-sub3.x, sub3.y);
                    float sub4Angle = UDEMath.Deg(-sub4.x, -sub4.y);

                    float timeScale = UDETime.Instance.PlayerTimeScale;
                    float deltaTime = Time.deltaTime;

                    sub1Angle -= subWeaponAngle * 4 * deltaTime * timeScale;
                    sub2Angle -= subWeaponAngle * 4 * deltaTime * timeScale;
                    sub3Angle -= subWeaponAngle * 8 * deltaTime * timeScale;
                    sub4Angle -= subWeaponAngle * 8 * deltaTime * timeScale;

                    if (sub1Angle <= dstAngleInner)
                    {
                        sub1Angle = dstAngleInner;
                        sub1Fin = true;
                    }

                    if (sub2Angle <= dstAngleInner)
                    {
                        sub2Angle = dstAngleInner;
                        sub2Fin = true;
                    }

                    if (sub3Angle <= dstAngleOuter)
                    {
                        sub3Angle = dstAngleOuter;
                        sub3Fin = true;
                    }

                    if (sub4Angle <= dstAngleOuter)
                    {
                        sub4Angle = dstAngleOuter;
                        sub4Fin = true;
                    }

                    var sb1 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub1Angle);
                    var sb2 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub2Angle);
                    var sb3 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub3Angle);
                    var sb4 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub4Angle);

                    subWeapons[0].localPosition = new Vector3(-sb1.x, sb1.y, 0);
                    subWeapons[1].localPosition = new Vector3(-sb2.x, -sb2.y, 0);
                    subWeapons[2].localPosition = new Vector3(-sb3.x, sb3.y, 0);
                    subWeapons[3].localPosition = new Vector3(-sb4.x, -sb4.y, 0);

                    if (sub1Fin && sub2Fin && sub3Fin && sub4Fin)
                        break;

                    yield return null;
                }

                yield return null;
            }
            else
            {
                float dstAngleInner = subWeaponAngle * 0.5f;
                float dstAngleOuter = subWeaponAngle * 1.5f;

                bool sub1Fin = false, sub2Fin = false, sub3Fin = false, sub4Fin = false;

                while (true)
                {
                    Vector2 sub1 = subWeapons[0].localPosition;
                    Vector2 sub2 = subWeapons[1].localPosition;
                    Vector2 sub3 = subWeapons[2].localPosition;
                    Vector2 sub4 = subWeapons[3].localPosition;

                    float sub1Angle = UDEMath.Deg(-sub1.x, sub1.y);
                    float sub2Angle = UDEMath.Deg(-sub2.x, -sub2.y);
                    float sub3Angle = UDEMath.Deg(-sub3.x, sub3.y);
                    float sub4Angle = UDEMath.Deg(-sub4.x, -sub4.y);

                    float timeScale = UDETime.Instance.PlayerTimeScale;
                    float deltaTime = Time.deltaTime;

                    sub1Angle += subWeaponAngle * 4 * deltaTime * timeScale;
                    sub2Angle += subWeaponAngle * 4 * deltaTime * timeScale;
                    sub3Angle += subWeaponAngle * 8 * deltaTime * timeScale;
                    sub4Angle += subWeaponAngle * 8 * deltaTime * timeScale;

                    if (sub1Angle >= dstAngleInner)
                    {
                        sub1Angle = dstAngleInner;
                        sub1Fin = true;
                    }

                    if (sub2Angle >= dstAngleInner)
                    {
                        sub2Angle = dstAngleInner;
                        sub2Fin = true;
                    }

                    if (sub3Angle >= dstAngleOuter)
                    {
                        sub3Angle = dstAngleOuter;
                        sub3Fin = true;
                    }

                    if (sub4Angle >= dstAngleOuter)
                    {
                        sub4Angle = dstAngleOuter;
                        sub4Fin = true;
                    }

                    var sb1 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub1Angle);
                    var sb2 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub2Angle);
                    var sb3 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub3Angle);
                    var sb4 = UDEMath.Polar2Cartesian(subWeaponsDistance, sub4Angle);

                    subWeapons[0].localPosition = new Vector3(-sb1.x, sb1.y, 0);
                    subWeapons[1].localPosition = new Vector3(-sb2.x, -sb2.y, 0);
                    subWeapons[2].localPosition = new Vector3(-sb3.x, sb3.y, 0);
                    subWeapons[3].localPosition = new Vector3(-sb4.x, -sb4.y, 0);

                    if (sub1Fin && sub2Fin && sub3Fin && sub4Fin)
                        break;

                    yield return null;
                }

                yield return null;
            }
        }
    }
}