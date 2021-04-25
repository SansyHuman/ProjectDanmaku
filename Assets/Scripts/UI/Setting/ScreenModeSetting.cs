using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    public class ScreenModeSetting : MonoBehaviour
    {
        [SerializeField]
        private SansyHuman.UI.Setting.Settings settings;

        [SerializeField]
        private GraphicsSetting graphicsSetting;

        [SerializeField]
        private TextMeshProUGUI screenMode;

        private void OnEnable()
        {
            currentMode = lastCurrentMode;
            selectedMode = currentMode;
            screenMode.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Graphics/ScreenMode/{modes[selectedMode].ToString()}");
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

        public void OnLocalChange()
        {
            screenMode.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Graphics/ScreenMode/{modes[selectedMode].ToString()}");
        }

        private void Update()
        {
            if (graphicsSetting.currentSetting != GraphicsSetting.Settings.SCREEN_MODE)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
            {
                if (selectedMode > 0)
                {
                    selectedMode--;
                    screenMode.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Graphics/ScreenMode/{modes[selectedMode].ToString()}");

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
            {
                if (selectedMode < modes.Length - 1)
                {
                    selectedMode++;
                    screenMode.text = I2.Loc.LocalizationManager.GetTranslation($"Settings/Graphics/ScreenMode/{modes[selectedMode].ToString()}");

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                if (selectedMode != currentMode)
                {
                    currentMode = selectedMode;
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, modes[currentMode], Screen.currentResolution.refreshRate);

                    settings.PlayMenuSelect();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
        }
    }
}