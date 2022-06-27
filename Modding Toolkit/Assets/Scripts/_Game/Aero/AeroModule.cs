using System.Collections.Generic;
using System.Linq;
using SFS.Parts.Modules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS.World.Drag
{
    public abstract class AeroModule : MonoBehaviour
    {
        [Required] public HeatManager heatManager;
        [Required] public BurnManager burnManager;
        [Space]
        [Required] public AeroMesh shockEdge;
        [Required] public AeroMesh shockOuter;
        [Space]
        [Required] public AeroMesh reentryEdge;
        [Required] public AeroMesh reentryOuter;
        [Space]
        public AudioModule airflowSound;
        public AudioModule burnSound;

        
        int frameIndex;
        void FixedUpdate()
        {
            frameIndex++;

            bool drewShockMesh = false;
            bool drewReentryMesh = false;
            
            
            // Heat dissipation
            heatManager.DissipateHeat(frameIndex);
            
            
            // Mesh enable
            if (shockEdge.gameObject.activeSelf != drewShockMesh)
            {
                shockEdge.gameObject.SetActive(drewShockMesh);
                shockOuter.gameObject.SetActive(drewShockMesh);   
            }
            if (reentryEdge.gameObject.activeSelf != drewReentryMesh)
            {                
                reentryEdge.gameObject.SetActive(drewReentryMesh);
                reentryOuter.gameObject.SetActive(drewReentryMesh);
            }
        }
        
        static (float drag, Vector2 centerOfDrag) CalculateDragForce(List<Surface> surfaces)
        {
            float drag = 0;
            Vector2 centerOfDrag = Vector2.zero;

            foreach (Surface surface in surfaces)
            {
                Vector2 size = surface.line.end - surface.line.start;

                if (size.x < 0.01f)
                    continue;

                float dragPerArea = size.x / (size.x + Mathf.Abs(size.y));
                float surfaceDrag = size.x * dragPerArea;

                drag += surfaceDrag;
                centerOfDrag += (surface.line.start + surface.line.end) * surfaceDrag;
            }

            if (drag > 0)
                centerOfDrag /= drag * 2;

            return (drag, centerOfDrag);
        }
        
        // Generally useful
        public static float GetIntensity(float value, float halfPoint)
        {
            return value / (value + halfPoint);
        }
        public static float GetHeatTolerance(HeatTolerance a)
        {
            switch (a)
            {
                case HeatTolerance.High: return 6000;
                case HeatTolerance.Mid: return 800;
                case HeatTolerance.Low: return 300;
                default: return 0;
            }
        }
        
        // Surfaces utility
        public static List<Surface> GetExposedSurfaces(List<Surface> surfaces)
        {
            if (surfaces.Count == 0)
                return new List<Surface>();

            // Uses shell sort to sort surfaces
            SortDragSurfacesByEndX(surfaces);

            List<Surface> output = new List<Surface> {surfaces.First()};
            const float MARGIN = 0.001f;
            for (int i = 1; i < surfaces.Count; i++)
            {
                Surface LINE = surfaces[i];
                int startPopulationIndex = output.Count - 1;

                // New section extends past last || is above
                bool is_New_Line_Above = LINE.line.end.x > output.Last().line.end.x + MARGIN;
                if (is_New_Line_Above)
                    output.Add(new Surface(LINE.owner, new Line2(LINE.line.GetPositionAtX_Unclamped(Mathf.Max(output.Last().line.end.x, LINE.line.start.x)), LINE.line.end)));

                // Populates existing in reverse
                for (int index = startPopulationIndex; index >= 0; index--)
                {
                    Surface SECTION_ORIGINAL = output[index];
                    Surface SECTION = SECTION_ORIGINAL;

                    // Lines do not overlap -> Finish
                    if (LINE.line.start.x > SECTION.line.end.x - MARGIN)
                        break;

                    // End
                    int above_End_Int;
                    {
                        Vector2 end_NewLine = LINE.line.GetPositionAtX_Unclamped(SECTION_ORIGINAL.line.end.x);
                        float diff_End = end_NewLine.y - SECTION_ORIGINAL.line.end.y;
                        above_End_Int = Mathf.Abs(diff_End) > MARGIN ? (int) Mathf.Sign(diff_End) : 0;

                        if (above_End_Int == 1)
                        {
                            if (is_New_Line_Above)
                            {
                                // merge
                                SetSectionEnd(output[index + 1].owner, output[index + 1].line.end); // Extends current end
                                RemoveSection(index + 1); // Removes next
                            }
                            else
                            {
                                // nothing
                                SetSectionEnd(LINE.owner, end_NewLine);
                            }
                        }
                    }


                    // Crossing && Start
                    bool beingAppliedFullyCoversExisting = LINE.line.start.x < SECTION_ORIGINAL.line.start.x + MARGIN;
                    Vector2 start_NewLine = beingAppliedFullyCoversExisting ? LINE.line.GetPositionAtX_Unclamped(SECTION_ORIGINAL.line.start.x) : LINE.line.start;
                    Vector2 start_Section = beingAppliedFullyCoversExisting ? SECTION_ORIGINAL.line.start : SECTION_ORIGINAL.line.GetPositionAtX_Unclamped(LINE.line.start.x);
                    float diff_Start = start_NewLine.y - start_Section.y;
                    int above_Start_Int = Mathf.Abs(diff_Start) > MARGIN ? (int) Mathf.Sign(diff_Start) : 0;

                    // Crossing
                    bool crossing = above_End_Int != above_Start_Int;
                    if (crossing && Line2.FindIntersection_Unclamped(LINE.line, SECTION_ORIGINAL.line, out Vector2 position))
                    {
                        // cuts
                        InsertSection(index + 1, SECTION.owner, position, SECTION.line.end);
                        SetSectionEnd(SECTION.owner, position);
                    }
  

                    // Start
                    if (beingAppliedFullyCoversExisting)
                    {
                        if (above_Start_Int >= 0)
                        {
                            // Above
                            Vector2 start;
                            if (index > 0 && LINE.line.start.x < output[index - 1].line.end.x + MARGIN) // line + margin fills entire gap
                                start = LINE.line.GetPositionAtX_Unclamped(output[index - 1].line.end.x); // Gap fill
                            else if (LINE.line.start.x < SECTION.line.start.x - MARGIN) // line extends more than margin
                                start = LINE.line.start; // extend past (make sure it ends before previous does)
                            else
                                start = LINE.line.GetPositionAtX_Unclamped(SECTION.line.start.x); // else -> keeps section x
                            
                            SetSectionStart(LINE.owner, start);
                        }
                        else
                        {
                            bool hasGap = index == 0 || output[index - 1].line.end.x + MARGIN < SECTION.line.start.x;
                            bool newLineExtendsBeyondSectionStart = hasGap && LINE.line.start.x < SECTION.line.start.x - MARGIN;
                            float previousSectionEnd = index > 0 ? output[index - 1].line.end.x : float.NegativeInfinity;
                            float start_X = newLineExtendsBeyondSectionStart ? Mathf.Max(LINE.line.start.x, previousSectionEnd) : float.NaN;
                            
                            // below
                            if (newLineExtendsBeyondSectionStart)
                            {
                                // Current section itself does not need any changes
                                InsertSection(index, LINE.owner, LINE.line.GetPositionAtX_Unclamped(start_X), LINE.line.GetPositionAtX_Unclamped(SECTION.line.start.x)); // new segment
                                above_Start_Int = 1;
                            }

                            // else --> below and does not extend --> does nothing
                        }
                    }
                    else
                    {
                        // Not full && above
                        if (above_Start_Int >= 0)
                        {
                            // cuts
                            InsertSection(index + 1, SECTION.owner, LINE.line.start, SECTION.line.end); // new line
                            SetSectionEnd(SECTION_ORIGINAL.owner, SECTION_ORIGINAL.line.GetPositionAtX_Unclamped(LINE.line.start.x)); // rest of current section
                        }
                    }

                    is_New_Line_Above = above_Start_Int == 1;

                    // Utility
                    void RemoveSection(int ii)
                    {
                        output.RemoveAt(ii);
                    }

                    void SetSectionStart(I_HeatModule part, Vector2 start)
                    {
                        SECTION.owner = part;
                        SECTION.line.start = start;
                        output[index] = SECTION;
                    }

                    void SetSectionEnd(I_HeatModule part, Vector2 end)
                    {
                        SECTION.owner = part;
                        SECTION.line.end = end;
                        output[index] = SECTION;
                    }
                }

                // Utility
                void InsertSection(int ii, I_HeatModule part, Vector2 start, Vector2 end)
                {
                    output.Insert(ii, new Surface(part, new Line2(start, end)));
                }
            }

            //return output.Where(a => a.line.SizeX > 0.01f).ToList();
            return output;
        }
        static void SortDragSurfacesByEndX(List<Surface> surfaces)
        {
            int n = surfaces.Count;
            int gap = n / 2;

            while (gap > 0)
            {
                for (int i = 0; i + gap < n; i++)
                {
                    int j = i + gap;
                    Surface temp = surfaces[j];

                    while (j - gap >= 0 && temp.line.end.x < surfaces[j - gap].line.end.x)
                    {
                        surfaces[j] = surfaces[j - gap];
                        j -= gap;
                    }

                    surfaces[j] = temp;
                }

                gap /= 2;
            }
        }
        //
        static List<Surface> RemoveSurfaces(List<Surface> surfaces, float maxSlope)
        {
            return surfaces.Where(surface => Mathf.Abs(surface.line.SizeY / surface.line.SizeX) < maxSlope && surface.line.SizeX > 0.1f).ToList();
        }
        static void ApplyProtectionZone(List<Surface> surfaces)
        {
            const float margin = 0.1f;
            const float slope = 0.2f;
            const float max = 0.4f;
            for (int i = 1; i < surfaces.Count - 1; i++)
            {
                Line2 line = surfaces[i].line;

                // Left
                float diff_Left = line.start.y - surfaces[i - 1].line.end.y;
                if (diff_Left > margin)
                {
                    float x = line.start.x - Mathf.Min(diff_Left * slope, max);

                    for (int ii = i - 1; ii >= 0; ii--)
                    {
                        if (surfaces[ii].line.start.x > x)
                        {
                            surfaces.RemoveAt(ii);
                            i--;
                        }
                        else
                        {
                            Surface a = surfaces[ii];
                            a.line.end = a.line.GetPositionAtX(x);
                            surfaces[ii] = a;
                            break;
                        }
                    }
                }

                // Right
                float diff_Right = line.end.y - surfaces[i + 1].line.start.y;
                if (diff_Right > margin)
                {
                    float x = line.end.x + Mathf.Min(diff_Left * slope, max);

                    for (int ii = i + 1; ii < surfaces.Count; ii++)
                    {
                        if (surfaces[ii].line.end.x < x)
                        {
                            surfaces.RemoveAt(ii);
                            ii--;
                        }
                        else
                        {
                            Surface a = surfaces[ii];
                            a.line.start = a.line.GetPositionAtX(x);
                            surfaces[ii] = a;
                            break;
                        }
                    }
                }
            }

            for (int i = surfaces.Count - 1; i >= 0; i--)
                if (surfaces[i].line.SizeX < 0.1f)
                    surfaces.RemoveAt(i);
        }
        //
        public static Line2[] RotateSurfaces(List<Surface> surfaces, Matrix2x2 localToWorld)
        {
            int topSurfacesCount = surfaces.Count;
            Line2[] output = new Line2[topSurfacesCount];
                    
            for (int i = 0; i < surfaces.Count; i++)
            {
                Surface surface = surfaces[i];
                output[i] = new Line2(surface.line.start * localToWorld, surface.line.end * localToWorld);
            }

            return output;
        }
        
        
        // Implementation
        protected abstract bool PhysicsMode { get; }
        protected abstract List<Surface> GetDragSurfaces(Matrix2x2 rotate);
        protected abstract void ApplyParachuteDrag(ref float force, ref Vector2 centerOfDrag_World);
        protected virtual void ApplyWingLift() {}
        protected abstract void AddForceAtPosition(Vector2 force, Vector2 position);
        protected abstract float GetMass();
    }
    
    public struct Surface
    {
        public I_HeatModule owner;
        public Line2 line;

        public Surface(I_HeatModule owner, Line2 line)
        {
            this.owner = owner;
            this.line = line;
        }
    }
    public interface I_HeatModule
    {
        string Name { get; }
        bool IsHeatShield { get; }
        
        float Temperature { get; set; } // NaN when heat is 0
        int LastAppliedIndex { get; set; }
        float ExposedSurface { get; set; }
        
        float HeatTolerance { get; }
        void OnOverheat(bool breakup);
    }
    
    // Use SO in the future
    public enum HeatTolerance
    {
        Low,
        Mid,
        High,
    }
}