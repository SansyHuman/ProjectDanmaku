using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Item;
using SansyHuman.Management;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util.Math;

using TMPro;

using UnityEngine;

namespace SansyHuman.Enemy
{
    public class EnemyBase : UDEEnemy
    {
        [SerializeField]
        private GameObject death;

        [Serializable]
        public struct DropPowerItem
        {
            public float power;
            public int number;
            public int initSpeed;
        }

        [Serializable]
        public struct DropItem
        {
            public ItemBase item;
            public int number;
            public float initSpeed;
        }

        [SerializeField]
        private DropPowerItem[] dropPowerItems;

        [SerializeField]
        private DropItem[] dropItems;

        [SerializeField]
        private float itemDropRange;

        private int specialSpellCount = 0;
        private UnityEngine.UI.Image enemyHealthBar;
        private TextMeshProUGUI patternCnt;

        [SerializeField]
        protected bool showHealth = false;

        public DropPowerItem[] DropPowerItems
        {
            get => dropPowerItems;
            set => dropPowerItems = value;
        }

        public DropItem[] DropItems
        {
            get => dropItems;
            set => dropItems = value;
        }

        public float ItemDropRange
        {
            get => itemDropRange;
            set => itemDropRange = value > 0 ? value : 0;
        }

        public int SpecialSpellCount => specialSpellCount;

        public bool ShowHealth
        {
            get => showHealth;
        }

        public override void Initialize()
        {
            base.Initialize();

            enemyHealthBar = GameObject.Find("EnemyHealthBar").GetComponent<UnityEngine.UI.Image>();
            patternCnt = GameObject.Find("SpellCount").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < shotPatterns.Count; i++)
            {
                if (shotPatterns[i].shotPattern.Type == UDE.Pattern.UDEBaseShotPattern.PatternType.SPECIAL)
                    specialSpellCount++;
            }
        }

        public virtual void LateUpdate()
        {
            if (showHealth)
                enemyHealthBar.fillAmount = health / shotPatterns[currentPhase].health;
        }

        protected override void OnPatternEnd(UDEBaseShotPattern endedPattern)
        {
            base.OnPatternEnd(endedPattern);

            if (endedPattern.Type == UDEBaseShotPattern.PatternType.SPECIAL)
            {
                specialSpellCount--;
                if (showHealth)
                    patternCnt.text = $"{specialSpellCount}";
            }
        }

        public virtual void ShowHealthAndSpecllCount()
        {
            showHealth = true;
            enemyHealthBar.fillAmount = health / shotPatterns[currentPhase].health;
            patternCnt.text = $"{specialSpellCount}";
        }

        public virtual void OnDestroy()
        {
            base.OnDeath();
            enemyHealthBar.fillAmount = 0;
            patternCnt.text = "0";
        }

        public override void OnDeath()
        {
            base.OnDeath();

            GameManager.player.AddScore(scoreOnDeath);

            GameObject obj = Instantiate(death);
            obj.transform.position = transform.position;

            for (int i = 0; i < dropPowerItems.Length; i++)
            {
                ref DropPowerItem drop = ref dropPowerItems[i];

                for (int j = 0; j < drop.number; j++)
                {
                    float distance = UnityEngine.Random.Range(0, itemDropRange);
                    float angle = UnityEngine.Random.Range(0, 360);

                    (float x, float y) = UDEMath.Polar2Cartesian(distance, angle);
                    Vector2 initPos = (Vector2)characterTr.position + new Vector2(x, y);

                    ItemManager.Instance.SummonPowerItem(drop.power, drop.initSpeed, initPos);
                }
            }

            for (int i = 0; i < dropItems.Length; i++)
            {
                ref DropItem drop = ref dropItems[i];

                for (int j = 0; j < drop.number; j++)
                {
                    float distance = UnityEngine.Random.Range(0, itemDropRange);
                    float angle = UnityEngine.Random.Range(0, 360);

                    (float x, float y) = UDEMath.Polar2Cartesian(distance, angle);
                    Vector2 initPos = (Vector2)characterTr.position + new Vector2(x, y);

                    ItemBase item;
                    if (drop.item.UsesObjectPool)
                        item = ObjectPool.Instance.GetObject(drop.item);
                    else
                        item = Instantiate<ItemBase>(drop.item);

                    item.Initialize(drop.initSpeed, initPos);
                }
            }
        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            if (!canBeDamaged)
                return;

            if (collision.CompareTag("Bullet"))
            {
                UDEAbstractBullet bullet = collision.GetComponent<UDEAbstractBullet>();
                if (bullet != null && bullet.gameObject.activeSelf && bullet.OriginCharacter is UDEPlayer)
                {
                    health -= bullet.Damage;

                    UDEBulletPool.Instance.ReleaseBullet(bullet);
                }
            }
            if (collision.CompareTag("Laser"))
            {
                UDELaser laser = collision.GetComponent<UDELaser>();
                if (laser != null && laser.OriginCharacter is UDEPlayer)
                {
                    health -= laser.Dps * Time.deltaTime * UDETime.Instance.PlayerTimeScale;
                }
            }
        }
    }
}