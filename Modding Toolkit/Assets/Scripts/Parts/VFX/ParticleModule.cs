using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SFS.Parts.Modules
{
    public class ParticleModule : MonoBehaviour
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
        
        [Button]
        public void Spawn()
        {
           
            Vector3 pos = transform.position;
            Vector3 vel = Rocket != null ? Rocket.rb2d.velocity : Vector2.zero;
            
            (Vector3, Vector3)[] output = new (Vector3, Vector3)[particleCount];
            for (int i = 0; i < output.Length; i++)
            {
                Vector2 direction = Random.insideUnitCircle * Random.Range(0.5f, 1);
                output[i] = (pos + transform.TransformVector(direction * new Vector2(1.3f, 0.1f)), vel + transform.TransformVector(direction * new Vector2(1.0f, 0.7f) * velocityRange));
            }
            
            // Spawns
            particle.GetInstance().Spawn(output);
        }
        
        // For SRB
        void OnFixedUpdate()
        {
           
            Vector3 pos = transform.position;
            Vector3 vel = default; //rb2d.velocity + (Vector2)transform.TransformDirection(thrustNormal.Value) * -100;
            
            (Vector3, Vector3)[] output = new (Vector3, Vector3)[1];
            for (int i = 0; i < output.Length; i++)
            {
                Vector2 circle = Random.insideUnitCircle;
                
                output[i] =            
                (
                    pos + transform.TransformVector(circle),
                    vel + transform.TransformVector(circle * 10)
                );
            }
            
            // Spawns
            //particles.GetInstance().Spawn(output);
        }
    }

    // public class Particle
    // {
    //     
    // }  
}

// public void SpawnLaunchSmokeParticles()
// {
//    WorldParticle a = EffectManager.main.launchSmoke;
//     
//     a.Spawn(
//         WorldView.ToGlobalPosition(transform.position),
//         WorldView.ToGlobalVelocity(Rocket != null? Rocket.rb2d.velocity : Vector2.zero), particleCount,
//         () =>
//         {
//             Vector2 c = Random.insideUnitCircle * Random.Range(0.5f, 1);
//
//             return
//             (
//                 Vector3.zero,
//                 transform.TransformVector(new Vector2(c.x, -1) * velocityRange)
//             );
//         });
// }