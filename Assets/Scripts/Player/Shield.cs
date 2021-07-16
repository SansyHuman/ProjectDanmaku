using System.Collections;
using System.Collections.Generic;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using UnityEngine;

namespace SansyHuman.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Shield : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(EraseBullets());
        }

        private IEnumerator EraseBullets()
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            collider.enabled = true;
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.PLAYER));
            collider.enabled = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Bullet"))
            {
                UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEEnemy)
                {
                    UDEBulletPool.Instance.ReleaseBullet(bullet);
                    // TODO: Convert enemy bullets into items.
                }
            }
        }
    }
}