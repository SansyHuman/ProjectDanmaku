using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Effect
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NoiseDistortion : MonoBehaviour
    {
        private Material mat;

        private int noiseIntensityMapID;
        private int noiseSizeID;
        private int noiseSpeedID;
        private int noiseIntensityID;

        public Texture2D NoiseIntensityMap
        {
            get => mat.GetTexture(noiseIntensityMapID) as Texture2D;
            set => mat.SetTexture(noiseIntensityMapID, value);
        }

        public float NoiseSize
        {
            get => mat.GetFloat(noiseSizeID);
            set
            {
                if (value < 0)
                    value = 0;

                mat.SetFloat(noiseSizeID, value);
            }
        }

        public float NoiseSpeed
        {
            get => mat.GetFloat(noiseSpeedID);
            set => mat.SetFloat(noiseSpeedID, value);
        }

        public float NoiseIntensity
        {
            get => mat.GetFloat(noiseIntensityID);
            set => mat.SetFloat(noiseIntensityID, value);
        }

        private void Awake()
        {
            mat = GetComponent<SpriteRenderer>().material;

            noiseIntensityMapID = Shader.PropertyToID("_NoiseIntensityMap");
            noiseSizeID = Shader.PropertyToID("_NoiseSize");
            noiseSpeedID = Shader.PropertyToID("_NoiseSpeed");
            noiseIntensityID = Shader.PropertyToID("_NoiseIntensity");
        }
    }
}