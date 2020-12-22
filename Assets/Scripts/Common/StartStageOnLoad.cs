using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Common
{
    [DisallowMultipleComponent]
    public class StartStageOnLoad : MonoBehaviour
    {
        [SerializeField]
        private int stageNumber;

        private void Start()
        {
            GameManager.Instance.StartStage(stageNumber);
        }
    }
}