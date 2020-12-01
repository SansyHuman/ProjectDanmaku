using System.Collections;
using System.Collections.Generic;

using SansyHuman.Management;

using UnityEngine;

namespace SansyHuman.Item
{
    public class Power : ItemBase
    {
        [SerializeField]
        private float power = 0.01f;

        public float PowerPoint
        {
            get => power;
            internal set
            {
                power = value;
            }
        }

        public override bool UsesObjectPool => true;

        public override void RemoveItem()
        {
            ObjectPool.Instance.ReturnObject(this);
        }
    }
}