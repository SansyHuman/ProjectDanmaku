using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Debugging
{
    [DisallowMultipleComponent]
    public class ToggleDebugUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUI;

        // Start is called before the first frame update
        void Start()
        {
            debugUI.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                debugUI.SetActive(!debugUI.gameObject.activeSelf);
            }
        }
    }
}