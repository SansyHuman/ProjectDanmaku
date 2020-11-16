using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.Debugging
{
    [DisallowMultipleComponent]
    public class FPS : MonoBehaviour
    {
        private float avgFps = 0.0f;
        private float avgInsFps = 0.0f;
        private float low1pc = 0.0f;
        private float low0d1pc = 0.0f;

        private float avgFs = 0.0f;
        private float avgInsFs = 0.0f;

        private long updateCnt = 0;
        private float accTime = 0.0f;
        private int internalCnt = 0;

        private SortedList<int, long> fpsDist = new SortedList<int, long>();

        [SerializeField]
        private Text fps;

        [SerializeField]
        [Range(0, 1)]
        private float updateInterval = 0.5f;

        void Update()
        {
            float dt = Time.unscaledDeltaTime;

            updateCnt++;
            accTime += dt;
            internalCnt++;

            float instFps = 1.0f / dt;

            avgFps = (avgFps * (updateCnt - 1) + instFps) / updateCnt;
            avgFs = (avgFs * (updateCnt - 1) + dt * 1000f) / updateCnt;


            int instFpsX10 = (int)(instFps * 10.0f);
            if (!fpsDist.ContainsKey(instFpsX10))
                fpsDist.Add(instFpsX10, 0);
            fpsDist[instFpsX10]++;

            if (accTime >= updateInterval)
            {
                avgInsFps = internalCnt / accTime;
                avgInsFs = accTime * 1000f / internalCnt;

                long low1cnt = updateCnt / 100;
                long low0d1cnt = updateCnt / 1000;
                if (low1cnt == 0)
                    low1cnt = 1;
                if (low0d1cnt == 0)
                    low0d1cnt = 1;

                long acc = 0;
                int i = 0;
                for (; i <= fpsDist.Count; i++)
                {
                    if (acc >= low0d1cnt || i == fpsDist.Count)
                    {
                        low0d1pc = (float)fpsDist.Keys[i - 1] / 10.0f;
                        break;
                    }
                    acc += fpsDist.Values[i];  
                }

                for (; i <= fpsDist.Count; i++)
                {
                    if (acc >= low1cnt || i == fpsDist.Count)
                    {
                        low1pc = (float)fpsDist.Keys[i - 1] / 10.0f;
                        break;
                    }
                    acc += fpsDist.Values[i];
                }

                accTime = 0.0f;
                internalCnt = 0;
            }

            fps.text = $"{avgInsFs,-5:0.0} ms ({avgInsFps,-6:0.0} fps)\nAVG {avgFs,-5:0.0} ms ({avgFps,-6:0.0} fps)\nLOW1 {low1pc,-5:0.0} fps LOW0.1 {low0d1pc,-5:0.0} fps";
        }
    }
}