using UnityEngine;

namespace SFS.Parts.Modules
{
    public class CustomColor : ColorModule
    {
        public Color color = Color.white;
        public override Color GetColor() => color;
    }
}
