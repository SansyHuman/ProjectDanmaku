using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Effect
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CircleLensDistortion : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Material mat;
        private Transform tr;
        private float worldUnscaledSize;

        private int distortionID;
        private int worldCenterID;
        private int radiusID;

        public float Distortion
        {
            get => mat.GetFloat(distortionID);
            set => mat.SetFloat(distortionID, value);
        }

        public float Scale
        {
            get => tr.localScale.x;
            set => tr.localScale = new Vector3(value, value, value);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mat = spriteRenderer.material;
            tr = transform;
            worldUnscaledSize = Mathf.Min(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height) * 0.5f / spriteRenderer.sprite.pixelsPerUnit;

            distortionID = Shader.PropertyToID("_Distortion");
            worldCenterID = Shader.PropertyToID("_WorldCenter");
            radiusID = Shader.PropertyToID("_Radius");

            tr.localScale = new Vector3(1, 1, 1);
        }

        private void Update()
        {
            mat.SetVector(worldCenterID, new Vector4(tr.position.x, tr.position.y));
            mat.SetFloat(radiusID, worldUnscaledSize * Scale);
        }
    }
}