using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.Item;

namespace SansyHuman.Player
{
    [DisallowMultipleComponent]
    public class PlayerItemMagnet : MonoBehaviour
    {
        private PlayerBase player;
        private Transform playerTr;

        private void Awake()
        {
            player = GetComponentInParent<PlayerBase>();
            playerTr = player.transform;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Item"))
            {
                ItemBase item = collision.gameObject.GetComponent<ItemBase>();
                item.DragToPlayer(playerTr);
            }
        }
    }
}