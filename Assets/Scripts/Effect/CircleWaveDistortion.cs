using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Effect
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CircleWaveDistortion : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Material mat;
        private Transform tr;
        private float worldUnscaledSize;

        private int algorithmID;
        private int distortionID;
        private int distortion2ID;
        private int worldCenterID;
        private int radiusID;
        private int innerRadiusRatioID;

        public float Scale
        {
            get => tr.localScale.x;
            set => tr.localScale = new Vector3(value, value, value);
        }

        public enum Algorithm
        {
            Polynomial = 0,
            Exponential = 1,
        }

        public Algorithm DistortionAlgorithm
        {
            get => (Algorithm)mat.GetInt(algorithmID);
            set => mat.SetInt(algorithmID, (int)value);
        }

        public float Distortion
        {
            get => mat.GetFloat(distortionID);
            set => mat.SetFloat(distortionID, value);
        }

        public float Distortion2
        {
            get => mat.GetFloat(distortion2ID);
            set => mat.SetFloat(distortion2ID, value);
        }

        public float InnerRadiusRatio
        {
            get => mat.GetFloat(innerRadiusRatioID);
            set
            {
                if (value < 0.5f)
                    value = 0.5f;
                if (value > 1.0f)
                    value = 1.0f;

                mat.SetFloat(innerRadiusRatioID, value);
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mat = spriteRenderer.material;
            tr = transform;
            worldUnscaledSize = Mathf.Min(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height) * 0.5f / spriteRenderer.sprite.pixelsPerUnit;

            algorithmID = Shader.PropertyToID("_Algorithm");
            distortionID = Shader.PropertyToID("_Distortion");
            distortion2ID = Shader.PropertyToID("_Distortion2");
            worldCenterID = Shader.PropertyToID("_WorldCenter");
            radiusID = Shader.PropertyToID("_Radius");
            innerRadiusRatioID = Shader.PropertyToID("_InnerRadiusRatio");

            tr.localScale = new Vector3(1, 1, 1);
        }

        private void Update()
        {
            mat.SetVector(worldCenterID, new Vector4(tr.position.x, tr.position.y));
            mat.SetFloat(radiusID, worldUnscaledSize * Scale);
        }
    }
}