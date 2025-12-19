using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class MagnetModule : MonoBehaviour
    {
        const float Range = 1f;
        public Point[] points;

        public Vector2[] GetSnapPointsWorld()
        {
            return points.Select(p => (Vector2)transform.TransformPoint(p.position.Value)).ToArray();
        }
        public static List<Vector2> GetAllSnapOffsets(MagnetModule[] A, MagnetModule[] B, float snapDistance)
        {
            List<Vector2> offsets = new List<Vector2>();

            // List for storing all points in Build Grid
            Dictionary<Vector2Int, List<(Vector2, Point)>> snapPointsBuildDictionary = GetDictionary(B, Range);

            foreach (MagnetModule magnetModule in A)
                foreach (Vector2 point in magnetModule.GetSnapPointsWorld())
                foreach (Vector2Int key in GetDictionaryKeys(point, Range))
                    if (snapPointsBuildDictionary.TryGetValue(key, out List<(Vector2, Point)> matches))
                        foreach ((Vector2, Point) match in matches)
                            if (!match.Item2.occupied)
                            {
                                Vector2 offset = match.Item1 - point;
                                
                                if (offset.sqrMagnitude <= snapDistance * snapDistance)
                                    offsets.Add(offset);
                            }

            return offsets;
        }
        public static void UpdateOccupied(Part[] parts)
        {
            MagnetModule[] modules = Part_Utility.GetModules<MagnetModule>(parts).ToArray();
            Dictionary<Vector2Int, List<(Vector2, Point)>> dictionary = GetDictionary(modules, Range);
            
            foreach (MagnetModule module in modules)
            foreach (Point point in module.points)
            {
                point.occupied = GetOccupied();

                bool GetOccupied()
                {
                    Vector2 position = module.transform.TransformPoint(point.position.Value);
                    
                    foreach (Vector2Int key in GetDictionaryKeys(position, Range))
                        if (dictionary.TryGetValue(key, out List<(Vector2, Point)> matches))
                            foreach ((Vector2, Point) match in matches)
                                if ((match.Item1 - position).sqrMagnitude < 0.05f * 0.05f && point != match.Item2) // In range and not itself
                                    return true;

                    return false;
                }
            }
        }
        
        static Dictionary<Vector2Int, List<(Vector2, Point)>> GetDictionary(MagnetModule[] modules, float range)
        {
            Dictionary<Vector2Int, List<(Vector2, Point)>> output = new Dictionary<Vector2Int, List<(Vector2, Point)>>();
            
            foreach (MagnetModule module in modules)
            foreach (Point point in module.points)
            {
                Vector2 position = module.transform.TransformPoint(point.position.Value);
                
                Vector2Int key = new Vector2Int(Mathf.FloorToInt(position.x / range), Mathf.FloorToInt(position.y / range));
                (Vector2, Point) value = (position, point);
                
                if (output.TryGetValue(key, out List<(Vector2, Point)> list))
                    list.Add(value);
                else
                {
                    list = new List<(Vector2, Point)> { value };
                    output.Add(key, list);
                }
            }
            
            return output;
        }
        public static Vector2Int[] GetDictionaryKeys(Vector2 position, float range)
        {
            int roofX = Mathf.RoundToInt(position.x / range);
            int roofY = Mathf.RoundToInt(position.y / range);
            int floorX = roofX - 1;
            int floorY = roofY - 1;
            return new Vector2Int[] { new Vector2Int(floorX, floorY), new Vector2Int(roofX, floorY), new Vector2Int(floorX, roofY), new Vector2Int(roofX, roofY) };
        }

        [Serializable]
        public class Point
        {
            public Composed_Vector2 position;
            
            // State
            [NonSerialized] public bool occupied;
        }
    }
}