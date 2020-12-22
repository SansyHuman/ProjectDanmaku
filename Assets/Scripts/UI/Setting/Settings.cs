using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    public class Settings : MonoBehaviour
    {
        void Update()
        {
            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.Escape) || (pad != null && pad.buttonEast.wasPressedThisFrame))
            {
                SceneManager.UnloadSceneAsync("Settings");
            }
        }
    }
}