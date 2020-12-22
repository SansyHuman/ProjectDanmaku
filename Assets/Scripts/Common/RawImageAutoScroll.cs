using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.Common
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RawImage))]
    public class RawImageAutoScroll : MonoBehaviour
    {
        private RawImage image;

        [SerializeField]
        private Vector2 scrollSpeed = new Vector2(0, -0.02f);

        private void Awake()
        {
            image = GetComponent<RawImage>();
        }

        private void OnDisable()
        {
            image.uvRect = new Rect(0, 0, 1, 1);
        }

        private void Update()
        {
            var uvRect = image.uvRect;

            uvRect.x += scrollSpeed.x * Time.deltaTime;
            uvRect.y += scrollSpeed.y * Time.deltaTime;

            image.uvRect = uvRect;
        }
    }
}