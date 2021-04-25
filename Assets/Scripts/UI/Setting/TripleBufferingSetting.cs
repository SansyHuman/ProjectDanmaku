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
        private SansyHuman.UI.Setting.Settings settings;

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

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (pad != null && pad.dpad.right.wasPressedThisFrame))
            {
                if (selectedQueuedFrames < 3)
                {
                    selectedQueuedFrames = 3;
                    tripleBuffering.text = "On";

                    settings.PlayMenuMove();
                }
                else
                {
                    settings.PlayMenuError();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (pad != null && pad.buttonSouth.wasPressedThisFrame))
            {
                if (selectedQueuedFrames != QualitySettings.maxQueuedFrames)
                {
                    QualitySettings.maxQueuedFrames = selectedQueuedFrames;

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