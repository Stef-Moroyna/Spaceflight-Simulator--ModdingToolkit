using SFS.Parts.Modules;
using UnityEngine;

namespace InterplanetaryModule
{
    public class RepeatMoveModule : MonoBehaviour
    {
        public bool activated;
        public float time;
        public float duration;
        public bool unscaledTime;
        public EngineModule engineModule;
        public MoveModule moveModule;

        void Start()
        {
            moveModule.Activate();
        }

        void Update()
        {
            if (!activated)
                return;
            float num = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (num == 0f)
                return;
            float maxDelta = (duration > 0f) ? (num / duration) : 10000f;
            bool active = engineModule || engineModule.engineOn.Value && engineModule.throttle_Out.Value > 0f;
            bool finished = moveModule.time.Value >= 1f;
            if (active)
            {
                time = Mathf.MoveTowards(time, 1, maxDelta);
                if (time >= 1)
                    time = 0;
                moveModule.enabled = true;
                moveModule.time.Value = time;
                moveModule.targetTime.Value = time;
            }
            else
            {
                if (!finished)
                    time = Mathf.MoveTowards(time, 1, maxDelta);
                else
                    moveModule.enabled = false;
            }
        }
    }
}