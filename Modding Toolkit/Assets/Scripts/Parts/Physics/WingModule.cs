using System.Collections.Generic;
using System.Linq;
using SFS.Variables;
using SFS.World;
using SFS.World.Drag;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class WingModule : MonoBehaviour, I_InitializePartModule
    {
        public SurfaceData wingSurface;
        public AnimationCurve liftAtSpeed;
        public AnimationCurve liftAtAngleOfAttack;
        public Composed_Vector2 flightDirection;
        public Composed_Vector2 centerOfLift;
        public Composed_Float areaMultiplier;
        public MoveModule elevator;
        
        public float TurnAxis { set => turnAxis.Value = value; }
        readonly Float_Local turnAxis = new Float_Local();

        public int Priority => -1;
        public void Initialize()
        {
            turnAxis.OnChange += RecalculateElevatorDeflection;
        }

        void RecalculateElevatorDeflection()
        {
        }

        float? _wingArea;
        // Only recalculate _wingArea if it's dirty
        public float WingArea => (_wingArea = _wingArea ?? CalculateWingArea()) ?? 0.0f;

        public void RecalculateWingArea() => _wingArea = null;
        
        float CalculateWingArea()
        {
            List<Vector2> points = wingSurface.surfaces.SelectMany(x => x.points).OrderBy(x => Vector2.Dot(x, flightDirection.Value)).ToList();
            return (points.Last() - points.First()).magnitude;
        }
    }
}