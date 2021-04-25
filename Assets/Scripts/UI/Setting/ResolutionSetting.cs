using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    public class ResolutionSetting : MonoBehaviour
    {
        [SerializeField]
        private SansyHuman.UI.Setting.Settings settings;

        [SerializeField]
        private GraphicsSetting graphicsSetting;

        [SerializeField]
        private ScreenModeSetting screenModeSetting;

        [SerializeField]
        private TextMeshProUGUI resolution;

        private Resolution[] resolutions;
        private int currentResInd;
        private int selectedResInd;

        private bool ResolutionEquals(in Resolution r1, in Resolution r2)
        {
            return r1.width == r2.width && r1.height == r2.height && Math.Abs(r1.refreshRate - r2.refreshRate) <= 1;
        }

        private void OnEnable()
        {
            resolutions = Screen.resolutions;
            currentResInd = Array.FindIndex(resolutions, res => ResolutionEquals(res, Screen.currentResolution));
            selectedResInd = currentResInd;

            resolution.text = resolutions[currentResInd].ToString();
        }

        private void Update()
        {
            if (graphicsSetting.currentSetting != GraphicsSetting.Settings.RESOLUTION)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
            {
                if (selectedResInd > 0)
                {
                    selectedResInd--;
                    resolution.text = resolutions[selectedResInd].ToString();

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
            {
                if (selectedResInd < resolutions.Length - 1)
                {
                    selectedResInd++;
                    resolution.text = resolutions[selectedResInd].ToString();

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                if (selectedResInd != currentResInd)
                {
                    ref Resolution tmp = ref resolutions[selectedResInd];
                    Screen.SetResolution(tmp.width, tmp.height, screenModeSetting.CurrentMode, tmp.refreshRate);
                    currentResInd = selectedResInd;

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