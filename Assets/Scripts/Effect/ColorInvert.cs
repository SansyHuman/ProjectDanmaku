using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Effect
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ColorInvert : MonoBehaviour
    {
        private Material mat;
        private int colorID;

        public Color OverallColor
        {
            get => mat.GetColor(colorID);
            set => mat.SetColor(colorID, value);
        }

        private void Awake()
        {
            mat = GetComponent<SpriteRenderer>().material;

            colorID = Shader.PropertyToID("_Color");
        }
    }
}