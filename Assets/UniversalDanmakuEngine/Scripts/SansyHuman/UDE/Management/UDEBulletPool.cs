// Copyright (c) 2019 Subo Lee (KAIST HAJE)
// Please direct any bugs/comments/suggestions to suboo0308@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Object;

namespace SansyHuman.UDE.Management
{
    /// <summary>
    /// Object pool of bullets.
    /// </summary>
    [AddComponentMenu("UDE/Management/Bullet Pool")]
    public class UDEBulletPool : UDESingleton<UDEBulletPool>
    {
        /// <summary>
        /// Internal class to represent pooling bullets and their initial number
        /// </summary>
        [Serializable]
        public struct InitializingObject
        {
            /// <summary>
            /// A prefab that has a component <see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> or its children
            /// </summary>
            public UDEAbstractBullet prefab;
            /// <summary>
            /// Initial number of the bullet
            /// </summary>
            public int initialNumber;
        }

        [SerializeField]
        private List<InitializingObject> initializingObjectList = null;

        private Dictionary<UDEAbstractBullet, Stack<UDEAbstractBullet>> poolList;

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Management.UDESingleton{UDEBulletPool}.Awake()"/>.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            CreatePools();
        }

        private void CreatePools()
        {
            if (initializingObjectList == null)
                initializingObjectList = new List<InitializingObject>();
                
            poolList = new Dictionary<UDEAbstractBullet, Stack<UDEAbstractBullet>>(initializingObjectList.Count);
            for (int i = 0; i < initializingObjectList.Count; i++)
            {
                InitializingObject init = initializingObjectList[i];
                AddNewPool(init.prefab, init.initialNumber);
            }
        }

        /// <summary>
        /// Create new pool of the bullet prefab
        /// </summary>
        /// <param name="prefab"><see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> prefab</param>
        /// <param name="initialNumber">Initial number of bullets</param>
        public void AddNewPool(UDEAbstractBullet prefab, int initialNumber)
        {
            try
            {
                _ = poolList[prefab];
            }
            catch (KeyNotFoundException) // When the pool of the prefab does not exist.
            {
                Stack<UDEAbstractBullet> pool = new Stack<UDEAbstractBullet>(initialNumber * 2);
                for (int j = 0; j < initialNumber; j++)
                {
                    UDEAbstractBullet instance = Instantiate<UDEAbstractBullet>(prefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
#pragma warning disable CS0618 // Type or member is obsolete
                    instance.OriginPrefab = prefab;
#pragma warning restore CS0618 // Type or member is obsolete
                    GameObject instObj = instance.gameObject;
                    instObj.SetActive(false);
                    instObj.name = prefab.gameObject.name;
                    instObj.transform.parent = this.transform;
                    pool.Push(instance);
                }
                poolList.Add(prefab, pool);
                return;
            }

            Debug.LogWarning("The pool of the prefab is already exist.");
        }

        /// <summary>
        /// Deletes pool if all bullets in the pool are inactive, else just delete bullets that are inactive and leaves the pool.
        /// <para>If you set <paramref name="deleteActiveBullets"/> to <c>true</c>, It will destroy all the bullets
        /// even if they are active and delete the pool.</para>
        /// </summary>
        /// <param name="prefab"><see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> prefab to destroy</param>
        /// <param name="deleteActiveBullets">If true, delete all bullets in the pool even if they are active. The default value is <c>false</c></param>
        public void ClearPool(UDEAbstractBullet prefab, bool deleteActiveBullets = false)
        {
            Stack<UDEAbstractBullet> pool;
            if (!poolList.TryGetValue(prefab, out pool))
            {
                Debug.LogWarning("The pool of the prefab does not exist. Pool deletion will be ignored.");
                return;
            }

            Stack<UDEAbstractBullet> temp = new Stack<UDEAbstractBullet>(pool.Count);

            for (int i = 0; i < pool.Count; i++)
            {
                UDEAbstractBullet bullet = pool.Pop();
                if (!deleteActiveBullets && bullet.gameObject.activeSelf)
                    temp.Push(bullet);
                else
                    Destroy(bullet.gameObject);
            }

            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                    pool.Push(temp.Pop());
            }
            else
            {
                poolList.Remove(prefab);
            }
        }

        /// <summary>
        /// Deletes all existing pools if all bullets in the pool are inactive, else just delete bullets that are inactive and leaves the pool.
        /// <para>If you set <paramref name="deleteActiveBullets"/> to <c>true</c>, It will destroy all the bullets
        /// even if they are active and delete the pool.</para>
        /// </summary>
        /// <param name="deleteActiveBullets">If true, delete all bullets in the pool even if they are active. The default value is <c>false</c></param>
        public void ClearAllPools(bool deleteActiveBullets = false)
        {
            List<UDEAbstractBullet> pools = new List<UDEAbstractBullet>(poolList.Keys);
            for (int i = 0; i < pools.Count; i++)
                ClearPool(pools[i], deleteActiveBullets);
        }

        /// <summary>
        /// Gets <see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> instance of the target object.
        /// </summary>
        /// <param name="target">Bullet object to get</param>
        /// <returns><see cref="SansyHuman.UDE.Object.UDEAbstractBullet"/> instance</returns>
        public UDEAbstractBullet GetBullet(UDEAbstractBullet target)
        {
            UDEAbstractBullet returnInstance = null;

            Stack<UDEAbstractBullet> targetPool;
            if (poolList.TryGetValue(target, out targetPool))
            {
                if (targetPool.Count > 0)
                {
                    UDEAbstractBullet inst = targetPool.Pop();
                    inst.gameObject.SetActive(true);
                    returnInstance = inst;
                }
                else // When there is no more bullet in the pool.
                {
                    UDEAbstractBullet instance = Instantiate<UDEAbstractBullet>(target, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
#pragma warning disable CS0618 // Type or member is obsolete
                    instance.OriginPrefab = target;
#pragma warning restore CS0618 // Type or member is obsolete
                    GameObject instObj = instance.gameObject;
                    instObj.name = target.gameObject.name;
                    instObj.transform.parent = this.transform;
                    returnInstance = instance;
                }
            }
            else // When there is no object pool of the target bullet.
            {
                Debug.LogWarning("The target bullet is not found. Create new pool for the bullet.");
                AddNewPool(target, 32);
                returnInstance = poolList[target].Pop();
                returnInstance.gameObject.SetActive(true);
            }

            return returnInstance;
        }

        /// <summary>
        /// Returns the bullet object to the pool.
        /// <para>If you try to release the bullet that already released, it will be ignored.</para>
        /// </summary>
        /// <param name="target">Bullet object to release</param>
        public void ReleaseBullet(UDEAbstractBullet target)
        {
            if (!target.gameObject.activeSelf)
            {
                Debug.LogWarning("You tried to release bullet that already released. THe release is ignored.");
                return;
            }
            target.gameObject.SetActive(false);
#pragma warning disable CS0618 // Type or member is obsolete
            poolList[target.OriginPrefab].Push(target);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Returns multiple bullets to the pool.
        /// </summary>
        /// <param name="targets">Bullets to release</param>
        public void ReleaseBullets(params UDEAbstractBullet[] targets)
        {
            for (int i = 0; i < targets.Length; i++)
                ReleaseBullet(targets[i]);
        }
    }
}