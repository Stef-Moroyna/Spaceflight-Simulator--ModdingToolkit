using SFS;
using SFS.Parts;
using SFS.World;
using UnityEngine;

namespace Parts.Transform
{
    public class CraftFlipModule : MonoBehaviour
    {
        public void Flip()
        {
            Part_Utility.GetBuildColliderBounds_WorldSpace(out Rect rect, true, Rocket.partHolder.parts.ToArray());
            Vector2 pivot = Rocket.partHolder.transform.InverseTransformPoint(rect.center.Round(new Vector2(0.25f, 0.25f)));
            Part_Utility.ApplyOrientationChange(new Orientation(-1, 1, 0), pivot, Rocket.partHolder.parts);
        }

        public Rocket Rocket { private get; set; }
    }
}