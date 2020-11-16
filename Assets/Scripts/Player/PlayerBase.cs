using System.Collections;
using System.Collections.Generic;

using SansyHuman.Item;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

using UnityEngine;

namespace SansyHuman.Player
{
    public abstract class PlayerBase : UDEPlayer
    {
        [SerializeField]
        [Tooltip("The marker when the character is in the slow mode.")]
        private SpriteRenderer slowMarker;
        private Transform slowMarkerTr;

        [SerializeField]
        [Tooltip("The spawn point of the main player bullet.")]
        protected Transform mainShotPoint;

        [SerializeField]
        [Range(0.0f, 3.0f)]
        [Tooltip("The power of the character.")]
        protected float power = 0.0f;
        protected int powerLevel = 0;

        protected bool isSlowMode = false;
        private IEnumerator slowTransition = null;

        private AudioSource fireSound;
        protected override void Awake()
        {
            base.Awake();

            Color col = slowMarker.color;
            col.a = 0;
            slowMarker.color = col;

            slowMarkerTr = slowMarker.gameObject.transform;

            fireSound = GetComponent<AudioSource>();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(slowMode))
            {
                isSlowMode = true;

                isSlowMode = true;

                slowTransition = ShowSlowMarker();
                StartCoroutine(slowTransition);
            }

            if (Input.GetKeyUp(slowMode))
            {
                isSlowMode = false;

                if (slowTransition != null)
                {
                    StopCoroutine(slowTransition);
                }
                slowTransition = null;
                Color col = slowMarker.color;
                col.a = 0;
                slowMarker.color = col;
            }

            if (Input.GetKeyDown(shoot))
                fireSound.Play();

            if (Input.GetKeyUp(shoot))
                fireSound.Stop();

            int tmpLvl = (int)power;
            if (tmpLvl != powerLevel)
            {
                powerLevel = tmpLvl;
                OnPowerLevelChange();
            }
        }

        /// <summary>
        /// Called when the power level changed.
        /// </summary>
        protected abstract void OnPowerLevelChange();

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            base.OnTriggerStay2D(collision);

            if (collision.CompareTag("Item"))
            {
                ItemBase item = collision.GetComponent<ItemBase>();

                if (item is Power power && this.power < 3.0f)
                {
                    this.power += power.PowerPoint;
                    if (this.power > 3.0f)
                        this.power = 3.0f;
                }

                item.RemoveItem();
            }
        }

        private IEnumerator ShowSlowMarker()
        {
            float accTime = 0;
            float totalTime = 0.3f;

            while (accTime <= totalTime)
            {
                Color color = slowMarker.color;
                color.a = accTime / totalTime;
                slowMarker.color = color;

                Vector3 scale = slowMarkerTr.localScale;
                scale.x = scale.y = 1.5f + (totalTime - accTime) * 2f;
                slowMarkerTr.localScale = scale;

                accTime += Time.deltaTime * UDETime.Instance.PlayerTimeScale;
                yield return null;
            }

            Color col = slowMarker.color;
            col.a = 1;
            slowMarker.color = col;
            slowMarkerTr.localScale = new Vector3(1.5f, 1.5f, 1);
            yield return null;
        }
    }
}