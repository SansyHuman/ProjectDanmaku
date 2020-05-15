using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Util;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Management;

public class TestStage : UDEBaseStagePattern
{
    enum Enemy
    {
        TEST_ENEMY
    }

    public UDETransitionHelper.EaseType easeType = UDETransitionHelper.EaseType.EaseLinear;

    protected override IEnumerator StagePattern()
    {
        while (true)
        {
            UDEEnemy boss = SummonEnemy(enemies[(int)Enemy.TEST_ENEMY]);
            boss.transform.SetPositionAndRotation(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0)), Quaternion.Euler(0, 0, 0));
            boss.transform.Translate(new Vector3(0, 0, 10));
            boss.Initialize();
            // UDETransitionHelper.MoveOnCurve(boss.gameObject, UDECurve.GetCurveByName("First"), 4, UDETransitionHelper.EaseTypeOf(easeType), UDETime.TimeScale.ENEMY, true);
            /*
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(1f, UDETime.TimeScale.ENEMY));
            UDETransitionHelper.MoveOnCurve(boss.gameObject, UDECurve.GetCurveByName("First"), 5, UDETransitionHelper.EaseTypeOf(easeType), UDETime.TimeScale.ENEMY, true);
            yield return StartCoroutine(UDETime.Instance.WaitForScaledSeconds(7f, UDETime.TimeScale.ENEMY));
            UDETransitionHelper.MoveOnCurve(boss.gameObject, UDECurve.GetCurveByName("First"), 2, UDETransitionHelper.EaseTypeOf(easeType), UDETime.TimeScale.ENEMY, true);
            */
            yield return new WaitUntil(() => !boss.Alive);
        }
    }
}