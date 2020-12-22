using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Math;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SansyHuman.Management
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private static string nextScene;

        [SerializeField]
        private TextMeshProUGUI loadingText;

        [SerializeField]
        private TextMeshProUGUI loadingPercentText;

        [SerializeField]
        private RectTransform[] loadingCircles;

        [SerializeField]
        private float loadingCircleRevolvePeriod;

        [SerializeField]
        private float loadingCircleStopTime;

        [SerializeField]
        private float loadingCircleRadius;

        public static void LoadScene(string sceneName)
        {
            nextScene = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }

        private void Start()
        {
            StartCoroutine(LoadScene());
            StartCoroutine(RotateLoadingCircles());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator LoadScene()
        {
            var result = SceneManager.LoadSceneAsync(nextScene);
            result.allowSceneActivation = false;
            StartCoroutine(UpdatePercentage(result));

            int cnt = 1;
            var wait = new WaitForSeconds(0.33f);

            while (!result.isDone)
            {
                loadingText.text = "Now Loading" + (cnt == 1 ? "." : (cnt == 2 ? ".." : "..."));
                cnt++;
                if (cnt > 3)
                    cnt = 1;

                yield return wait;
            }
        }

        private IEnumerator UpdatePercentage(AsyncOperation result)
        {
            float accTime = 0;

            while (true)
            {
                if (accTime > 2f)
                {
                    result.allowSceneActivation = true;
                    loadingPercentText.text = $"{100}";
                    yield break;
                }
                loadingPercentText.text = $"{(int)(Mathf.Lerp(0, result.progress, (accTime / 2f)) * 100)}";
                accTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator RotateLoadingCircles()
        {
            float dt = loadingCircleStopTime / loadingCircles.Length;
            var wait = new WaitForSeconds(dt);

            for (int i = 0; i < loadingCircles.Length; i++)
            {
                StartCoroutine(RotateCircle(loadingCircles[i], loadingCircleRevolvePeriod, loadingCircleStopTime, loadingCircleRadius));
                yield return wait;
            }
        }

        private IEnumerator RotateCircle(RectTransform circle, float period, float waitTime, float radius)
        {
            UDEMath.TimeFunction func = UDETransitionHelper.easeInOutCubic.Composite(t => t / period);

            float accTime = 0;

            while (true)
            {
                float angle = -90 + 360 * func(accTime);
                if (accTime > period)
                    angle = 270;
                (float x, float y) = UDEMath.Polar2Cartesian(radius, angle);

                circle.anchoredPosition = new Vector2(x, y);

                accTime += Time.deltaTime;
                if (accTime > period + waitTime)
                    accTime = 0;

                yield return null;
            }
        }
    }
}