using System;
using UnityEngine;

namespace SFS
{
    public class TimeEvent : MonoBehaviour
    {
        public static TimeEvent main;
        void Awake() => main = this;

        public event Action on_100Ms;
        public event Action on_1000Ms;
        public event Action on_10000Ms;

        void LateUpdate()
        {
            float time = Time.time;
            float oldTime = Time.time - Time.deltaTime;

            if ((int)(time * 10) > (int)(oldTime * 10))
                on_100Ms?.Invoke();

            if ((int)time > (int)oldTime)
                on_1000Ms?.Invoke();

            if ((int)(time * 0.1f) > (int)(oldTime * 0.1f))
                on_10000Ms?.Invoke();
        }
    }
}