using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Player.Skills
{
    public class Shield : SansyHuman.Player.Skill
    {
        [SerializeField]
        private SansyHuman.Player.Shield shield;

        public override string SkillName => I2.Loc.ScriptLocalization.Skill.Shield;
        public override string Tooltip => I2.Loc.ScriptLocalization.Skill.ShieldDescription;

        public override IEnumerator UseSkill(PlayerBase player)
        {
            player.ActivateShield(shield);
            player.MakeInvincibleForSeconds(1.5f, false);
            yield return null;
        }
    }
}