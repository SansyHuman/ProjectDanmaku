using System.Collections;
using System.Collections.Generic;

using SansyHuman.Effect;
using SansyHuman.Item;
using SansyHuman.Management;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util;

using TMPro;

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

        [SerializeField]
        protected ColorInvert circleColorInvert;

        [SerializeField]
        protected CircleWaveDistortion distortion;

        [SerializeField]
        protected AudioClip dieSound;

        /// <summary>
        /// Maximum health level.
        /// </summary>
        protected const float MaxHealth = 8.0f;

        /// <summary>
        /// Maximum power level.
        /// </summary>
        protected const float MaxPower = 3.0f;

        protected bool isSlowMode = false;
        private IEnumerator slowTransition = null;

        protected bool controllable = true;

        private AudioSource fireSound;

        private Camera mainCamera;

        private UnityEngine.UI.Image healthBar;
        private TextMeshProUGUI healthText;

        private UnityEngine.UI.Image powerBar;
        private TextMeshProUGUI powerText;

        protected override void Awake()
        {
            base.Awake();

            health = 3;
            healthBar = GameObject.Find("HealthBar").GetComponent<UnityEngine.UI.Image>();
            healthBar.fillAmount = health / MaxHealth;
            healthText = GameObject.Find("HealthAmount").GetComponent<TextMeshProUGUI>();
            healthText.text = $"x{health:0.0}";

            power = 0;
            powerBar = GameObject.Find("PowerBar").GetComponent<UnityEngine.UI.Image>();
            powerBar.fillAmount = power / MaxPower;
            powerText = GameObject.Find("PowerAmount").GetComponent<TextMeshProUGUI>();
            powerText.text = $"{power:0.00}";

            Color col = slowMarker.color;
            col.a = 0;
            slowMarker.color = col;

            slowMarkerTr = slowMarker.gameObject.transform;

            fireSound = GetComponent<AudioSource>();

            mainCamera = Camera.main;

            tmp = new Stack<ItemBase>();
        }

        // Temporary stack for item drag. If directely enumerate items and drag, it will cause
        // InvalidOperationException.
        private Stack<ItemBase> tmp;

        protected virtual void EnableSlowMode()
        {
            isSlowMode = true;
            slowTransition = ShowSlowMarker();
            StartCoroutine(slowTransition);
        }

        protected virtual void DisableSlowMove()
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

        protected override void Update()
        {
            if (controllable)
                base.Update();

            if (Input.GetKeyDown(slowMode))
            {
                EnableSlowMode();
            }

            if (Input.GetKeyUp(slowMode))
            {
                DisableSlowMove();
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

            if (mainCamera.WorldToViewportPoint(characterTr.position).x > 0.75f)
            {
                var undraggedItems = ItemManager.Instance.GetUndraggedItems();
                while (undraggedItems.MoveNext())
                {
                    tmp.Push(undraggedItems.Current);
                }

                while (tmp.Count > 0)
                {
                    tmp.Pop().DragToPlayer(characterTr);
                }
            }
        }

        /// <summary>
        /// Called when the power level changed.
        /// </summary>
        protected abstract void OnPowerLevelChange();

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            if (!invincible)
            {
                if (collision.CompareTag("Enemy"))
                    StartCoroutine(DamageSelf(1));
                else if (collision.CompareTag("Bullet"))
                {
                    UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                    if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEEnemy)
                    {
                        UDEBulletPool.Instance.ReleaseBullet(bullet);
                        StartCoroutine(DamageSelf(1));
                    }
                }
                else if (collision.CompareTag("Laser"))
                {
                    UDELaser laser = collision.GetComponent<UDELaser>();
                    if (laser != null && laser.OriginCharacter is UDEEnemy)
                    {
                        StartCoroutine(DamageSelf(1));
                    }
                }
            }

            if (collision.CompareTag("Item"))
            {
                ItemBase item = collision.GetComponent<ItemBase>();

                if (item is Power power && this.power < 3.0f)
                {
                    this.power += power.PowerPoint;
                    if (this.power > 3.0f)
                        this.power = 3.0f;

                    powerBar.fillAmount = this.power / MaxPower;
                    powerText.text = $"{this.power:0.00}";
                }

                item.RemoveItem();
            }
        }

        // TODO: Reduce player's power and drop some power.
        private IEnumerator DamageSelf(float damage)
        {
            health -= damage;
            healthBar.fillAmount = health / MaxHealth;
            healthText.text = $"x{health:0.0}";
            invincible = true;
            controllable = false;

            SpriteRenderer renderer = self.GetComponent<SpriteRenderer>();
            Color col = renderer.color;
            col.a = 0.5f;
            renderer.color = col;

            // TODO: Convert enemy bullets into items within a circle.
            UDEObjectManager.Instance.DestroyAllBullets();

            Vector3 pos = characterTr.position;

            ColorInvert inv1 = Instantiate<ColorInvert>(circleColorInvert, pos + new Vector3(1.0f, 1.0f, 0), Quaternion.identity);
            ColorInvert inv2 = Instantiate<ColorInvert>(circleColorInvert, pos + new Vector3(-1.0f, 1.0f, 0), Quaternion.identity);
            ColorInvert inv3 = Instantiate<ColorInvert>(circleColorInvert, pos + new Vector3(-1.0f, -1.0f, 0), Quaternion.identity);
            ColorInvert inv4 = Instantiate<ColorInvert>(circleColorInvert, pos + new Vector3(1.0f, -1.0f, 0), Quaternion.identity);

            inv1.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            inv2.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            inv3.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            inv4.transform.localScale = new Vector3(0.1f, 0.1f, 1);

            CircleWaveDistortion dist = Instantiate<CircleWaveDistortion>(distortion, pos, Quaternion.identity);
            dist.Scale = 0.1f;
            dist.DistortionAlgorithm = CircleWaveDistortion.Algorithm.Polynomial;
            dist.Distortion = 3;
            dist.Distortion2 = 2.5f;
            dist.InnerRadiusRatio = 0.75f;

            AudioSource dieFX = gameObject.AddComponent<AudioSource>();
            dieFX.clip = dieSound;
            dieFX.volume = 0.7f;
            dieFX.Play();

            float scaleSpeed = 3f;

            characterTr.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0.5f));
            UDETransitionHelper.MoveTo(
                this.gameObject,
                Camera.main.ViewportToWorldPoint(new Vector3(0.15f, 0.5f)),
                1f,
                UDETransitionHelper.easeLinear,
                UDETime.TimeScale.PLAYER,
                false
                );

            yield return null;

            float accTime = 0f;

            while (true)
            {
                accTime += Time.deltaTime * UDETime.Instance.PlayerTimeScale;

                float scale = scaleSpeed * accTime;

                inv1.transform.localScale = new Vector3(scale, scale, 1);
                inv2.transform.localScale = new Vector3(scale, scale, 1);
                inv3.transform.localScale = new Vector3(scale, scale, 1);
                inv4.transform.localScale = new Vector3(scale, scale, 1);
                dist.Scale = scale;

                yield return null;

                if (accTime >= 1.5f)
                    break;
            }

            controllable = true;

            Destroy(inv1.gameObject);
            Destroy(inv2.gameObject);
            Destroy(inv3.gameObject);
            Destroy(inv4.gameObject);
            Destroy(dist.gameObject);

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(3f, UDETime.TimeScale.PLAYER));

            col.a = 1f;
            renderer.color = col;

            Destroy(dieFX);

            invincible = false;
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