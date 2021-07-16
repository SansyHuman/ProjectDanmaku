using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace SansyHuman.Player
{
    [DisallowMultipleComponent]
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Name of the skill")]
        protected string skillName;

        [SerializeField]
        [Tooltip("Tooltip of the skill")]
        protected string tooltip;

        [SerializeField]
        [Tooltip("Image of the skill")]
        protected Sprite skillImage;

        [SerializeField]
        [Tooltip("Cooldown time of the skill")]
        protected float cooldown;

        [SerializeField]
        [Tooltip("MP consumption")]
        protected int mp;

        [SerializeField]
        [Tooltip("Multiplier to movement speed while using skill")]
        protected float moveSpeedMultiplier = 1.0f;

        [SerializeField]
        [Tooltip("Whether the player can fire bullets while using skill")]
        protected bool firable = true;

        public virtual string SkillName => skillName;
        public virtual string Tooltip => tooltip;
        public Sprite SkillImage => skillImage;
        public float Cooldown => cooldown;
        public int MP => mp;
        public float MoveSpeedMultiplier => moveSpeedMultiplier;
        public bool Firable => firable;

        /// <summary>
        /// Uses skill.
        /// </summary>
        public abstract IEnumerator UseSkill(PlayerBase player);
    }
}