using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace SansyHuman.UI.Setting
{
    public class ScreenModeSetting : MonoBehaviour
    {
        [SerializeField]
        private GraphicsSetting graphicsSetting;

        [SerializeField]
        private TextMeshProUGUI screenMode;

        private void OnEnable()
        {
            currentMode = lastCurrentMode;
            selectedMode = currentMode;
            screenMode.text = modes[currentMode].ToString();
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, modes[currentMode], Screen.currentResolution.refreshRate);
        }

        private void OnDisable()
        {
            lastCurrentMode = currentMode;
        }

        private FullScreenMode[] modes = { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
        private int currentMode = 1; // Default: FullScreenWindow
        private int selectedMode;
        private static int lastCurrentMode;

        static ScreenModeSetting()
        {
            lastCurrentMode = 1;
        }

        public FullScreenMode CurrentMode => modes[currentMode];

        private void Update()
        {
            if (graphicsSetting.currentSetting != GraphicsSetting.Settings.SCREEN_MODE)
                return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedMode > 0)
                {
                    selectedMode--;
                    screenMode.text = modes[selectedMode].ToString();

                    graphicsSetting.audioSource.clip = graphicsSetting.menuMove;
                    graphicsSetting.audioSource.Play();
                }
                else
                {
                    graphicsSetting.audioSource.clip = graphicsSetting.menuError;
                    graphicsSetting.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedMode < modes.Length - 1)
                {
                    selectedMode++;
                    screenMode.text = modes[selectedMode].ToString();

                    graphicsSetting.audioSource.clip = graphicsSetting.menuMove;
                    graphicsSetting.audioSource.Play();
                }
                else
                {
                    graphicsSetting.audioSource.clip = graphicsSetting.menuError;
                    graphicsSetting.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (selectedMode != currentMode)
                {
                    currentMode = selectedMode;
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, modes[currentMode], Screen.currentResolution.refreshRate);

                    graphicsSetting.audioSource.clip = graphicsSetting.menuSelect;
                    graphicsSetting.audioSource.Play();
                }
                else
                {
                    graphicsSetting.audioSource.clip = graphicsSetting.menuError;
                    graphicsSetting.audioSource.Play();
                }
            }
        }
    }
}