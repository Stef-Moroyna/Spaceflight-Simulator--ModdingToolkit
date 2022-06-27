using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.Parts.Modules;
using SFS.Parts;
using SFS.Translations;
using System.Linq;
using SFS.Platform;
using SFS.World.Drag;

namespace SFS.World
{
    public partial class Rocket
    {
        // References
        [Required] public Mass_Calculator mass;
        [Required] public Rigidbody2D rb2d;
        [Required] public PartHolder partHolder;

        // SECURITY
        [Required] public GameObject timeManager;
        [Required] public GameObject partManager;

        // State
        public string rocketName;
        public float collisionImmunity;

        List<(ResourceModule[], ResourceModule)> pipeFlows = new List<(ResourceModule[], ResourceModule)>();
    }
}