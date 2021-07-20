using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SansyHuman.UI.HUD
{
    [DisallowMultipleComponent]
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Image skillImage;

        [SerializeField]
        private UnityEngine.UI.Image mp;

        [SerializeField]
        private UnityEngine.UI.Image cooldown;

        [SerializeField]
        private TextMeshProUGUI cooldownText;

        /// <summary>
        /// Gets and sets the image of the skill
        /// </summary>
        public Sprite SkillImage
        {
            get => skillImage.sprite;
            set => skillImage.sprite = value;
        }

        /// <summary>
        /// Gets and sets the MP cooldown of the skill.
        /// MP cooldown is 1 - (current MP / MP requirements).
        /// </summary>
        public float MPCooldown
        {
            get => mp.fillAmount;
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;

                mp.fillAmount = value;
            }
        }

        /// <summary>
        /// Gets and sets the cooldown of the skill.
        /// Cooldown is remaining cooldown / cooldown of the skill.
        /// </summary>
        public float Cooldown
        {
            get => cooldown.fillAmount;
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;

                cooldown.fillAmount = value;
            }
        }

        public void SetCooldownText(float cooldown)
        {
            int cooldownInt = (int)Mathf.Ceil(cooldown);
            if (cooldownInt <= 0)
                cooldownText.text = "";
            else if (cooldownInt < 10)
                cooldownText.text = $"0{cooldownInt}";
            else if (cooldownInt < 100)
                cooldownText.text = cooldownInt.ToString();
            else
                cooldownText.text = "99";
        }
    }
}