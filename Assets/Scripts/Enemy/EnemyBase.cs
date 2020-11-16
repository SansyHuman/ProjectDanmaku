using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Item;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;

using UnityEngine;

namespace SansyHuman.Enemy
{
    public class EnemyBase : UDEEnemy
    {
        [SerializeField]
        private GameObject death;

        [Serializable]
        public struct DropItem
        {
            public ItemBase item;
            public int number;
            public float initSpeed;
        }

        [SerializeField]
        private DropItem[] dropItems;

        [SerializeField]
        private float itemDropRange;

        public virtual void OnDestroy()
        {
            base.OnDeath();
        }

        public override void OnDeath()
        {
            base.OnDeath();

            GameObject obj = Instantiate(death);
            obj.transform.position = transform.position;

            for (int i = 0; i < dropItems.Length; i++)
            {
                ref DropItem drop = ref dropItems[i];

                for (int j = 0; j < drop.number; j++)
                {
                    float distance = UnityEngine.Random.Range(0, itemDropRange);
                    float angle = UnityEngine.Random.Range(0, 360);

                    (float x, float y) = UDEMath.Polar2Cartesian(distance, angle);
                    Vector2 initPos = (Vector2)characterTr.position + new Vector2(x, y);

                    ItemBase item = Instantiate<ItemBase>(drop.item);
                    item.Initialize(drop.initSpeed, initPos);
                }
            }
        }
    }
}