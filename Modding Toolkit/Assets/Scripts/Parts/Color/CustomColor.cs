using UnityEngine;

namespace SFS.Parts.Modules
{
    public class CustomColor : ColorModule
    {
        public Color color;

        public override Color GetColor()
        {
            return color;
        }
    }
}
