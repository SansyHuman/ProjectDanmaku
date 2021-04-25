using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Enemy
{
    public class Boss : EnemyBase
    {
        [SerializeField]
        [Tooltip("Name of the boss.")]
        private string bossName = "";
    }
}