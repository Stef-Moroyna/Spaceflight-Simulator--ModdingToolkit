using UnityEngine;

namespace SFS.Parts.Modules
{
    public class FlameRandomizer : MonoBehaviour
    {
        public Vector2 min, max;

        void Update()
        {
            if (Time.timeScale == 0.0f)
                return;
            transform.localScale = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        }
    }
}