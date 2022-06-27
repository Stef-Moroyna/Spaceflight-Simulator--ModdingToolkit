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
    public class ColliderModule : MonoBehaviour
    {
        public ImpactTolerance impactTolerance = ImpactTolerance.Medium;
        public Collider2D ownEngineNozzle;

        protected Collider2D collider2d;
        int flameHeatLayer;
        I_HeatModule heatModule;
        [NonSerialized] public HeatManager heatManager;

        // Injected reference
        Rocket rocket;


        void Start()
        {
            flameHeatLayer = LayerMask.NameToLayer("Flame Trigger");
            heatModule = transform.GetComponentInParentTree<I_HeatModule>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            collision.GetContacts(contacts);
            
            float contactVelocity = contacts.Select(x => Vector2.Dot(x.normal, collision.relativeVelocity)).OrderByDescending(x => x).FirstOrDefault();
            if (contactVelocity < MaxImpactTolerance)
                return;

            // Destroys part
            GetPart(out Part part);
            
            ColliderModule other = collision.otherCollider.GetComponent<ColliderModule>();
            if (other != null)
            {
                GetPart(out Part otherPart);

                if (rocket.collisionImmunity > Time.time || part.mass.Value > otherPart.mass.Value * 5)
                    return;
            }

            
            // List<Joint<Part>> joints = rocket.jointsGroup.joints;
            // bool split = false;
            // if (joints.Count > 1)
            //     rocket.jointsGroup.DestroyJoint(joints[Random.Range(0, joints.Count)], out split, out Rocket newRocket);
            //
            // if (split)
            //     return;
            
            
            DestructionReason reason = DestructionReason.RocketCollision;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Celestial Body"))
            {
                reason = DestructionReason.TerrainCollision;
            }
            
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
        float MaxImpactTolerance
        {
            get
            {
                switch (impactTolerance)
                {
                    case ImpactTolerance.Low: return 2.5f;
                    case ImpactTolerance.Medium: return 5.5f;
                    case ImpactTolerance.High: return 12.5f;
                    case ImpactTolerance.Wheel: return 50.5f;
                    default: throw new Exception();
                }
            }
        }
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
                    heatManager.HeatPart(heatModule);
                    
                    // if (collider2d.attachedRigidbody != other.attachedRigidbody)
                    //     other.Distance()
                }
        }
    }
}