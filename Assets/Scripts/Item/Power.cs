using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Item
{
    public class Power : ItemBase
    {
        [SerializeField]
        private float power = 0.01f;

        public float PowerPoint => power;
    }
}