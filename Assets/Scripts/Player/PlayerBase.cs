using System.Collections;
using System.Collections.Generic;

using SansyHuman.Debugging;
using SansyHuman.Effect;
using SansyHuman.Item;
using SansyHuman.Management;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util;
using SansyHuman.UI.HUD;
using SansyHuman.UI.Pause;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

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


        // Character stats

        // Score of the player
        protected int score = 0;

        [SerializeField]
        [Range(0.0f, 3.0f)]
        [Tooltip("The power of the character.")]
        protected float power = 0.0f;
        protected int powerLevel = 0;

        [SerializeField]
        [Range(0, 400)]
        [Tooltip("The MP of the character.")]
        protected int mana = 0;

        protected Shield activeShield = null;

        [SerializeField]
        [Tooltip("Skill 1")]
        protected Skill skill1;

        [SerializeField]
        [Tooltip("Skill 2")]
        protected Skill skill2;

        [SerializeField]
        [Tooltip("Skill 3")]
        protected Skill skill3;

        [SerializeField]
        [Range(0, 5)]
        [Tooltip("The regeneration speed of MP per second")]
        protected float manaRegenerationPerSecond = 0.0f;

        // One graze means it passed near by a bullet.
        protected int graze = 0;

        [SerializeField]
        [Tooltip("The score of one graze.")]
        protected int scorePerGraze = 1000;


        [SerializeField]
        [Tooltip("Skill 1 key")]
        protected KeyCode skill1Key = KeyCode.A;

        [SerializeField]
        [Tooltip("Skill 2 key")]
        protected KeyCode skill2Key = KeyCode.S;

        [SerializeField]
        [Tooltip("Skill 3 key")]
        protected KeyCode skill3Key = KeyCode.X;

        public struct AdditionalKeyMappingInfo
        {
            public KeyCode skill1Key;
            public KeyCode skill2Key;
            public KeyCode skill3Key;
        }


        [SerializeField]
        protected ColorInvert circleColorInvert;

        [SerializeField]
        protected CircleWaveDistortion distortion;

        [SerializeField]
        protected AudioClip grazeSoundClip;

        [SerializeField]
        protected AudioClip dieSound;

        [SerializeField]
        protected PauseMenu pauseMenu;

        [SerializeField]
        protected DebugConsole debugConsole;

        /// <summary>
        /// Maximum health level.
        /// </summary>
        protected const float MaxHealth = 8.0f;

        /// <summary>
        /// Maximum power level.
        /// </summary>
        protected const float MaxPower = 3.0f;

        /// <summary>
        /// Maximum MP level.
        /// </summary>
        protected const int MaxMana = 400;

        protected bool isSlowMode = false;
        private IEnumerator slowTransition = null;

        protected bool controllable = true;

        protected float skill1RemainingCooldown = 0;
        protected float skill2RemainingCooldown = 0;
        protected float skill3RemainingCooldown = 0;

        private AudioSource fireSound;
        private AudioSource grazeSound;

        private Camera mainCamera;

        private Animator animator;

        private TextMeshProUGUI scoreText;

        private UnityEngine.UI.Image healthBar;
        private TextMeshProUGUI healthText;

        private UnityEngine.UI.Image powerBar;
        private TextMeshProUGUI powerText;

        private UnityEngine.UI.Image manaBar;
        private TextMeshProUGUI manaText;

        private TextMeshProUGUI grazeText;

        private SkillHUD skillHUD;

        /// <summary>
        /// Sets the additional key mapping of the player
        /// </summary>
        /// <param name="info">Additional key mapping informations</param>
        public void SetAdditionalKeyMapping(AdditionalKeyMappingInfo info)
        {
            skill1Key = info.skill1Key;
            skill2Key = info.skill2Key;
            skill3Key = info.skill3Key;
        }

        protected override void Awake()
        {
            base.Awake();

            scoreText = GameObject.Find("ScoreAmount").GetComponent<TextMeshProUGUI>();
            scoreText.text = $"{score:D12}";

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

            mana = 0;
            manaBar = GameObject.Find("ManaBar").GetComponent<UnityEngine.UI.Image>();
            manaBar.fillAmount = (float)mana / MaxMana;
            manaText = GameObject.Find("ManaAmount").GetComponent<TextMeshProUGUI>();
            manaText.text = $"{mana:D3}";

            activeShield = null;

            grazeText = GameObject.Find("GrazeAmount").GetComponent<TextMeshProUGUI>();
            grazeText.text = $"{graze}";

            skillHUD = GameObject.Find("SkillHUD").GetComponent<SkillHUD>();

            skillHUD.ResetImages();

            skill1RemainingCooldown = 0;
            skill2RemainingCooldown = 0;
            skill3RemainingCooldown = 0;

            if (skill1 != null)
            {
                SkillSlot skill1Slot = skillHUD.Skill1;
                skill1Slot.SkillImage = skill1.SkillImage;
                skill1Slot.MPCooldown = 1 - ((float)mana / skill1.MP);
                skill1Slot.Cooldown = 0;
            }

            if (skill2 != null)
            {
                SkillSlot skill2Slot = skillHUD.Skill2;
                skill2Slot.SkillImage = skill2.SkillImage;
                skill2Slot.MPCooldown = 1 - ((float)mana / skill2.MP);
                skill2Slot.Cooldown = 0;
            }

            if (skill3 != null)
            {
                SkillSlot skill3Slot = skillHUD.Skill3;
                skill3Slot.SkillImage = skill3.SkillImage;
                skill3Slot.MPCooldown = 1 - ((float)mana / skill3.MP);
                skill3Slot.Cooldown = 0;
            }

            Color col = slowMarker.color;
            col.a = 0;
            slowMarker.color = col;

            slowMarkerTr = slowMarker.gameObject.transform;

            fireSound = GetComponent<AudioSource>();
            grazeSound = gameObject.AddComponent<AudioSource>();
            grazeSound.clip = grazeSoundClip;
            grazeSound.volume = 0.3f;

            mainCamera = Camera.main;

            animator = GetComponent<Animator>();

            tmp = new Stack<ItemBase>();
        }

        [System.Obsolete("This method is called only internally.")]
        public override void Move(float deltaTime)
        {
            Gamepad pad = Gamepad.current;
            if (pad == null)
            {
                base.Move(deltaTime); // Use default keyboard input.

                int tmp = 0;
                if (Input.GetKey(moveRight))
                    tmp += 1;
                if (Input.GetKey(moveLeft))
                    tmp -= 1;

                animator.SetFloat("Direction", (float)tmp);

                return;
            }

            Keyboard key = Keyboard.current;
            if (key != null && key.anyKey.isPressed)
            {
                base.Move(deltaTime);

                int tmp = 0;
                if (Input.GetKey(moveRight))
                    tmp += 1;
                if (Input.GetKey(moveLeft))
                    tmp -= 1;

                animator.SetFloat("Direction", (float)tmp);

                return;
            }

            // Gamepad control
            // Left stick: move
            // RB: slow move
            float realSpeed = Speed;
            if (pad.rightShoulder.isPressed)
                realSpeed *= slowModeSpeedMultiplier;

            Vector3 velocity = pad.leftStick.ReadValue().normalized * realSpeed;
            characterTr.Translate(velocity * deltaTime * UDETime.Instance.PlayerTimeScale);

            Vector3 pos = Camera.main.WorldToViewportPoint(characterTr.position);
            if (pos.x < 0)
                pos.x = 0;
            if (pos.x > 1)
                pos.x = 1;
            if (pos.y < 0)
                pos.y = 0;
            if (pos.y > 1)
                pos.y = 1;
            characterTr.position = Camera.main.ViewportToWorldPoint(pos);

            if (velocity.x > 0.0001)
                animator.SetFloat("Direction", 1);
            else if (velocity.x < -0.0001)
                animator.SetFloat("Direction", -1);
            else
                animator.SetFloat("Direction", 0);
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

        /// <summary>
        /// Activate the shield.
        /// </summary>
        /// <param name="prefab">Shield prefab</param>
        public void ActivateShield(Shield prefab)
        {
            if (activeShield != null)
                Destroy(activeShield.gameObject);

            Shield shield = Instantiate<Shield>(prefab);
            shield.transform.parent = gameObject.transform;
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localRotation = Quaternion.Euler(0, 0, 0);

            activeShield = shield;
        }

        /// <summary>
        /// Makes the player invincible for the time.
        /// </summary>
        /// <param name="time">Time to make the player invincible</param>
        /// <param name="playHitSound">Whether play hit sound</param>
        public void MakeInvincibleForSeconds(float time, bool playHitSound)
        {
            StartCoroutine(InvincibleForSecondsCoroutine(time, playHitSound));
        }

        private IEnumerator InvincibleForSecondsCoroutine(float time, bool playHitSound)
        {
            SpriteRenderer renderer = self.GetComponent<SpriteRenderer>();
            Color col = renderer.color;
            col.a = 0.5f;
            renderer.color = col;
            invincible = true;

            if (playHitSound)
            {
                AudioSource dieFX = gameObject.AddComponent<AudioSource>();
                dieFX.clip = dieSound;
                dieFX.volume = 0.7f;
                dieFX.Play();
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(time, UDETime.TimeScale.PLAYER));

            col.a = 1f;
            renderer.color = col;
            invincible = false;

            yield return null;
        }

        // Used on MP regeneration.
        private float accScaledTime = 0.0f;

        private bool paused = false;
        private bool fireSoundPlayed = false;
        private bool wasInSlowMode = false;

        protected override void Update()
        {
            float deltaTime = Time.deltaTime * UDETime.Instance.PlayerTimeScale;

            skill1RemainingCooldown -= deltaTime;
            skill2RemainingCooldown -= deltaTime;
            skill3RemainingCooldown -= deltaTime;

            if (skill1 != null)
            {
                SkillSlot skill1Slot = skillHUD.Skill1;
                skill1Slot.MPCooldown = 1 - ((float)mana / skill1.MP);
                if (skill1RemainingCooldown < 0)
                    skill1Slot.Cooldown = 0;
                else
                    skill1Slot.Cooldown = skill1RemainingCooldown / skill1.Cooldown;
                skill1Slot.SetCooldownText(skill1RemainingCooldown);
            }

            if (skill2 != null)
            {
                SkillSlot skill2Slot = skillHUD.Skill2;
                skill2Slot.MPCooldown = 1 - ((float)mana / skill2.MP);
                if (skill2RemainingCooldown < 0)
                    skill2Slot.Cooldown = 0;
                else
                    skill2Slot.Cooldown = skill2RemainingCooldown / skill2.Cooldown;
                skill2Slot.SetCooldownText(skill2RemainingCooldown);
            }

            if (skill3 != null)
            {
                SkillSlot skill3Slot = skillHUD.Skill3;
                skill3Slot.MPCooldown = 1 - ((float)mana / skill3.MP);
                if (skill3RemainingCooldown < 0)
                    skill3Slot.Cooldown = 0;
                else
                    skill3Slot.Cooldown = skill3RemainingCooldown / skill3.Cooldown;
                skill3Slot.SetCooldownText(skill3RemainingCooldown);
            }

            if (pauseMenu.GamePaused || debugConsole.DebugConsoleEnabled)
            {
                if (!paused)
                {
                    paused = true;
                    fireSoundPlayed = fireSound.isPlaying;
                    wasInSlowMode = isSlowMode;

                    fireSound.Stop();
                }
                
                return;
            }

            if (controllable)
                base.Update();

            Gamepad pad = Gamepad.current;

            if (paused)
            {
                paused = false;

                if (fireSoundPlayed && (Input.GetKey(shoot) || (pad != null && pad.rightTrigger.isPressed)))
                {
                    fireSound.Play();
                }

                if (wasInSlowMode && (!Input.GetKey(slowMode) || (pad != null && !pad.rightShoulder.isPressed)))
                {
                    DisableSlowMove();
                }
                
                if (!wasInSlowMode && (Input.GetKey(slowMode) || (pad != null && pad.rightShoulder.isPressed)))
                {
                    EnableSlowMode();
                }
            }

            // Gamepad control
            // RB: slow mode

            if (Input.GetKeyDown(slowMode) || (pad != null && pad.rightShoulder.wasPressedThisFrame))
            {
                EnableSlowMode();
            }

            if (Input.GetKeyUp(slowMode) || (pad != null && pad.rightShoulder.wasReleasedThisFrame))
            {
                DisableSlowMove();
            }

            // Gamepad control
            // RTrigger: shoot

            if (Input.GetKeyDown(shoot) || (pad != null && pad.rightTrigger.wasPressedThisFrame))
                fireSound.Play();

            if (Input.GetKeyUp(shoot) || (pad != null && pad.rightTrigger.wasReleasedThisFrame))
                fireSound.Stop();

            // Gamepad control
            // X: skill1
            // Y: skill2
            // B: skill3

            if (Input.GetKeyDown(skill1Key) || (pad != null && pad.buttonWest.wasPressedThisFrame))
            {
                if (skill1 != null && skill1RemainingCooldown <= 0 && mana >= skill1.MP)
                {
                    StartCoroutine(UseSkill(1));
                    AddMana(-skill1.MP);
                }
            }

            if (Input.GetKeyDown(skill2Key) || (pad != null && pad.buttonNorth.wasPressedThisFrame))
            {
                if (skill2 != null && skill2RemainingCooldown <= 0 && mana >= skill2.MP)
                {
                    StartCoroutine(UseSkill(2));
                    AddMana(-skill2.MP);
                }
            }

            if (Input.GetKeyDown(skill3Key) || (pad != null && pad.buttonEast.wasPressedThisFrame))
            {
                if (skill3 != null && skill3RemainingCooldown <= 0 && mana >= skill3.MP)
                {
                    StartCoroutine(UseSkill(3));
                    AddMana(-skill3.MP);
                }
            }

            // Power level check
            int tmpLvl = (int)power;
            if (tmpLvl != powerLevel)
            {
                powerLevel = tmpLvl;
                OnPowerLevelChange();
            }

            // MP regeneration
            accScaledTime += Time.deltaTime * UDETime.Instance.PlayerTimeScale * manaRegenerationPerSecond;
            if (accScaledTime >= 1.0f)
            {
                AddMana(1);
                accScaledTime = 0.0f;
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

        private IEnumerator UseSkill(int skillNumber)
        {
            float originalSpeed = speed;
            float originalFireInterval = bulletFireInterval;
            
            switch (skillNumber)
            {
                case 1:
                    speed *= skill1.MoveSpeedMultiplier;
                    if (!skill1.Firable)
                        bulletFireInterval = float.MaxValue;
                    skill1RemainingCooldown = skill1.Cooldown;
                    yield return StartCoroutine(skill1.UseSkill(this));
                    break;
                case 2:
                    speed *= skill2.MoveSpeedMultiplier;
                    if (!skill2.Firable)
                        bulletFireInterval = float.MaxValue;
                    skill2RemainingCooldown = skill2.Cooldown;
                    yield return StartCoroutine(skill2.UseSkill(this));
                    break;
                case 3:
                    speed *= skill3.MoveSpeedMultiplier;
                    if (!skill3.Firable)
                        bulletFireInterval = float.MaxValue;
                    skill3RemainingCooldown = skill3.Cooldown;
                    yield return StartCoroutine(skill3.UseSkill(this));
                    break;
            }

            speed = originalSpeed;
            bulletFireInterval = originalFireInterval;
            yield return null;
        }

        /// <summary>
        /// Called when the power level changed.
        /// </summary>
        protected abstract void OnPowerLevelChange();

        // Add score to the player(internal only).
        internal void AddScore(int score)
        {
            this.score += score;
            scoreText.text = $"{this.score:D12}";
        }

        // Reset score to 0(internal only).
        internal void ResetScore()
        {
            score = 0;
            scoreText.text = $"{score:D12}";
        }

        // Add power to the player(internal only).
        internal void AddPower(float power)
        {
            this.power += power;
            if (this.power > MaxPower)
                this.power = MaxPower;

            powerBar.fillAmount = this.power / MaxPower;
            powerText.text = $"{this.power:0.00}";
        }

        // Set power of the player(internal only).
        internal void SetPower(float power)
        {
            this.power = power;
            if (this.power > MaxPower)
                this.power = MaxPower;
            if (this.power < 0)
                this.power = 0;

            powerBar.fillAmount = this.power / MaxPower;
            powerText.text = $"{this.power:0.00}";
        }

        // Add MP to the player(internal only).
        internal void AddMana(int mana)
        {
            this.mana += mana;
            if (this.mana > MaxMana)
                this.mana = MaxMana;

            manaBar.fillAmount = (float)this.mana / MaxMana;
            manaText.text = $"{this.mana:D3}";
        }

        internal void SetMana(int mana)
        {
            this.mana = mana;
            if (this.mana > MaxMana)
                this.mana = MaxMana;
            if (this.mana < 0)
                this.mana = 0;

            manaBar.fillAmount = (float)this.mana / MaxMana;
            manaText.text = $"{this.mana:D3}";
        }

        // Add one graze to player(internal only).
        internal void AddGraze()
        {
            graze++;
            grazeText.text = $"{graze}";
            grazeSound.Stop();
            grazeSound.Play();
            AddScore(scorePerGraze);
        }

        // Reset graze to 0(internal only).
        internal void ResetGraze()
        {
            graze = 0;
            grazeText.text = $"{graze}";
        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            if (!invincible)
            {
                if (collision.CompareTag("Enemy"))
                {
                    if (activeShield != null)
                    {
                        Destroy(activeShield.gameObject);
                        activeShield = null;
                        MakeInvincibleForSeconds(0.75f, true);
                    }
                    else
                        StartCoroutine(DamageSelf(1));
                }
                else if (collision.CompareTag("Bullet"))
                {
                    UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                    if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEEnemy)
                    {
                        UDEBulletPool.Instance.ReleaseBullet(bullet);
                        if (activeShield != null)
                        {
                            Destroy(activeShield.gameObject);
                            activeShield = null;
                            MakeInvincibleForSeconds(0.75f, true);
                        }
                        else
                            StartCoroutine(DamageSelf(1));
                    }
                }
                else if (collision.CompareTag("Laser"))
                {
                    UDELaser laser = collision.GetComponent<UDELaser>();
                    if (laser != null && laser.OriginCharacter is UDEEnemy)
                    {
                        if (activeShield != null)
                        {
                            Destroy(activeShield.gameObject);
                            activeShield = null;
                            MakeInvincibleForSeconds(0.75f, true);
                        }
                        else
                            StartCoroutine(DamageSelf(1));
                    }
                }
            }

            if (collision.CompareTag("Item"))
            {
                ItemBase item = collision.GetComponent<ItemBase>();

                if (item is Power power)
                {
                    AddScore(500);
                    AddPower(power.PowerPoint);
                }

                item.RemoveItem();
            }
        }

        // TODO: Reduce player's power and drop some power.
        private IEnumerator DamageSelf(float damage)
        {
            animator.SetFloat("Direction", 0);

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