using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util;

using UnityEngine;

namespace SansyHuman.Enemy
{
    public class EnemyDeath : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = new Vector3(1, 1, 1);
            sprite = GetComponent<SpriteRenderer>();
            Color col = sprite.color;
            col.a = 1;
            sprite.color = col;

            accTime = 0;

            GetComponent<AudioSource>().Play();
        }

        SpriteRenderer sprite;
        float accTime;

        // Update is called once per frame
        void Update()
        {
            float deltaTime = Time.deltaTime * Mathf.Max(UDETime.Instance.EnemyTimeScale, UDETime.Instance.PlayerTimeScale);
            accTime += deltaTime;

            Color col = sprite.color;
            col.a = 1 - UDETransitionHelper.easeOutQuad(accTime * 5);
            sprite.color = col;
            Vector3 scale = transform.localScale;
            scale.x += 2 * deltaTime * 5;
            scale.y += 2 * deltaTime * 5;
            transform.localScale = scale;
            if (transform.localScale.x >= 3)
                Destroy(gameObject);
        }
    }
}