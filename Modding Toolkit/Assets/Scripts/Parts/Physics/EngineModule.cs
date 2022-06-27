using SFS.Builds;
using SFS.Translations;
using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules 
{
    public class EngineModule : MonoBehaviour
    {
        //#warning merge into thrust vector?
        [BoxGroup("Ref", false), SuffixLabel("t")] public Composed_Float thrust;
        [BoxGroup("Ref", false)] public Composed_Vector2 thrustNormal = new Composed_Vector2(Vector2.up);
        
        
        [BoxGroup("Ref", false)] public Composed_Float ISP;
        [BoxGroup("Ref", false)] public Composed_Vector2 thrustPosition = new Composed_Vector2(Vector2.zero);
        [BoxGroup("Ref", false), Required] public FlowModule source;
        [BoxGroup("Ref", false)] public MoveModule gimbal;
        //
        [BoxGroup("State", false)] public Bool_Reference engineOn;
        [BoxGroup("State", false)] public Float_Reference throttle_Out;
        
        // Patch
        public Bool_Reference heatOn;
        public GameObject heatHolder;
        Vector3 originalPosition;
        
        // Data Injection
        public bool IsPlayer { get; set; }
        public Rigidbody2D Rb2d { get; set; }
        //
        readonly Float_Local throttle_Input = new Float_Local();
        readonly Float_Local turnAxis_Input = new Float_Local();


        void Start()
        {
            source.onStateChange += CheckOutOfFuel;
            
            thrust.OnChange += RecalculateMassFlow;
            ISP.OnChange += RecalculateMassFlow;
            throttle_Out.OnChange += RecalculateMassFlow;
            
            throttle_Out.OnChange += UpdateApplyPhysics;


            engineOn.OnChange += RecalculateEngineThrottle;
            throttle_Input.OnChange += RecalculateEngineThrottle;
            
            throttle_Out.OnChange += RecalculateGimbal;
            turnAxis_Input.OnChange += RecalculateGimbal;
            
            if (heatHolder != null)
                heatHolder.gameObject.SetActive(heatOn.Value);
        }

        void OnDestroy()
        {
        }

        // On Change
        void RecalculateMassFlow()
        {
        }
        void CheckOutOfFuel()
        {
        }
        void RecalculateEngineThrottle()
        {
            throttle_Out.Value = engineOn.Value ? throttle_Input.Value : 0;
        }
        void RecalculateGimbal()
        {
            if (gimbal != null)
                gimbal.targetTime.Value = throttle_Out.Value > 0 ? turnAxis_Input.Value * transform.RotationDirection() : 0;
        }

        
        // Applies thrust
        void UpdateApplyPhysics()
        {
            enabled = Rb2d != null && throttle_Out.Value > 0;
        }
        void FixedUpdate()
        {
            if (Rb2d == null)
                return;

            Vector2 force = transform.TransformVector(thrustNormal.Value * (thrust.Value * 9.8f * throttle_Out.Value));
            Vector2 position = Rb2d.GetRelativePoint(Transform_Utility.LocalToLocalPoint(transform, Rb2d, thrustPosition.Value));

            
            Rb2d.AddForceAtPosition(force, position, ForceMode2D.Force);
            PositionFlameHitbox();
        }
        void PositionFlameHitbox(Vector2 _) => PositionFlameHitbox();
        void PositionFlameHitbox() => heatHolder.transform.localPosition = originalPosition + heatHolder.transform.parent.InverseTransformVector(Rb2d.velocity * Time.fixedDeltaTime);

        
        
        // Functions
        public void ToggleEngine()
        {
        }


        //#warning Saturn V will have wrong values in RSS, we need a better solution, maybe a separate module?
        public float oldMass;
    }
}