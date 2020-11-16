using System;
using System.Collections;
using System.Collections.Generic;

using SansyHuman.Pattern;
using SansyHuman.UDE.Object;

using UnityEngine;

using static SansyHuman.UDE.Pattern.UDEBaseShotPattern;

namespace SansyHuman.Enemy
{
    public class LambdaPatternEnemy : EnemyBase
    {
        public void Initialize(Func<LambdaShotPattern, List<UDEAbstractBullet>, UDEEnemy, IEnumerator> pattern, int health, RemoveBulletsOnDeath removeBulletsOnDeath, params UDEAbstractBullet[] bulletPrefabs)
        {
            GameObject patternObj = new GameObject("lambdaPattern", typeof(LambdaShotPattern));
            patternObj.transform.parent = this.transform;

            LambdaShotPattern patternScrpt = patternObj.GetComponent<LambdaShotPattern>();
            patternScrpt.Initialize(this, bulletPrefabs, pattern, removeBulletsOnDeath);

            this.shotPatterns = new List<ShotPattern>();
            shotPatterns.Add(new ShotPattern() { health = health, shotPattern = patternScrpt });

            base.Initialize();
        }
    }
}