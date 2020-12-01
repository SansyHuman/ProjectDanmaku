using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

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

            if (Input.GetKeyDown(KeyCode.LeftArrow))
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
            else if (Input.GetKeyDown(KeyCode.RightArrow))
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
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
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