using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    public class GraphicsSetting : MonoBehaviour
    {
        [SerializeField]
        private Color selectColor;

        [SerializeField]
        private Color unselectColor;

        [SerializeField]
        private SansyHuman.UI.Setting.Settings settings; 

        [SerializeField]
        private TextMeshProUGUI resolution;

        [SerializeField]
        private TextMeshProUGUI screenMode;

        [SerializeField]
        private TextMeshProUGUI vsync;

        [SerializeField]
        private TextMeshProUGUI tripleBuffering;

        public enum Settings
        {
            RESOLUTION = 0,
            SCREEN_MODE = 1,
            VSYNC = 2,
            TRIPLE_BUFFERING = 3
        }

        public Settings currentSetting = Settings.RESOLUTION;

        private void OnEnable()
        {
            if (currentSetting == Settings.RESOLUTION)
            {
                resolution.color = selectColor;
            }
            else
            {
                resolution.color = unselectColor;
            }

            if (currentSetting == Settings.SCREEN_MODE)
            {
                screenMode.color = selectColor;
            }
            else
            {
                screenMode.color = unselectColor;
            }

            if (currentSetting == Settings.VSYNC)
            {
                vsync.color = selectColor;
            }
            else
            {
                vsync.color = unselectColor;
            }

            if (currentSetting == Settings.TRIPLE_BUFFERING)
            {
                tripleBuffering.color = selectColor;
            }
            else
            {
                tripleBuffering.color = unselectColor;
            }
        }

        private void Update()
        {
            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.DownArrow) || (pad != null && pad.dpad.down.wasPressedThisFrame))
            {
                if (currentSetting < Settings.TRIPLE_BUFFERING)
                {
                    switch (currentSetting)
                    {
                        case Settings.RESOLUTION:
                            resolution.color = unselectColor;
                            break;
                        case Settings.SCREEN_MODE:
                            screenMode.color = unselectColor;
                            break;
                        case Settings.VSYNC:
                            vsync.color = unselectColor;
                            break;
                    }

                    currentSetting++;
                    switch (currentSetting)
                    {
                        case Settings.SCREEN_MODE:
                            screenMode.color = selectColor;
                            break;
                        case Settings.VSYNC:
                            vsync.color = selectColor;
                            break;
                        case Settings.TRIPLE_BUFFERING:
                            tripleBuffering.color = selectColor;
                            break;
                    }

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || (pad != null && pad.dpad.up.wasPressedThisFrame))
            {
                if (currentSetting > Settings.RESOLUTION)
                {
                    switch (currentSetting)
                    {
                        case Settings.TRIPLE_BUFFERING:
                            tripleBuffering.color = unselectColor;
                            break;
                        case Settings.VSYNC:
                            vsync.color = unselectColor;
                            break;
                        case Settings.SCREEN_MODE:
                            screenMode.color = unselectColor;
                            break;
                    }

                    currentSetting--;
                    switch (currentSetting)
                    {
                        case Settings.VSYNC:
                            vsync.color = selectColor;
                            break;
                        case Settings.SCREEN_MODE:
                            screenMode.color = selectColor;
                            break;
                        case Settings.RESOLUTION:
                            resolution.color = selectColor;
                            break;
                    }

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
        }
    }
}