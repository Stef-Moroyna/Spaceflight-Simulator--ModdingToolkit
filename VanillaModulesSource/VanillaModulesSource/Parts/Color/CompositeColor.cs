using UnityEngine;

namespace SFS.Parts.Modules
{
    public class CompositeColor : ColorModule
    {
        public ColorModule[] colors;

        public override Color GetColor()
        {
            Color output = Color.white;

            foreach (ColorModule color in colors)
                output *= color.GetColor();

            return output;
        }
    }
}