using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SFS.Parts.Modules
{
    public class ParticleModule : MonoBehaviour
    {
        WorldParticle[] Options => Resources.LoadAll<WorldParticle>(string.Empty);
        [Required, ValueDropdown(nameof(Options))] public WorldParticle particle;
        [Space]
        public float velocityRange;
        public int particleCount;
        
        public bool atStart;
        
        public void Spawn()
        {
        }
    }
}