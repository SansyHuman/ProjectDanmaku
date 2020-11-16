using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;

using UnityEngine;

namespace SansyHuman.Player
{
    [DisallowMultipleComponent]
    public class RotateSlowMarker : MonoBehaviour
    {
        private Transform thisTr;

        [SerializeField]
        private float rotationSpeed = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            thisTr = transform;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 euler = thisTr.rotation.eulerAngles;
            euler.z += rotationSpeed * UDETime.Instance.PlayerTimeScale * Time.deltaTime;
            if (euler.z > 360f)
                euler.z -= 360f;
            if (euler.z < 0f)
                euler.z += 360f;
            thisTr.rotation = Quaternion.Euler(euler);
        }
    }
}