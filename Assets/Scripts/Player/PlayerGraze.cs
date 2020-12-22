using System.Collections;
using System.Collections.Generic;

using SansyHuman.Bullet;

using UnityEngine;

namespace SansyHuman.Player
{
    [DisallowMultipleComponent]
    public class PlayerGraze : MonoBehaviour
    {
        private PlayerBase player;

        private void Awake()
        {
            player = GetComponentInParent<PlayerBase>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            Grazable grazable = collision.gameObject.GetComponent<Grazable>();
            if (grazable != null)
            {
                if (grazable.Graze())
                    player.AddGraze();
            }
        }
    }
}