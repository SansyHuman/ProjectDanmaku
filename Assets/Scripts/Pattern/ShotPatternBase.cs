using System.Collections;
using System.Collections.Generic;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Pattern;
using SansyHuman.UI.HUD;
using UnityEngine;

namespace SansyHuman.Pattern
{
    public abstract class ShotPatternBase : UDEBaseShotPattern
    {
        [SerializeField]
        [Tooltip("Whethere to display remaining time of the pattern on HUD")]
        protected bool showTimeLimit = false;

        private TimelimitHUD timelimitHUD;

        public override void Initialize(UDEEnemy originEnemy)
        {
            if (initialized)
                return;

            base.Initialize(originEnemy);

            timelimitHUD = GameObject.Find("TimelimitHUD").GetComponent<TimelimitHUD>();
        }

        public override void StartPattern()
        {
            if (!initialized)
            {
                Debug.LogError("You did not initalized the shot pattern but you tried to start the pattern.");
                return;
            }
            if (shotPatternOn)
            {
                Debug.LogError("The shot pattern is already running on. StartPattern is ignored.");
                return;
            }

            base.StartPattern();

            if (showTimeLimit)
                timelimitHUD.ShowTimeLimit(this);
        }

        public override void ResetPattern()
        {
            if (shotPatternOn)
            {
                Debug.LogError("The shot pattern is running on. Cannot reset the pattern.");
                return;
            }

            base.ResetPattern();

            if (showTimeLimit)
                timelimitHUD.StopShowingTimeLimit();
        }

        public override void EndPattern()
        {
            base.EndPattern();

            if (showTimeLimit)
                timelimitHUD.StopShowingTimeLimit();
        }
    }
}