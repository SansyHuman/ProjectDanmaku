using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class Settings : MonoBehaviour
    {
        [SerializeField]
        private Color selectColor;

        [SerializeField]
        private Color unselectColor;

        [SerializeField]
        private TextMeshProUGUI graphics;
        [SerializeField]
        private GameObject graphicsSetting;

        [SerializeField]
        private TextMeshProUGUI language;
        [SerializeField]
        private GameObject languageSetting;

        [SerializeField]
        private TextMeshProUGUI keyboard;
        [SerializeField]
        private GameObject keyboardSetting;

        [SerializeField]
        private TextMeshProUGUI gamepad;
        [SerializeField]
        private GameObject gamepadSetting;

        [SerializeField]
        private AudioClip menuMove;

        [SerializeField]
        private AudioClip menuSelect;

        [SerializeField]
        private AudioClip menuError;

        private AudioSource audioSource;

        public enum SettingList
        {
            GRAPHICS = 0,
            LANGUAGE = 1,
            KEYBOARD = 2,
            GAMEPAD = 3
        }

        public SettingList currentSetting = SettingList.GRAPHICS;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays menu move sound.
        /// </summary>
        public void PlayMenuMove()
        {
            audioSource.clip = menuMove;
            audioSource.Play();
        }

        /// <summary>
        /// Plays menu select sound.
        /// </summary>
        public void PlayMenuSelect()
        {
            audioSource.clip = menuSelect;
            audioSource.Play();
        }

        /// <summary>
        /// Plays menu error sound.
        /// </summary>
        public void PlayMenuError()
        {
            audioSource.clip = menuError;
            audioSource.Play();
        }

        private void OnEnable()
        {
            if (currentSetting == SettingList.GRAPHICS)
            {
                graphics.color = selectColor;
                graphicsSetting.SetActive(true);
            }
            else
            {
                graphics.color = unselectColor;
                graphicsSetting.SetActive(false);
            }

            if (currentSetting == SettingList.LANGUAGE)
            {
                language.color = selectColor;
                languageSetting.SetActive(true);
            }
            else
            {
                language.color = unselectColor;
                languageSetting.SetActive(false);
            }

            if (currentSetting == SettingList.KEYBOARD)
            {
                keyboard.color = selectColor;
                keyboardSetting.SetActive(true);
            }
            else
            {
                keyboard.color = unselectColor;
                keyboardSetting.SetActive(false);
            }

            if (currentSetting == SettingList.GAMEPAD)
            {
                gamepad.color = selectColor;
                gamepadSetting.SetActive(true);
            }
            else
            {
                gamepad.color = unselectColor;
                gamepadSetting.SetActive(false);
            }
        }

        private void Update()
        {
            if (keyboardSetting.GetComponent<KeyboardSetting>().ReadingKeyInput)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.Escape) || (pad != null && pad.buttonEast.wasPressedThisFrame))
            {
                SceneManager.UnloadSceneAsync("Settings");
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || (pad != null && pad.leftShoulder.wasPressedThisFrame))
            {
                if (currentSetting > SettingList.GRAPHICS)
                {
                    switch (currentSetting)
                    {
                        case SettingList.LANGUAGE:
                            language.color = unselectColor;
                            languageSetting.SetActive(false);
                            break;
                        case SettingList.KEYBOARD:
                            keyboard.color = unselectColor;
                            keyboardSetting.SetActive(false);
                            break;
                        case SettingList.GAMEPAD:
                            gamepad.color = unselectColor;
                            gamepadSetting.SetActive(false);
                            break;
                    }

                    currentSetting--;

                    switch (currentSetting)
                    {
                        case SettingList.GRAPHICS:
                            graphics.color = selectColor;
                            graphicsSetting.SetActive(true);
                            break;
                        case SettingList.LANGUAGE:
                            language.color = selectColor;
                            languageSetting.SetActive(true);
                            break;
                        case SettingList.KEYBOARD:
                            keyboard.color = selectColor;
                            keyboardSetting.SetActive(true);
                            break;
                    }

                    PlayMenuMove();
                }
                else
                {
                    PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || (pad != null && pad.rightShoulder.wasPressedThisFrame))
            {
                if (currentSetting < SettingList.GAMEPAD)
                {
                    switch (currentSetting)
                    {
                        case SettingList.GRAPHICS:
                            graphics.color = unselectColor;
                            graphicsSetting.SetActive(false);
                            break;
                        case SettingList.LANGUAGE:
                            language.color = unselectColor;
                            languageSetting.SetActive(false);
                            break;
                        case SettingList.KEYBOARD:
                            keyboard.color = unselectColor;
                            keyboardSetting.SetActive(false);
                            break;
                    }

                    currentSetting++;

                    switch (currentSetting)
                    {
                        case SettingList.LANGUAGE:
                            language.color = selectColor;
                            languageSetting.SetActive(true);
                            break;
                        case SettingList.KEYBOARD:
                            keyboard.color = selectColor;
                            keyboardSetting.SetActive(true);
                            break;
                        case SettingList.GAMEPAD:
                            gamepad.color = selectColor;
                            gamepadSetting.SetActive(true);
                            break;
                    }

                    PlayMenuMove();
                }
                else
                {
                    PlayMenuError();
                }
            }
        }
    }
}