using UnityEngine;
using SFS.Builds;

namespace SFS.Parts.Modules
{
    public class BuildColor : ColorModule
    {
        public Color buildColor = Color.white;

        public override Color GetColor()
        {
            return buildColor;
        }
    }
}