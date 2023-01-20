using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.Variables;
using System.Linq;
using System;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class SkinModule : MonoBehaviour
    {
        // Variables
        [BoxGroup("Ref", false), Required] public string skinTag;
        //
        public PipeMesh[] meshModules;
        ColorTexture[] defaultColors;
        ShapeTexture[] defaultShapes;
        //
        [BoxGroup("Tex", false)] public String_Reference colorTextureName;
        [BoxGroup("Tex", false)] public String_Reference shapeTextureName;
        public bool disableColorSelect, disableShapeSelect;
    }
}