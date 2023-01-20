using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.Parts.Modules 
{
    public class EngineModule : MonoBehaviour
    {
        [BoxGroup("Ref", false), SuffixLabel("t")] public Composed_Float thrust;
        [BoxGroup("Ref", false)] public Composed_Vector2 thrustNormal = new Composed_Vector2(Vector2.up);
        //
        [BoxGroup("Ref", false)] public Composed_Float ISP;
        [BoxGroup("Ref", false)] public Composed_Vector2 thrustPosition = new Composed_Vector2(Vector2.zero);
        [BoxGroup("Ref", false), Required] public FlowModule source;
        //
        [BoxGroup("Ref", false)] public bool hasGimbal = true;
        [BoxGroup("Ref", false), ShowIf("hasGimbal")] public Bool_Reference gimbalOn;
        [BoxGroup("Ref", false), ShowIf("hasGimbal")] public MoveModule gimbal;
        //
        [BoxGroup("State", false)] public Bool_Reference engineOn;
        [BoxGroup("State", false)] public Float_Reference throttle_Out;
        
        // Patch
        public Bool_Reference heatOn;
        [Required] public GameObject heatHolder;
        
        public void ToggleEngine()
        {
        }
        
        [Space]
        public float oldMass = float.NaN;
    }
}