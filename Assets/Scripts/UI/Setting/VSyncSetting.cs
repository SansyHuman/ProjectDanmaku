using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    public class VSyncSetting : MonoBehaviour
    {
        [SerializeField]
        private GraphicsSetting graphicsSetting;

        [SerializeField]
        private TextMeshProUGUI vsync;

        private int selectedVsyncCnt;

        private void OnEnable()
        {
            selectedVsyncCnt = QualitySettings.vSyncCount;
            if (selectedVsyncCnt == 0)
            {
                vsync.text = "Off";
            }
            else
            {
                vsync.text = $"On({selectedVsyncCnt})";
            }
        }

        private void Update()
        {
            if (graphicsSetting.currentSetting != GraphicsSetting.Settings.VSYNC)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
            {
                if (selectedVsyncCnt > 0)
                {
                    selectedVsyncCnt--;
                    if (selectedVsyncCnt == 0)
                    {
                        vsync.text = "Off";
                    }
                    else
                    {
                        vsync.text = $"On({selectedVsyncCnt})";
                    }

                    graphicsSetting.audioSource.clip = graphicsSetting.menuMove;
                    graphicsSetting.audioSource.Play();
                }
                else
                {
                    graphicsSetting.audioSource.clip = graphicsSetting.menuError;
                    graphicsSetting.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
            {
                if (selectedVsyncCnt < 4)
                {
                    selectedVsyncCnt++;
                    if (selectedVsyncCnt == 0)
                    {
                        vsync.text = "Off";
                    }
                    else
                    {
                        vsync.text = $"On({selectedVsyncCnt})";
                    }

                    graphicsSetting.audioSource.clip = graphicsSetting.menuMove;
                    graphicsSetting.audioSource.Play();
                }
                else
                {
                    graphicsSetting.audioSource.clip = graphicsSetting.menuError;
                    graphicsSetting.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                if (selectedVsyncCnt != QualitySettings.vSyncCount)
                {
                    QualitySettings.vSyncCount = selectedVsyncCnt;

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