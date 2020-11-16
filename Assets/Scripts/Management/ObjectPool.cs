using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;

using UnityEngine;

namespace SansyHuman.Management
{
    public class ObjectPool : UDESingleton<ObjectPool>
    {
        private class ComponentPrefab : MonoBehaviour
        {
            public Component prefab;
        }

        private class ObjectPrefab : MonoBehaviour
        {
            public GameObject prefab;
        }

        private Dictionary<Component, Stack<Component>> componentPoolList;
        private Dictionary<GameObject, Stack<GameObject>> gameObjectPoolList;

        protected override void Awake()
        {
            base.Awake();
            componentPoolList = new Dictionary<Component, Stack<Component>>();
            gameObjectPoolList = new Dictionary<GameObject, Stack<GameObject>>();
        }

        public void AddNewPool<T>(T prefab, int initialNumber) where T : Component
        {
            if (!componentPoolList.ContainsKey(prefab))
            {
                Stack<Component> pool = new Stack<Component>(initialNumber * 2);

                for (int i = 0; i < initialNumber; i++)
                {
                    T instance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                    GameObject compObj = instance.gameObject;
                    compObj.AddComponent<ComponentPrefab>().prefab = prefab;

                    compObj.SetActive(false);
                    compObj.name = prefab.gameObject.name;
                    compObj.transform.parent = this.transform;

                    pool.Push(instance);
                }

                componentPoolList.Add(prefab, pool);
                return;
            }

            Debug.LogWarning("The pool of the prefab is already exist.");
        }

        public void AddNewPool(GameObject prefab, int initialNumber)
        {
            if (!gameObjectPoolList.ContainsKey(prefab))
            {
                Stack<GameObject> pool = new Stack<GameObject>(initialNumber * 2);

                for (int i = 0; i < initialNumber; i++)
                {
                    GameObject instance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                    instance.AddComponent<ObjectPrefab>().prefab = prefab;
                    instance.SetActive(false);
                    instance.name = prefab.name;
                    instance.transform.parent = this.transform;

                    pool.Push(instance);
                }

                gameObjectPoolList.Add(prefab, pool);
                return;
            }

            Debug.LogWarning("The pool of the prefab is already exist.");
        }

        public T GetObject<T>(T prefab) where T : Component
        {
            T returnInstance = null;

            if (componentPoolList.TryGetValue(prefab, out Stack<Component> targetPool))
            {
                if (targetPool.Count > 0)
                {
                    returnInstance = targetPool.Pop() as T;
                    returnInstance.gameObject.SetActive(true);
                }
                else
                {
                    returnInstance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                    GameObject compObj = returnInstance.gameObject;
                    compObj.AddComponent<ComponentPrefab>().prefab = prefab;

                    compObj.name = prefab.gameObject.name;
                    compObj.transform.parent = this.transform;
                }
            }
            else
            {
                Debug.LogWarning("The target prefab is not found. Create new pool for the bullet.");
                AddNewPool<T>(prefab, 32);
                returnInstance = componentPoolList[prefab].Pop() as T;
                returnInstance.gameObject.SetActive(true);
            }

            return returnInstance;
        }

        public GameObject GetObject(GameObject prefab)
        {
            GameObject returnInstance = null;

            if (gameObjectPoolList.TryGetValue(prefab, out Stack<GameObject> targetPool))
            {
                if (targetPool.Count > 0)
                {
                    returnInstance = targetPool.Pop();
                    returnInstance.SetActive(true);
                }
                else
                {
                    returnInstance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                    returnInstance.AddComponent<ObjectPrefab>().prefab = prefab;
                    returnInstance.name = prefab.name;
                    returnInstance.transform.parent = this.transform;
                }
            }
            else
            {
                Debug.LogWarning("The target prefab is not found. Create new pool for the bullet.");
                AddNewPool(prefab, 32);
                returnInstance = gameObjectPoolList[prefab].Pop();
                returnInstance.SetActive(true);
            }

            return returnInstance;
        }

        public void ReturnObject<T>(T target) where T : Component
        {
            GameObject targetObj = target.gameObject;
            ComponentPrefab prefab = targetObj.GetComponent<ComponentPrefab>();

            if (prefab == null || !componentPoolList.ContainsKey(prefab.prefab))
            {
                Debug.LogError("The target object is not from the object pool.");
                return;
            }

            if (!target.gameObject.activeSelf)
            {
                Debug.LogWarning("You tried to return object that already returned. The return is ignored.");
                return;
            }
            
            targetObj.SetActive(false);
            componentPoolList[prefab.prefab].Push(target);
        }

        public void ReturnObject(GameObject target)
        {
            ObjectPrefab prefab = target.GetComponent<ObjectPrefab>();

            if (prefab == null || !gameObjectPoolList.ContainsKey(prefab.prefab))
            {
                Debug.LogError("The target object is not from the object pool.");
                return;
            }

            if (!target.activeSelf)
            {
                Debug.LogWarning("You tried to return object that already returned. The return is ignored.");
                return;
            }

            target.SetActive(false);
            gameObjectPoolList[prefab.prefab].Push(target);
        }
    }
}