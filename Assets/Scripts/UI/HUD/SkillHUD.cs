using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.UI.HUD
{
    [DisallowMultipleComponent]
    public class SkillHUD : MonoBehaviour
    {
        [SerializeField]
        private SkillSlot skill1;

        [SerializeField]
        private SkillSlot skill2;

        [SerializeField]
        private SkillSlot skill3;

        [SerializeField]
        [Tooltip("Default skill image")]
        private Sprite defaultSkillImage;

        public SkillSlot Skill1 => skill1;
        public SkillSlot Skill2 => skill2;
        public SkillSlot Skill3 => skill3;

        /// <summary>
        /// Resets all skill images to default.
        /// </summary>
        public void ResetImages()
        {
            skill1.SkillImage = defaultSkillImage;
            skill2.SkillImage = defaultSkillImage;
            skill3.SkillImage = defaultSkillImage;

            skill1.MPCooldown = 0;
            skill1.Cooldown = 0;
            skill1.SetCooldownText(0);
            skill2.MPCooldown = 0;
            skill2.Cooldown = 0;
            skill2.SetCooldownText(0);
            skill3.MPCooldown = 0;
            skill3.Cooldown = 0;
            skill3.SetCooldownText(0);
        }
    }
}