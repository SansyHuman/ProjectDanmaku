using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Pattern;
using System;
using SansyHuman.UDE.Object;

namespace SansyHuman.Pattern
{
    public class LambdaShotPattern : ShotPatternBase
    {
        private Func<LambdaShotPattern, List<UDEAbstractBullet>, UDEEnemy, IEnumerator> patternFunc;

        internal void Initialize(UDEEnemy originEnemy, UDEAbstractBullet[] bulletPrefabs, Func<LambdaShotPattern, List<UDEAbstractBullet>, UDEEnemy, IEnumerator> pattern, RemoveBulletsOnDeath removeBulletsOnDeath)
        {
            this.patternFunc = pattern;
            patternBullets = new List<UDEAbstractBullet>();
            patternBullets.AddRange(bulletPrefabs);
            this.removeBullets = removeBulletsOnDeath;

            base.Initialize(originEnemy);
        }

        protected override IEnumerator ShotPattern()
        {
            return patternFunc.Invoke(this, patternBullets, originEnemy);
        }
    }
}