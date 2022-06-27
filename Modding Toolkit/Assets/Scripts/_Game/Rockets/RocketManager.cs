using System;
using SFS.Builds;
using SFS.Parts;
using SFS.Parts.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.World
{
    public class RocketManager : MonoBehaviour
    {
        public Rocket rocketPrefab;
        static Rocket prefab;

        void Awake() => prefab = rocketPrefab;
    }

    public enum DestructionReason
    {
        TerrainCollision,
        RocketCollision,
        Overheat,
        Intentional
    }
}