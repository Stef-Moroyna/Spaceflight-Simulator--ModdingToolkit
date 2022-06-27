using System;
using UnityEngine;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    public class BurnMark : MonoBehaviour
    {
        // Const
        const float margin = 0.3f;
        const float textureWidth = 7f;
        static readonly int Opacity = Shader.PropertyToID("_Opacity");
        static readonly int OffsetTexture = Shader.PropertyToID("_Offset");


        // State
        public Burn burn;
        MeshReference[] meshReferences;
        Texture2D offsetTexture;
        [ShowInInspector, ReadOnly] float opacity = 1;
        
        
        // Initialization
        public void Initialize()
        {
            meshReferences = GetComponentsInChildren<MeshRenderer>(true).Select(a => new MeshReference(a, a.GetComponent<MeshFilter>())).ToArray();
        }


        [Button]
        void DrawSurfaces()
        {
            Matrix2x2 r = Matrix2x2.Angle(-(-burn.angle + 90) * Mathf.Deg2Rad);
            
            foreach (Line2 topSurface in burn.topSurfaces)
                Debug.DrawLine(transform.TransformPoint(topSurface.start * r), transform.TransformPoint(topSurface.end * r), Color.red);
            foreach (Line2 topSurface in burn.bottomSurfaces)
                Debug.DrawLine(transform.TransformPoint(topSurface.start * r), transform.TransformPoint(topSurface.end * r), Color.yellow);
        }

        // Set burn data
        public void SetBurn(Vector2 velocityDirection, Transform positionContext, float intensity, Line2[] topSurfaces_World, Line2[] bottomSurfaces_World, float opacity)
        {
            if (meshReferences == null)
                Initialize();
            

            // Flips bottom surfaces
            for (int i = 0; i < bottomSurfaces_World.Length; i++)
            {
                Line2 line = bottomSurfaces_World[i];
                (line.start, line.end) = (line.end, line.start);
                bottomSurfaces_World[i] = line;
            }
            bottomSurfaces_World = bottomSurfaces_World.Reverse().ToArray();
            
            // Cut bounds
            float velocityAngleRad = Mathf.Atan2(velocityDirection.y, velocityDirection.x) - Mathf.PI * 0.5f;
            Matrix2x2 toVelocityDirectionLocal = Matrix2x2.Angle(-velocityAngleRad);
            (float xMin, float xMax) = GetCutBounds(toVelocityDirectionLocal);

            // Data
            Vector2 directionLocal = transform.InverseTransformVector(velocityDirection);
            float angleLocal = Mathf.Atan2(directionLocal.y, directionLocal.x) * Mathf.Rad2Deg;
            float rotateAngle = (-angleLocal + 90) * Mathf.Deg2Rad;
            Matrix2x2 rotate = Matrix2x2.Angle(rotateAngle);

            // Cuts surfaces
            Line2[] topSurfaces = CutSurfaces(xMin, xMax, toVelocityDirectionLocal, rotate, topSurfaces_World); 
            Line2[] bottomSurfaces = CutSurfaces(xMin, xMax, toVelocityDirectionLocal, rotate, bottomSurfaces_World);

            
            // Coordinate for texture
            Vector2 directionContext = positionContext.InverseTransformVector(velocityDirection);
            float angleContext = Mathf.Atan2(directionContext.y, directionContext.x) * Mathf.Rad2Deg;
            float x = (positionContext.InverseTransformPoint(transform.position) * Matrix2x2.Angle((-angleContext + 90) * Mathf.Deg2Rad)).x;
            
            // Set state
            burn = new Burn { angle = angleLocal, intensity = intensity, x = x, topSurfaces = topSurfaces, bottomSurfaces = bottomSurfaces };
            this.opacity = opacity;
        }
        (float, float) GetCutBounds(Matrix2x2 toVelocityDirectionLocal)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            foreach (MeshReference m in meshReferences)
            {
                MeshFilter mesh = m.filter;
                foreach (Vector3 a in mesh.sharedMesh.vertices)
                {
                    float X = toVelocityDirectionLocal.GetX(mesh.transform.TransformPoint(a));
                    xMin = Mathf.Min(xMin, X);
                    xMax = Mathf.Max(xMax, X);
                }

                // for (int i = 0; i < mesh.sharedMesh.vertices.Length - 1; i++)
                //     Debug.DrawLine(mesh.transform.TransformPoint(mesh.sharedMesh.vertices[i]) * toVelocityDirectionLocal, mesh.transform.TransformPoint(mesh.sharedMesh.vertices[i + 1]) * toVelocityDirectionLocal, Color.red);
                //
                // Debug.DrawLine(mesh.transform.TransformPoint(mesh.sharedMesh.vertices.First()) * toVelocityDirectionLocal, mesh.transform.TransformPoint(mesh.sharedMesh.vertices.Last()) * toVelocityDirectionLocal, Color.red);
            }
            
            return (xMin, xMax);
        }
        Line2[] CutSurfaces(float xMin, float xMax, Matrix2x2 toVelocityDirectionLocal, Matrix2x2 rotate, Line2[] surfaces_WorldCoords)
        {
            // foreach (Line2 a in surfaces_WorldCoords)
            //     Debug.DrawLine(a.start * toVelocityDirectionLocal, a.end * toVelocityDirectionLocal);
            //
            // Debug.DrawRay(new Vector3(xMin, -1000), Vector3.up * 2000, Color.magenta);
            // Debug.DrawRay(new Vector3(xMax, -1000), Vector3.up * 2000, Color.magenta);
            
            // Range
            int startIndex = 0;
            while (startIndex < surfaces_WorldCoords.Length && toVelocityDirectionLocal.GetX(surfaces_WorldCoords[startIndex].end) < xMin - margin)
                startIndex++;
            int endIndex = surfaces_WorldCoords.Length - 1;
            while (endIndex > 0 && toVelocityDirectionLocal.GetX(surfaces_WorldCoords[endIndex].start) > xMax + margin)
                endIndex--;

            // Cuts
            int size = endIndex - startIndex + 1;
            Line2[] output = new Line2[size];
            for (int i = 0; i < output.Length; i++)
            {
                // World -> local -> rotated
                Line2 line = surfaces_WorldCoords[startIndex + i];
                output[i] = new Line2(transform.InverseTransformPoint(line.start) * rotate, transform.InverseTransformPoint(line.end) * rotate);
                
                //Debug.DrawLine(line.start * toVelocityDirectionLocal, line.end * toVelocityDirectionLocal, Color.green);
            }
            
            return output;
        }

        
        
        // Apply
        [Button] public void ApplyEverything()
        {
            float angleForExposed = (-burn.angle + 90) * Mathf.Deg2Rad;
            Matrix2x2 rotate = Matrix2x2.Angle(angleForExposed);
            Vector2[][] rotatedVertices = GetRotatedVertices(rotate);
            Bounds bounds = GetBounds(rotatedVertices);
            
            ApplyUV(rotatedVertices, bounds);
            CreateTexture(bounds);
            ApplyTexture();
            ApplyIntensity();
        }
        
        void ApplyUV(Vector2[][] rotatedVertices, Bounds bounds)
        {
            for (int meshIndex = 0; meshIndex < meshReferences.Length; meshIndex++)
            {
                MeshReference meshReference = meshReferences[meshIndex];
                Vector2[] vertices = rotatedVertices[meshIndex];
                
                Vector3[] uvs = new Vector3[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                    uvs[i] = new Vector3((burn.x + vertices[i].x) / textureWidth, vertices[i].y - bounds.yMin, Mathf.InverseLerp(bounds.xMin, bounds.xMax, vertices[i].x));
                meshReference.filter.sharedMesh.SetUVs(4, uvs);
            }
        }

        void CreateTexture(Bounds bounds)
        {
            Destroy(offsetTexture); // Memory clear
            
            float width = bounds.xMax - bounds.xMin;
            int resolution = (int)(width * 6);
            offsetTexture = new Texture2D(resolution, 1) { wrapMode = TextureWrapMode.Clamp };
            float step = width / resolution;

            // Create texture
            for (int x = 0; x < resolution; x++)
            {
                float X = bounds.xMin + step * (0.5f + x);

                float height_Top = GetHeightAtX(burn.topSurfaces);
                float verticalOffset = height_Top - bounds.yMin;
                
                float height_Bottom = GetHeightAtX(burn.bottomSurfaces);
                float size = Mathf.Max(height_Top - height_Bottom, 0.7f);

                offsetTexture.SetPixel(x, 0, new Color(verticalOffset / 10 * 0.5f + 0.5f, size / 10, 0, 0));

                float GetHeightAtX(Line2[] surfaces)
                {
                    foreach (Line2 line in surfaces)
                        if (X >= line.start.x && X <= line.end.x)
                            return line.GetHeightAtX_Unclamped(X);

                    if (surfaces.Length == 0)
                        return float.MinValue;

                    Line2 best = surfaces[0];
                    foreach (Line2 line in surfaces)
                        if (Mathf.Abs(line.LerpUnclamped(0.5f).x - X) < Mathf.Abs(best.LerpUnclamped(0.5f).x - X))
                            best = line;

                    return best.GetHeightAtX(X);
                }
            }
            offsetTexture.Apply();
        }
        [Button] void ApplyTexture()
        {
            SetPropertyBlock(a => a.SetTexture(OffsetTexture, offsetTexture));
        }

        public void SetOpacity(float opacity, bool forceApply)
        {
            if (opacity == this.opacity && !forceApply)
                return;
            
            this.opacity = opacity;
            ApplyIntensity();
        }
        [Button] void ApplyIntensity()
        {
            float newIntensity = burn.intensity * opacity;
            SetPropertyBlock(a => a.SetFloat(Opacity, newIntensity * 1.2f));
        }

        void SetPropertyBlock(Action<MaterialPropertyBlock> setData)
        {
            foreach (MeshReference meshReference in meshReferences)
                for (int i = 0; i < meshReference.renderer.materials.Length; i++)
                {
                    MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                    if (meshReference.renderer.HasPropertyBlock())
                        meshReference.renderer.GetPropertyBlock(propertyBlock, i);
                
                    setData.Invoke(propertyBlock);
                    meshReference.renderer.SetPropertyBlock(propertyBlock, i);
                }
        }
        

        // Utility
        Vector2[][] GetRotatedVertices(Matrix2x2 rotate)
        {
            Vector2[][] meshVertices = new Vector2[meshReferences.Length][];
            for (int meshIndex = 0; meshIndex < meshReferences.Length; meshIndex++)
            {
                MeshFilter mesh = meshReferences[meshIndex].filter;
                Vector3[] input = mesh.sharedMesh.vertices;
                Vector2[] output = new Vector2[input.Length];

                for (int i = 0; i < input.Length; i++)
                    output[i] = transform.InverseTransformPoint(mesh.transform.TransformPoint(input[i])) * rotate;

                meshVertices[meshIndex] = output;
            }

            return meshVertices;
        }
        static Bounds GetBounds(Vector2[][] vertices)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;
            
            foreach (Vector2[] vertexes in vertices)
            foreach (Vector2 vertex in vertexes)
            {
                xMin = Mathf.Min(xMin, vertex.x);
                xMax = Mathf.Max(xMax, vertex.x);
                yMin = Mathf.Min(yMin, vertex.y);
                yMax = Mathf.Max(yMax, vertex.y);
            }
            
            return new Bounds { xMin = xMin, xMax = xMax, yMin = yMin, yMax = yMax };
        }


        void OnDestroy()
        {
            Destroy(offsetTexture);
        }


        public float GetAngleRadWorld()
        {
            Vector2 direction_Local = new Vector2(Mathf.Cos(burn.angle), Mathf.Sin(burn.angle));
            Vector2 direction_World = transform.TransformVector(direction_Local);
            return Mathf.Atan2(direction_World.y, direction_World.x);
        }
        

        [Serializable]
        public class Burn
        {
            public float angle;
            public float intensity;
            public float x;
            public Line2[] topSurfaces, bottomSurfaces;

            public Burn GetCopy()
            {
                return new Burn { angle = angle, intensity = intensity, x = x, topSurfaces = topSurfaces, bottomSurfaces = bottomSurfaces };
            }
        }
        [Serializable]
        public class BurnSave
        {
            public float angle;
            public float intensity;
            public float x;
            public string top, bottom;

            public BurnSave() {}

            public BurnSave(Burn burn)
            {
                angle = burn.angle;
                intensity = burn.intensity;
                x = burn.x;

                top = ToSurfaceSave(burn.topSurfaces);
                bottom = ToSurfaceSave(burn.bottomSurfaces);
            }
            string ToSurfaceSave(Line2[] surfaces)
            {
                StringBuilder builder = new StringBuilder(surfaces.Length * 4 * 5);
                
                foreach (Line2 surface in surfaces)
                {
                    builder.Append(Mathf.RoundToInt(surface.start.x * 100) + ",");
                    builder.Append(Mathf.RoundToInt(surface.start.y * 100) + ",");
                    builder.Append(Mathf.RoundToInt(surface.end.x * 100) + ",");
                    builder.Append(Mathf.RoundToInt(surface.end.y * 100) + ",");
                }
                
                return builder.ToString();
            }

            public Burn FromSave()
            {
                return new Burn { angle = angle, intensity = intensity, x = x, topSurfaces = FromSurfaceSave(top), bottomSurfaces = FromSurfaceSave(bottom) };
            }
            Line2[] FromSurfaceSave(string text)
            {
                string[] texts = text.Split(',');
                int count = texts.Length - 1;
                
                int[] values = new int[count];
                for (int i = 0; i < count; i++)
                    values[i] = int.Parse(texts[i]);

                Line2[] output = new Line2[texts.Length / 4];
                for (int i = 0; i < output.Length; i++)
                    output[i] = new Line2(new Vector2(values[i * 4] / 100f, values[i * 4 + 1] / 100f), new Vector2(values[i * 4 + 2] / 100f, values[i * 4 + 3] / 100f));
                
                return output;
            }
        }

        class MeshReference
        {
            public MeshRenderer renderer;
            public MeshFilter filter;

            public MeshReference(MeshRenderer renderer, MeshFilter filter)
            {
                this.renderer = renderer;
                this.filter = filter;
            }
        }

        struct Bounds
        {
            public float xMin, xMax, yMin, yMax;
        }
    }
}