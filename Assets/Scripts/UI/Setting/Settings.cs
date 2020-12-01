using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SansyHuman.UI.Setting
{
    [DisallowMultipleComponent]
    public class Settings : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.UnloadSceneAsync("Settings");
            }
        }
    }
}