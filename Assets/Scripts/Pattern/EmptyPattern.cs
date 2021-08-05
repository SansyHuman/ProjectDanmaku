using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Pattern
{
    public class EmptyPattern : ShotPatternBase
    {
        protected override IEnumerator ShotPattern()
        {
            yield return null;
        }
    }
}