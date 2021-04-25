using System.Collections;
using System.Collections.Generic;

using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util.Math;

using UnityEngine;

namespace SansyHuman.Common
{
    [DisallowMultipleComponent]
    public class BackgroundTilt : MonoBehaviour
    {
        [SerializeField]
        private float maxAngle;

        [SerializeField]
        private float period;

        [SerializeField]
        private UDETime.TimeScale timeScale;

        private Transform tr;
        private float accTime;

        // Start is called before the first frame update
        void Start()
        {
            tr = transform;
            accTime = 0;
        }

        // Update is called once per frame
        void Update()
        {
            accTime += Time.deltaTime * UDETime.Instance.GetTimeScale(timeScale);
            float theta = 2 * Mathf.PI * (accTime / period) + Mathf.PI / 4;
            float r = Mathf.Sin(theta);
            r = r * r;
            (float x, float y) = UDEMath.Polar2Cartesian(r, theta * Mathf.Rad2Deg);

            Vector3 rot = tr.rotation.eulerAngles;
            rot.x = x * maxAngle;
            rot.y = y * maxAngle;
            tr.rotation = Quaternion.Euler(rot);
        }
    }
}