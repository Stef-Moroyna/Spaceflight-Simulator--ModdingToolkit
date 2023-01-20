using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class BoosterModule : MonoBehaviour
    {
        public bool showDescription = true;
        [Space]
        public SurfaceData surfaceForCover;
        [Space]
        [Required] public ResourceType resourceType;
        public Composed_Float ISP;
        public Composed_Vector2 thrustVector;
        public Composed_Vector2 thrustPosition;

        [BoxGroup] public Composed_Float wetMass;
        [BoxGroup] public Composed_Float dryMassPercent;
        [BoxGroup] public Float_Reference fuelPercent;
        //
        public Bool_Reference boosterPrimed;
        //
        [BoxGroup("Output", false)] public Float_Reference throttle_Out;
        [BoxGroup("Output", false)] public Double_Reference mass_Out = new Double_Reference();
        
        // Patch
        public Bool_Reference heatOn;
        [Required] public GameObject heatHolder;


        public void Fire()
        {
            
        }
    }
}