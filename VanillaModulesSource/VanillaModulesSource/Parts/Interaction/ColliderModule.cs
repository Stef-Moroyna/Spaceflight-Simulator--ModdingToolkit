using System;
using System.Collections.Generic;
using System.Linq;
using SFS.World;
using SFS.World.Drag;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class ColliderModule : MonoBehaviour, Rocket.INJ_Rocket
    {
        public ImpactTolerance impactTolerance = ImpactTolerance.Medium;
        public Collider2D ownEngineNozzle;

        // State
        int planetLayer, flameHeatLayer;
        HeatModuleBase heatModule;

        // Injected reference
        Rocket rocket;
        Rocket Rocket.INJ_Rocket.Rocket { set => rocket = value; }


        void Start()
        {
            planetLayer = LayerMask.NameToLayer("Celestial Body");
            flameHeatLayer = LayerMask.NameToLayer("Flame Trigger");
            heatModule = transform.GetComponentInParentTree<HeatModuleBase>();
        }

        //void OnCollisionStay2D(Collision2D collision) => A(collision);

        /*void A(Collision2D collision)
        {
            if (collision.gameObject.layer == planetLayer)
                for (int i = 0; i < collision.contactCount; i++)
                {
                    ContactPoint2D contact = collision.GetContact(i);
                    Vector2 impulse = new Vector2(contact.tangentImpulse, contact.normalImpulse);

                    if (impulse.magnitude > 0.1f)
                        GameManager.main.environment.SpawnGroundParticles(contact.point, rocket.rb2d.GetPointVelocity(contact.point) / 1.5f, contact.normal.Rotate_90(), (int)(impulse.magnitude * 2));
                }
        }*/
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            //A(collision);
            
            if (SandboxSettings.main.settings.unbreakableParts)
                return;

            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            collision.GetContacts(contacts);
            
            float contactVelocity = contacts.Max(x => (Vector2.Dot(x.normal, collision.relativeVelocity) + collision.relativeVelocity.magnitude * 0.5f) / 1.5f);
            if (contactVelocity < MaxImpactTolerance)
                return;
            
            GetPart(out Part part);
            
            ColliderModule other = collision.otherCollider.GetComponent<ColliderModule>();
            if (other != null)
                if (rocket.collisionImmunity > Time.time || part.mass.Value > other.GetComponentInParent<Part>().mass.Value * 5)
                    return;

            
            // List<Joint<Part>> joints = rocket.jointsGroup.joints;
            // bool split = false;
            // if (joints.Count > 1)
            //     rocket.jointsGroup.DestroyJoint(joints[Random.Range(0, joints.Count)], out split, out Rocket newRocket);
            //
            // if (split)
            //     return;
            
            
            DestructionReason reason = DestructionReason.RocketCollision;
            if (collision.gameObject.layer == planetLayer)
            {
                float impactVelocity = contacts.Max(x => collision.relativeVelocity.magnitude);
                rocket.stats.OnCrash(impactVelocity);
                reason = DestructionReason.TerrainCollision;
            }

            part.DestroyPart(rocket, true, reason);
        }
        void GetPart(out Part part)
        {
            Transform a = transform;

            while (true)
            {
                part = a.GetComponent<Part>();

                if (part != null)
                    return;

                a = a.parent;

                if (a == null)
                    throw new Exception("Could not find part in parent tree");
            }
        }
        
        float MaxImpactTolerance => impactTolerance switch
        {
            ImpactTolerance.Low => 2.5f,
            ImpactTolerance.Medium => 5.5f,
            ImpactTolerance.High => 12.5f,
            ImpactTolerance.Wheel => 50.5f,
            _ => throw new Exception()
        };
        public enum ImpactTolerance
        {
            Low,
            Medium,
            High,
            Wheel,
        }
        
        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == flameHeatLayer)
                if (other != ownEngineNozzle)
                {
                    EngineModule engineModule = other.GetComponentInParent<EngineModule>();
                    if (engineModule.heatOn.Value || !Base.worldBase.AllowsCheats)
                        rocket.aero.heatManager.HeatPart(heatModule);
                    
                    // if (rocket == engineModule.Rocket)
                    //     return;
                    // Vector2 thrustPoint = Transform_Utility.LocalToLocalPoint(engineModule, rocket, engineModule.thrustPosition.Value);
                    // Vector2 thrustDirection = Transform_Utility.LocalToLocalDirection(engineModule, rocket, engineModule.thrustNormal.Value).normalized;
                    // Vector2 force = thrustDirection * (float)(engineModule.throttle_Out.Value * engineModule.thrust.Value * Time.deltaTime * -0.6);
                    // rocket.rb2d.AddForceAtPosition(force, thrustPoint, ForceMode2D.Impulse);
                    //
                    // Debug.DrawRay(thrustPoint, force, Color.red);
                }
        }
    }
}