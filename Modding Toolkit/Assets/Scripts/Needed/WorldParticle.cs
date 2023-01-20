using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SFS.World
{
    public class WorldParticle : MonoBehaviour
    {
        public ParticleSystem effect;
        public float drag;
        public bool gas;
    }
}