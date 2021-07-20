using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace SansyHuman.Debugging
{
    /// <summary>
    /// Utilities for debugging.
    /// </summary>
    public static class DebugUtil
    {
        private static Func<int, UnityEngine.Object> findObjectFromInstanceID;

        static DebugUtil()
        {
            var methodInfo = typeof(UnityEngine.Object)
                .GetMethod("FindObjectFromInstanceID", BindingFlags.NonPublic | BindingFlags.Static);

            if (methodInfo == null)
                findObjectFromInstanceID = t => null;
            else
                findObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(typeof(Func<int, UnityEngine.Object>), methodInfo);
        }

        /// <summary>
        /// Gets the object of the type T with the instance <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="id">The instance id.</param>
        /// <returns>The object with the instance. If does not exist, the returns null.</returns>
        public static T FindObjectFromInstanceID<T>(int id) where T : UnityEngine.Object
        {
            var obj = findObjectFromInstanceID.Invoke(id);
            if (obj == null)
                return null;

            return obj as T;
        }
    }
}