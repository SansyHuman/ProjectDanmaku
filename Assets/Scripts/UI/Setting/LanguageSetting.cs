using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    public class LanguageSetting : MonoBehaviour
    {
        [SerializeField]
        private Color selectColor;

        [SerializeField]
        private Color unselectColor;

        [SerializeField]
        private SansyHuman.UI.Setting.Settings settings;

        [SerializeField]
        private TextMeshProUGUI language;

        private List<string> languages;
        private int currentLanguage;
        private int currentLanguageSelected;

        private void OnEnable()
        {
            languages = I2.Loc.LocalizationManager.GetAllLanguages();
            string cur = I2.Loc.LocalizationManager.CurrentLanguage;

            currentLanguage = languages.FindIndex(lang => lang == cur);
            currentLanguageSelected = currentLanguage;

            language.color = selectColor;
            language.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Language/{cur}");
        }

        private void Update()
        {
            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
            {
                if (currentLanguageSelected > 0)
                {
                    currentLanguageSelected--;
                    language.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Language/{languages[currentLanguageSelected]}");

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
            {
                if (currentLanguageSelected < languages.Count - 1)
                {
                    currentLanguageSelected++;
                    language.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Language/{languages[currentLanguageSelected]}");

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                if (currentLanguageSelected != currentLanguage)
                {
                    I2.Loc.LocalizationManager.CurrentLanguage = languages[currentLanguageSelected];
                    currentLanguage = currentLanguageSelected;

                    settings.PlayMenuSelect();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
        }

        // Callback function
        public void OnLanguageChange()
        {
            language.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Language/{languages[currentLanguageSelected]}");
        }
    }
}