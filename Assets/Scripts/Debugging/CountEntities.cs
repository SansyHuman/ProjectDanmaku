using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SansyHuman.Debugging
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class CountEntities : MonoBehaviour
    {
        public enum Mode
        {
            ENEMY,
            BULLET,
            LASER
        }

        [SerializeField]
        private Mode countMode = Mode.ENEMY;

        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (countMode)
            {
                case Mode.BULLET:
                    text.text = $"Bullet count: {UDEObjectManager.Instance.GetAllBullets().Count}";
                    break;
                case Mode.ENEMY:
                    text.text = $"Enemy count: {UDEObjectManager.Instance.GetAllEnemies().Count}";
                    break;
                case Mode.LASER:
                    text.text = $"Laser count: {UDEObjectManager.Instance.GetAllLasers().Count}";
                    break;
            }
        }
    }
}