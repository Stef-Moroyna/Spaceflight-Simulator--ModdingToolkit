using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SFS.Parts.Modules
{
    public class ParticleModule : MonoBehaviour, Rocket.INJ_Rocket
    {
        WorldParticle[] Options => ResourcesLoader.GetFiles_Array<WorldParticle>("");
        [Required, ValueDropdown(nameof(Options))] public WorldParticle particle;
        [Space]
        public float velocityRange;
        public int particleCount;

        // Injected
        public Rocket Rocket { get; set; }

        public bool atStart;

        void Start()
        {
            if (atStart)
                Spawn();
        }
        
        // For separator
        public void Spawn()
        {
            Vector3 pos = transform.position;
            Vector3 vel = Rocket != null ? Rocket.rb2d.linearVelocity : Vector2.zero;
            
            (Vector3, Vector3)[] output = new (Vector3, Vector3)[particleCount];
            for (int i = 0; i < output.Length; i++)
            {
                Vector2 direction = Random.insideUnitCircle * Random.Range(0.5f, 1);
                output[i] = (pos + transform.TransformVector(direction * new Vector2(1.3f, 0.1f)), vel + transform.TransformVector(direction * new Vector2(1.0f, 0.7f) * velocityRange));
            }
            
            particle.Spawn(output);
        }
    }
}