using SFS.UI;
using SFS.Variables;
using SFS.World;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SFS.Career;
using SFS.Translations;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class BoosterModule : MonoBehaviour, Rocket.INJ_Rocket, I_PartMenu, I_InitializePartModule, ResourceDrawer.I_Resource, Rocket.INJ_Throttle
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
        float newIgnitionTime = -1f;
        //
        [BoxGroup("Output", false)] public Float_Reference throttle_Out;
        [BoxGroup("Output", false)] public Double_Reference mass_Out = new Double_Reference();
        [BoxGroup("Output", false)] public bool setDensity = true;
        
        // Patch
        public Bool_Reference heatOn;
        [Required] public GameObject heatHolder;
        Vector3 originalPosition;


        // Injected
        public Rocket Rocket { get; set; }
        Float_Local throttle_Input = new Float_Local();
        public float Throttle { set => throttle_Input.Value = value; }
        // Ref
        Part part;
        
        // Get
        float ThrustDuration => FuelMass / (thrustVector.Value.magnitude / (ISP.Value * (float)Base.worldBase.settings.difficulty.IspMultiplier));
        float FuelMass => (1.0f - DryMassPercent) * wetMass.Value;
        float DryMassPercent => dryMassPercent.Value * (float)Base.worldBase.settings.difficulty.DryMassMultiplier;
        
        
        // Description
        void I_PartMenu.Draw(StatsMenu drawer, PartDrawSettings settings)
        {
            if (!showDescription)
                return;
            
            drawer.DrawStat(51, thrustVector.Value.magnitude.ToThrustString());
            drawer.DrawStat(50, ThrustDuration.ToBurnTimeString(false));
            drawer.DrawStat(49, ISP.Value.ToEfficiencyString());
            
            string fuelAmount = FuelMass.ToString(2, false) + resourceType.resourceUnit.Field;
            drawer.DrawStat(48, Loc.main.Info_Resource_Amount.InjectField(resourceType.displayName, "resource", true).Inject(fuelAmount, "amount"));
        }
        
        // Setup
        int I_InitializePartModule.Priority => -1;
        void I_InitializePartModule.Initialize()
        {
            part = transform.GetComponentInParentTree<Part>();

            enabled = fuelPercent.Value is < 1.0f and > 0.0f;

            throttle_Input.OnChange += TryIgnite;
            
            wetMass.OnChange += RecalculateMass;
            dryMassPercent.OnChange += RecalculateMass;
            fuelPercent.OnChange += RecalculateMass;
        }
        void RecalculateMass()
        {
            mass_Out.Value = DryMassPercent * wetMass.Value + FuelMass * fuelPercent.Value;
                
            if (setDensity)
                part.density = Mathf.Lerp(DryMassPercent, 1, fuelPercent.Value) * resourceType.density;
        }
        
        void Start()
        {
            heatHolder.gameObject.SetActive(heatOn.Value);
            
            if (GameManager.main != null)
            {
                originalPosition = heatHolder.transform.localPosition;
                WorldView.main.onVelocityOffset += PositionFlameHitbox;
            }
        }

        void OnDestroy()
        {
            if (GameManager.main != null)
                WorldView.main.onVelocityOffset -= PositionFlameHitbox;
        }

        // Activation
        public void Fire()
        {
            // Check if can use booster
            if (!CanUseBooster(this, Rocket.isPlayer.Value))
                return;

            // Regular
            if (CareerState.main.HasFeature(WorldSave.CareerState.throttleFeature))
            {
                boosterPrimed.Value = !boosterPrimed.Value;
                TryIgnite();
                
                if (!enabled)
                    MsgDrawer.main.Log(boosterPrimed.Value? Loc.main.Booster_On : Loc.main.Booster_Off);
                
                return;
            }

            // For career
            BoosterModule[] usableBoosters = transform.GetComponentInParentTree<Rocket>().partHolder.GetModules<BoosterModule>().Where(b => CanUseBooster(b, false)).ToArray();
            if (usableBoosters.Length == 1)
            {
                // Ignites instantly
                Ignite();
            }
            else
            {
                // Syncs ignition
                newIgnitionTime = Time.unscaledTime + 0.75f;
                usableBoosters.Where(b => b.newIgnitionTime != -1).ForEach(b => b.newIgnitionTime = newIgnitionTime);
                StartCoroutine(FireDelayed());
                
                IEnumerator FireDelayed()
                {
                    while (Time.unscaledTime < newIgnitionTime)
                        yield return new WaitForFixedUpdate();

                    Ignite();
                }
            }
        }
        void TryIgnite()
        {
            if (boosterPrimed.Value && throttle_Input.Value > 0.01f && CanUseBooster(this, false))
                Ignite();
        }
        //
        public void Fire_Instantly()
        {
            if (CanUseBooster(this, Rocket.isPlayer.Value))
                Ignite();
        }
        void Ignite()
        {
            boosterPrimed.Value = false;
            throttle_Out.Value = 1;
            enabled = true;   
        }
        //
        bool CanUseBooster(BoosterModule boosterModule, bool showMsg)
        {
            if (surfaceForCover != null && SurfaceData.IsSurfaceCovered(surfaceForCover))
            {
                if (showMsg)
                    MsgDrawer.main.Log(Loc.main.Cannot_Ignite_Covered_Booster);

                return false;
            }
            if (boosterModule.enabled)
            {
                if (showMsg)
                    MsgDrawer.main.Log(Loc.main.Booster_Cannot_Be_Off);
                
                return false;
            }
            if (!SandboxSettings.main.settings.infiniteFuel && boosterModule.fuelPercent.Value == 0)
            {
                if (showMsg)
                    MsgDrawer.main.Log(Loc.main.Msg_No_Resource_Left.InjectField(resourceType.displayName, "resource", true));
                
                return false;
            }
            
            return true;
        }

        // Thrust
        void FixedUpdate()
        {
            if (Rocket == null)
                return;
            
            // Removes fuel
            if (!SandboxSettings.main.settings.infiniteFuel)
            {
                fuelPercent.Value -= Time.fixedDeltaTime / ThrustDuration * throttle_Out.Value;
                
                if (fuelPercent.Value <= 0)
                {
                    // Out of fuel
                    throttle_Out.Value = 0;
                    fuelPercent.Value = 0;
                    enabled = false;
                    
                    if (Rocket.isPlayer.Value)
                        MsgDrawer.main.Log(Loc.main.Msg_No_Resource_Left.InjectField(resourceType.displayName, "resource", true));
                    
                    return;
                }   
            }
            
            // Force
            Rigidbody2D rb2d = Rocket.rb2d;
            Vector2 force_Local = thrustVector.Value * 9.8f;
            Vector2 force = Base.worldBase.AllowsCheats? transform.TransformVector(force_Local) : transform.TransformVectorUnscaled(force_Local);
            Vector2 position = rb2d.GetRelativePoint(Transform_Utility.LocalToLocalPoint(transform, rb2d, thrustPosition.Value));
            rb2d.AddForceAtPosition(force, position, ForceMode2D.Force);
            
            // Heat
            PositionFlameHitbox();
        }
        void PositionFlameHitbox(Vector2 _) => PositionFlameHitbox();
        void PositionFlameHitbox() => heatHolder.transform.localPosition = originalPosition + heatHolder.transform.parent.InverseTransformVector(Rocket.rb2d.linearVelocity * Time.fixedDeltaTime);


        // Implementation
        ResourceType ResourceDrawer.I_Resource.ResourceType => resourceType;
        float ResourceDrawer.I_Resource.WetMass => wetMass.Value;
        Double_Reference ResourceDrawer.I_Resource.ResourcePercent => fuelPercent;
    }
}