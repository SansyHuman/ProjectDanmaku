using System.Collections;
using System.Collections.Generic;

using SansyHuman.Enemy;
using SansyHuman.Pattern;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util;

using UnityEngine;

public class Stage1 : UDEBaseStagePattern
{
    public const int WhiteSpirit = 0;

    [SerializeField]
    private UDEAbstractBullet bullet;

    protected override IEnumerator StagePattern()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                LambdaPatternEnemy enemy = SummonEnemy(enemies[WhiteSpirit]) as LambdaPatternEnemy;
                enemy.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.05f + 0.2f * j + 0.1f * i, 0)), Quaternion.Euler(0, 0, 0));
                enemy.Initialize(WhiteSpiritPattern, 2, UDEBaseShotPattern.RemoveBulletsOnDeath.NONE, bullet);
                yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.1f, UDETime.TimeScale.ENEMY));
            }

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.6f, UDETime.TimeScale.ENEMY));
        }

        yield return null;
    }

    private IEnumerator WhiteSpiritPattern(LambdaShotPattern pattern, List<UDEAbstractBullet> bullets, UDEEnemy enemy)
    {
        var trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-7, 0.6f), 6, UDETransitionHelper.easeOutQuad, UDETime.TimeScale.ENEMY, true);
        yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.5f, UDETime.TimeScale.ENEMY));

        for (int i = 0; i < 5; i++)
        {
            UDEAbstractBullet bullet = UDEBulletPool.Instance.GetBullet(bullets[0]);
            bullet.MoveBulletToDirection(enemy, pattern, enemy.transform.position, 0, new Vector2(-5, 0), true);

            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(0.8f, UDETime.TimeScale.ENEMY));
        }

        yield return new WaitUntil(() => trResult.endTransition);

        trResult = UDETransitionHelper.MoveAmount(enemy.gameObject, new Vector2(-6, 15), 6, UDETransitionHelper.easeInQuad, UDETime.TimeScale.ENEMY, true);
        yield return new WaitUntil(() => trResult.endTransition);

        ((EnemyBase)enemy).OnDestroy();
        Destroy(enemy);

        yield return null;
    }
}
