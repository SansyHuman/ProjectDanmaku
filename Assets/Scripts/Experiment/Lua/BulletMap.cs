using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;
using System.Linq;

namespace SansyHuman.Experiment.Lua
{
    /// <summary>
    /// Singleton object that contains map from name to bullet prefab
    /// </summary>
    [DisallowMultipleComponent]
    public class BulletMap : UDESingleton<BulletMap>
    {
        private Dictionary<string, UDEAbstractBullet> bulletMap;

        protected override void Awake()
        {
            base.Awake();

            bulletMap = new Dictionary<string, UDEAbstractBullet>();

            UDEAbstractBullet[] bullets = Resources.LoadAll<UDEAbstractBullet>("Bullet");
            for (int i = 0; i < bullets.Length; i++)
            {
                bulletMap.Add(bullets[i].name, bullets[i]);
            }
        }

        /// <summary>
        /// Gets all bullet names.
        /// </summary>
        public string[] BulletNames => bulletMap.Keys.ToArray<string>();

        /// <summary>
        /// Gets a bullet prefab with the name.
        /// </summary>
        /// <param name="name">Name of the bullet</param>
        /// <returns>Bullet prefab</returns>
        public UDEAbstractBullet this[string name]
        {
            get
            {
                if (bulletMap.ContainsKey(name))
                    return bulletMap[name];
                else
                    return null;
            }
        }
    }
}