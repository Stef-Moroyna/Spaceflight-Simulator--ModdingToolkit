using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;


namespace SFS.Parts.Modules
{
    public class BoosterModule : MonoBehaviour, I_PartMenu, I_InitializePartModule
    {
        public bool showDescription = true;

        [Required] public SurfaceData surfaceForCover;
        [Required] public ResourceType resourceType;
        public Composed_Float ISP;
        public Composed_Vector2 thrustVector;
        public Composed_Vector2 thrustPosition;

        [BoxGroup] public Composed_Float wetMass;
        [BoxGroup] public Composed_Float dryMassPercent;
        [BoxGroup] public Float_Reference fuelPercent;
        //
        public Bool_Reference boosterPrimed;
        float newIgnitionTime = -1f;
        //
        [BoxGroup("Output", false)] public Float_Reference throttle_Out;
        [BoxGroup("Output", false)] public Double_Reference mass_Out = new Double_Reference();

        
        // Patch
        public Bool_Reference heatOn;
        public GameObject heatHolder;
        Vector3 originalPosition;
        
        // Injected
        public Rigidbody2D Rb2d { get; set; }
        Float_Local throttle_Input = new Float_Local();
        public float Throttle { set => throttle_Input.Value = value; }
        // Get
        float ThrustDuration => FuelMass / (thrustVector.Value.magnitude / (ISP.Value));
        float FuelMass => (1.0f - DryMassPercent) * wetMass.Value;
        float DryMassPercent => dryMassPercent.Value;

        // Setup
        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize()
        {
            enabled = fuelPercent.Value < 1.0f && fuelPercent.Value > 0.0f;

            throttle_Input.OnChange += TryIgnite;
            
            wetMass.OnChange += RecalculateMass;
            dryMassPercent.OnChange += RecalculateMass;
            fuelPercent.OnChange += RecalculateMass;
            
            void RecalculateMass() => mass_Out.Value = DryMassPercent * wetMass.Value + FuelMass * fuelPercent.Value;
        }

        // Activation
        public void Fire()
        {
        }
        void TryIgnite()
        {
            if (boosterPrimed.Value && throttle_Input.Value > 0.01f)
            {
                boosterPrimed.Value = false;
                enabled = true;
            }
        }
    }
}