using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SansyHuman.UI.Setting
{
    public class TripleBufferingSetting : MonoBehaviour
    {
        [SerializeField]
        private GraphicsSetting graphicsSetting;

        [SerializeField]
        private TextMeshProUGUI tripleBuffering;

        private int selectedQueuedFrames;

        private void OnEnable()
        {
            selectedQueuedFrames = QualitySettings.maxQueuedFrames;
            tripleBuffering.text = selectedQueuedFrames >= 3 ? "On" : "Off";
        }

        private void Update()
        {
            if (graphicsSetting.currentSetting != GraphicsSetting.Settings.TRIPLE_BUFFERING)
                return;

            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || (pad != null && pad.dpad.left.wasPressedThisFrame))
            {
                if (selectedQueuedFrames >= 3)
                {
                    selectedQueuedFrames = 2;
                    tripleBuffering.text = "Off";

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
                if (selectedQueuedFrames < 3)
                {
                    selectedQueuedFrames = 3;
                    tripleBuffering.text = "On";

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
                if (selectedQueuedFrames != QualitySettings.maxQueuedFrames)
                {
                    QualitySettings.maxQueuedFrames = selectedQueuedFrames;

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