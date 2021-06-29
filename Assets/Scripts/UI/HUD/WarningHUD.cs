using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SansyHuman.UDE.Management;

namespace SansyHuman.UI.HUD
{
    [DisallowMultipleComponent]
    public class WarningHUD : MonoBehaviour
    {
        [SerializeField]
        private AudioClip warningSound;

        [SerializeField]
        private UnityEngine.UI.Image warningSign;

        [SerializeField]
        private TextMeshProUGUI warningText;

        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            warningSign.gameObject.SetActive(false);
            warningText.gameObject.SetActive(false);
        }

        public void Warn()
        {
            StartCoroutine(Warning());
        }

        private IEnumerator Warning()
        {
            warningSign.gameObject.SetActive(true);
            warningText.gameObject.SetActive(true);

            source.clip = warningSound;
            source.Play();
            Color txtColor = warningText.color;
            txtColor.a = 1;
            warningText.color = txtColor;

            Color signColor = new Color();

            for (int i = 0; i < 3; i++)
            {
                signColor = warningSign.color;
                signColor.a = 1;
                warningSign.color = signColor;
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.1f, UDETime.TimeScale.ENEMY));

                signColor = warningSign.color;
                signColor.a = 0;
                warningSign.color = signColor;
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.1f, UDETime.TimeScale.ENEMY));
            }

            signColor = warningSign.color;
            signColor.a = 1;
            warningSign.color = signColor;
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

            for (float a = 1; a > 0; a -= 0.1f)
            {
                txtColor = warningText.color;
                signColor = warningSign.color;
                txtColor.a = a;
                signColor.a = a;
                warningText.color = txtColor;
                warningSign.color = signColor;

                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.05f, UDETime.TimeScale.ENEMY));
            }

            warningSign.gameObject.SetActive(false);
            warningText.gameObject.SetActive(false);
        }
    }
}