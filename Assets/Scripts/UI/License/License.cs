using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.License
{
    [DisallowMultipleComponent]
    public class License : MonoBehaviour
    {
        private void Update()
        {
            Gamepad pad = Gamepad.current;

            if (Input.GetKeyDown(KeyCode.Escape) || (pad != null && pad.buttonEast.wasPressedThisFrame))
            {
                SceneManager.UnloadSceneAsync("License");
            }
        }
    }
}