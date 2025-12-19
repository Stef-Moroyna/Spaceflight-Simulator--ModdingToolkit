using System;
using System.Linq;
using SFS;
using SFS.Parts;
using SFS.Parts.Modules;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Parts
{
    public class PartVisualizer : MonoBehaviour
    {
        public bool drag, attachment, clickCollider, buildCollider, physicsCollider, magnets, adaptTriggers, centerOfMass, thrust;

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Fix
            GetComponentsInChildren<SurfaceData>().ForEach(x => x.Output());
            Handles.DrawLine(Vector3.zero, Vector3.zero);
            
            // Lines
            Handles.color = Color.yellow;
            if (buildCollider)
                GetComponentsInChildren<PolygonData>().Where(x => x.BuildCollider).ForEach(a => DrawPolygon(a, false, false));

            Handles.color = Color.red;
            if (clickCollider)
                GetComponentsInChildren<PolygonData>().Where(x => x.Click).ForEach(a => DrawPolygon(a, false, true));

            Handles.color = Color.blue;
            if (drag)
                GetComponentsInChildren<SurfaceData>().Where(x => x.Drag).ForEach(x => DrawSurface(x, true));
            
            Handles.color = Color.green;
            if (attachment)
                GetComponentsInChildren<SurfaceData>().Where(x => x.Attachment).ForEach(x => DrawSurface(x, false));
            
            Handles.color = Color.cyan;
            if (physicsCollider)
            {
                GetComponentsInChildren<SurfaceCollider>().ForEach(x => DrawSurface(x.surfaces, true));
                GetComponentsInChildren<PolygonCollider>().Where(x => x.polygon != null).ForEach(x => DrawPolygon(x.polygon, true, false));
                
                GetComponentsInChildren<PolygonData>().Where(x => x.PhysicsCollider_IncludeInactive && x.isActiveAndEnabled).ForEach(x => DrawPolygon(x, true, false));
            }

            // Points
            if (magnets)
                GetComponentsInChildren<MagnetModule>().ForEach(magnet =>
                    magnet.GetSnapPointsWorld().ForEach(point => GLDrawer.DrawCircle(point, 0.1f, 16, Color.blue, 0)));

            if (adaptTriggers)
                GetComponentsInChildren<AdaptTriggerModule>().ForEach(module =>
                {
                    foreach (AdaptTriggerPoint point in module.points)
                        GLDrawer.DrawCircle(module.transform.TransformPoint(point.position.Value), 0.1f, 16, Color.magenta, 0);
                });
            
            if (centerOfMass)
            {
                GetComponentsInChildren<Part>().Select(part => part.transform.TransformPoint(part.centerOfMass.Value))
                    .ForEach(com => GLDrawer.DrawCircle(com, 0.1f, 16, Color.yellow, 0));

                //GLDrawer.DrawCircle(GetComponentsInChildren<Part>().Select(x => x.transform.TransformPoint(x.centerOfMass.Value) * x.mass.Value).Aggregate((x, y) => x + y)
                //                    / GetComponentsInChildren<Part>().Select(x => x.mass.Value).Aggregate((x, y) => x + y), 0.5f, 16, Color.yellow, 0);
            }

            if (thrust)
            {
                GetComponentsInChildren<EngineModule>().ForEach(x =>
                    Utility.DrawArrow_Ray(x.transform.TransformPoint(x.thrustPosition.Value),
                        x.transform.TransformDirection(x.thrustNormal.Value) * 2, Color.magenta));
                
                GetComponentsInChildren<RcsModule>().ForEach(x => x.thrusters.ForEach(thruster =>
                        Utility.DrawArrow_Ray(x.transform.TransformPoint(x.thrustPosition),
                            x.transform.TransformDirection(thruster.thrustNormal) * -0.5f, Color.magenta)));
            }

            void DrawPolygon(PolygonData polygon, bool reducedResolution, bool drawPipe)
            {
                if (drawPipe && polygon is PipeData x)
                    DrawPipe(x, reducedResolution);
                else
                    DrawSurface(polygon, reducedResolution);
            }
            void DrawSurface(SurfaceData polygon, bool reducedResolution)
            {
                (reducedResolution? polygon.surfacesFast : polygon.surfaces).ForEach(y => y.GetSurfacesWorld().ForEach(z => Handles.DrawLine(z.start, z.end)));
            }
            void DrawPipe(PipeData pipe, bool reducedResolution)
            {
                Color c = Handles.color;
                Handles.color = new Color(1, 1, 1, 0.5f);

                float depthM = -pipe.depthMultiplier * (Math.Abs(pipe.transform.localEulerAngles.y - 180) < 0.1f ? -1 : 1);
                
                pipe.pipe.points.ForEach(a => Handles.DrawDottedLine(pipe.transform.TransformPoint(a.GetPosition(-1)), pipe.transform.TransformPoint(a.GetPosition(1)), 4));

                for (int i = 0; i < pipe.pipe.points.Count - 1; i++)
                {
                    PipePoint a = pipe.pipe.points[i];
                    PipePoint b = pipe.pipe.points[i + 1];
                    Handles.DrawDottedLine(pipe.transform.TransformPoint(a.position.ToVector3(a.width.magnitude / 2 * depthM)), pipe.transform.TransformPoint(b.position.ToVector3(b.width.magnitude / 2 * depthM)), 5);
                }

                Handles.color = c;
                
                DrawSurface(pipe, reducedResolution);
            }
        }

        [PropertySpace]
        [Button(ButtonSizes.Medium)]
        void ApplyPrefabChanges()
        {
            for (int i = 0; i < transform.childCount; i++)
                PrefabUtility.ApplyPrefabInstance(transform.GetChild(i).gameObject, InteractionMode.UserAction);
        }
        
        [PropertySpace]
        [Button]
        void RoundPosition()
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).position = Math_Utility.Round(transform.GetChild(i).position, 0.5f);
        }
        [Button]
        void DisableEdit()
        {
            GetComponentsInChildren<CustomPolygon>().ForEach(a => a.edit = a.view = false);
            GetComponentsInChildren<CustomSurfaces>().ForEach(a => a.edit = a.view = false);
            GetComponentsInChildren<CustomPipe>().ForEach(a => a.edit = a.view = false);
        }

        [PropertySpace]
        [Button, HorizontalGroup]
        void ToInteriorView() => GetComponentsInChildren<InteriorModule>(true).ForEach(a => a.gameObject.SetActive(a.layerType == InteriorModule.LayerType.Interior));
        [PropertySpace]
        [Button, HorizontalGroup]
        void ToExteriorView() => GetComponentsInChildren<InteriorModule>(true).ForEach(a => a.gameObject.SetActive(a.layerType == InteriorModule.LayerType.Exterior));
        [Button]
        void ResetViewMode() => GetComponentsInChildren<InteriorModule>(true).ForEach(a => a.gameObject.SetActive(true));


        [Space]
        public bool loadBuild;
        void Start()
        {
            if (loadBuild)
                Base.sceneLoader.LoadBuildScene(false);
        }
        #endif
    }
}

/*[Button]
void SetMaterial(Material material)
{
    GetComponentsInChildren<MeshRenderer>(true).ForEach(a => a.material = material);
}

[Button]
void GetMeshReferences()
{
    GetComponentsInChildren<ModelSetup>(true).ForEach(a => a.GetRenderers());
}

[Button]
void SetMeshes()
{
    GetComponentsInChildren<ModelSetup>().ForEach(a => a.SetMesh());
}*/