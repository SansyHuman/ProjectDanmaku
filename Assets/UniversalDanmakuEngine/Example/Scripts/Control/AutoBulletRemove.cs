using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

public class AutoBulletRemove : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Bullet")
        {
            UDEAbstractBullet bullet = collision.gameObject.GetComponent<UDEAbstractBullet>();
            if (bullet == null || !bullet.gameObject.activeSelf)
                return;
            UDEBulletPool.Instance.ReleaseBullet(bullet);
        }
    }
}
