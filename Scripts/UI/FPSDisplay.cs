using UnityEngine;
using UnityEngine.UI;
using System;
namespace AssemblyCSharp
{
    public class FPSDisplay : MonoBehaviour
    {
        private Text fpsText;

        public float updateInterval = 0.5F;

        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval
        void Awake()
        {
            fpsText = transform.GetChild(0).GetComponent<Text>();
        }

        void Start()
        {
            timeleft = updateInterval;
        }

        void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = accum / frames;
                string format = System.String.Format("{0:F2} FPS", fps);
                fpsText.text = format;

                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
    }
}

