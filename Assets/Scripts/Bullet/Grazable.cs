using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.Bullet
{
    [DisallowMultipleComponent]
    public class Grazable : MonoBehaviour
    {
        private bool grazable = true;

        public bool ObjectGrazable => grazable;

        private void OnDisable()
        {
            grazable = true;
        }

        /// <summary>
        /// Graze the object.
        /// </summary>
        /// <returns><see langword="true"/> if the object is grazable. Else, return <see langword="false"/>.</returns>
        public bool Graze()
        {
            if (!grazable)
                return false;

            grazable = false;
            return true;
        }
    }
}