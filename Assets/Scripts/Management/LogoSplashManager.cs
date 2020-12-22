using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SansyHuman.Management
{
    [DisallowMultipleComponent]
    public class LogoSplashManager : MonoBehaviour
    {
        [Serializable]
        public struct Logo
        {
            public SpriteRenderer logo;
            public float duration;
        }

        [SerializeField]
        private Logo[] logos;

        [SerializeField]
        [Range(0, 1f)]
        private float transitionRatio;

        [SerializeField]
        private UnityEvent onLogoSplashEnd;

        private float accTime;
        private int currentLogo;
        private SpriteRenderer currentLogoObj;

        private void OnEnable()
        {
            accTime = 0;
            currentLogo = 0;
            currentLogoObj = null;
        }

        private void Update()
        {
            if (currentLogo >= logos.Length)
                return;

            ref Logo current = ref logos[currentLogo];

            if (currentLogoObj == null)
                currentLogoObj = Instantiate<SpriteRenderer>(current.logo, new Vector3(0, 0, 0), Quaternion.identity);

            accTime += Time.deltaTime;

            float transitionTime = current.duration * transitionRatio * 0.5f;
            if (accTime < transitionTime)
            {
                Color color = currentLogoObj.color;
                color.a = accTime / transitionTime;
                currentLogoObj.color = color;
            }
            else if (accTime < current.duration - transitionTime)
            {
                Color color = currentLogoObj.color;
                color.a = 1;
                currentLogoObj.color = color;

                if (Input.anyKeyDown)
                    accTime = current.duration - transitionTime;
            }
            else if (accTime < current.duration)
            {
                Color color = currentLogoObj.color;
                color.a = (current.duration - accTime) / transitionTime;
                currentLogoObj.color = color;
            }
            else
            {
                Destroy(currentLogoObj.gameObject);
                currentLogoObj = null;
                accTime = 0;
                currentLogo++;

                if (currentLogo >= logos.Length)
                    onLogoSplashEnd.Invoke();
            }
        }

        public void LoadGame()
        {
            LoadingSceneManager.LoadScene("MainMenu");
        }
    }
}