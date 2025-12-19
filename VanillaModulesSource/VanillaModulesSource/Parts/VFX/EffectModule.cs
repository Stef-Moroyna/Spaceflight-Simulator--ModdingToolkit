using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class EffectModule : MonoBehaviour
    {
        public GameObject effectPrefab;
        public bool attach;
        public float lifetime = 10.0f;

        public void Spawn(Transform holder)
        {
            Transform effect = EffectManager.CreateEffect(effectPrefab.transform, holder.position, lifetime);

            effect.rotation = holder.rotation;

            if (attach)
                effect.transform.SetParent(holder);
        }
    }
}