using System;
using UnityEngine;

namespace SFS
{
    public class TimeEvent : MonoBehaviour
    {
        public static TimeEvent main;
        void Awake() => main = this;

        public Action on_100Ms, on_10000Ms;
        
        void LateUpdate()
        {
            float time = Time.unscaledTime;
            float oldTime = Time.unscaledTime - Time.unscaledDeltaTime;

            if ((int)(time * 10) > (int)(oldTime * 10))
                on_100Ms?.Invoke();
            
            if ((int)(time * 0.1f) > (int)(oldTime * 0.1f))
                on_10000Ms?.Invoke();
        }
    }
}