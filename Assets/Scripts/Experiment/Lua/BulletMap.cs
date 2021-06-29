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
        [SerializeField]
        [Tooltip("Bullet prefabs")]
        private List<UDEAbstractBullet> bullets;

        private Dictionary<string, UDEAbstractBullet> bulletMap;

        protected override void Awake()
        {
            base.Awake();

            bulletMap = new Dictionary<string, UDEAbstractBullet>();

            for (int i = 0; i < bullets.Count; i++)
            {
                bulletMap.Add(bullets[i].gameObject.name, bullets[i]);
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

        /// <summary>
        /// Adds a bullet to the map. If the bullet already exists, then ignored.
        /// </summary>
        /// <param name="bullet">Bullet to add</param>
        public void AddBullet(UDEAbstractBullet bullet)
        {
            if (bulletMap.ContainsKey(bullet.gameObject.name))
            {
                Debug.LogError("Bullet map already has the bullet.");
                return;
            }

            bulletMap.Add(bullet.gameObject.name, bullet);
        }

        /// <summary>
        /// Removes a bullet from the map. If the bullet does not exist, then ignored.
        /// </summary>
        /// <param name="bullet">Bullet to remove</param>
        public void RemoveBullet(UDEAbstractBullet bullet)
        {
            if (bulletMap.ContainsKey(bullet.gameObject.name))
            {
                bulletMap.Remove(bullet.gameObject.name);
            }
            else
            {
                Debug.LogError("Bullet map does not have the bullet.");
            }
        }
    }
}