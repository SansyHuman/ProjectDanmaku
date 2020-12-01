using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using UnityEngine;

namespace SansyHuman.Common
{
    [DisallowMultipleComponent]
    public class ObjectRotation : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private UDETime.TimeScale timeScale;

        private Transform tr;

        // Start is called before the first frame update
        void Start()
        {
            tr = transform;
        }

        // Update is called once per frame
        void Update()
        {
            float deltaTime = Time.deltaTime * UDETime.Instance.GetTimeScale(timeScale);

            Vector3 rot = tr.rotation.eulerAngles;
            rot.z += speed * deltaTime;
            tr.rotation = Quaternion.Euler(rot);
        }
    }
}