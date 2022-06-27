using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//#warning PARTICLES / physics mode occasionally spawns it ahead
//#warning PARTICLES / Optimize drag

namespace SFS.World
{
    public class WorldParticle : MonoBehaviour
    {
        public ParticleSystem effect;
        public float drag;
        public bool gas;
        
        
        // Instances
        static Dictionary<WorldParticle, WorldParticle> instances = new Dictionary<WorldParticle, WorldParticle>();
        public WorldParticle GetInstance()
        {
            if (!instances.ContainsKey(this))
            {
                if (instances.ContainsValue(this))
                    throw new Exception("Your trying to clone a instance");
                
                WorldParticle particle = Instantiate(this);
                instances.Add(this, particle);
                SceneManager.sceneUnloaded += a => instances.Remove(this);
            }

            return instances[this];
        }


        // Create
        public void Spawn((Vector3, Vector3)[] particles)
        {
            bool realtimePhysics = true;

            foreach ((Vector3 pos, Vector3 vel) in particles)
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = pos,
                    velocity = vel * (realtimePhysics? 1 + Time.fixedDeltaTime : 1)
                };
                
                effect.Emit(emitParams, 1);
            }
        }
        
        
        // Lifetime and simulation
        void Start()
        {
            // We simulate on our own instead
            // I believe don't set it to 0 so updates still happen?
            ParticleSystem.MainModule a = effect.main;
            a.simulationSpeed = 0.001f;
        }
        
        // Applies change to particles
        void Apply(Action<ParticleSystem.Particle[]> action)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[effect.particleCount];
            int count = effect.GetParticles(particles);
            
            action.Invoke(particles);

            effect.SetParticles(particles, count); 
        } 
    }
}

//public Action spawnBuffer;
//spawnBuffer?.Invoke();
//spawnBuffer = null;