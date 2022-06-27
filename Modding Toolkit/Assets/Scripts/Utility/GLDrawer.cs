using System.Collections.Generic;
using UnityEngine;

public class GLDrawer : MonoBehaviour
{
    public static GLDrawer main;
    void Awake() => main = this;
    

    public Material material;
    Dictionary<float, Material> sortedMaterials = new Dictionary<float, Material>();

    // Instances
    public List<I_GLDrawer> drawers = new List<I_GLDrawer>();
    public static void Register(I_GLDrawer drawer) => main.drawers.Add(drawer);
    public static void Unregister(I_GLDrawer drawer) => main.drawers.Remove(drawer);

    void OnPostRender()
    {
        foreach (I_GLDrawer drawer in drawers)
            drawer.Draw();
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float width, float sortingOrder = 1f)
    {
        GL.Begin(GL.QUADS);
        
        GetMaterial(sortingOrder).SetPass(0);
        
        GL.Color(color);

        Vector3 perpendicular = Vector2.Perpendicular((start - end).normalized) * width * 0.5f;

        GL.Vertex(start - perpendicular);
        GL.Vertex(end - perpendicular);
        GL.Vertex(end + perpendicular);
        GL.Vertex(start + perpendicular);
        GL.End();
    }
    public static void DrawCircle(Vector3 position, float radius, int resolution, Color color, float sortingOrder = 1f)
    {
        if (resolution < 3)
            return;

        GL.Begin(GL.QUADS);

        GetMaterial(sortingOrder)?.SetPass(0);

        GL.Color(color);
        
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
            points[i] = new Vector3(position.x + radius * Mathf.Cos(2 * Mathf.PI * i / resolution), position.y + radius * Mathf.Sin(2 * Mathf.PI * i / resolution), position.z);

        for (int i = 0; i < points.Length; i++)
        {
            GL.Vertex(position);
            GL.Vertex(points[(i + 1) % points.Length]);
            GL.Vertex(points[i]);
            GL.Vertex(position);
        }

        GL.End();
    }

    static Material GetMaterial(float sortingOrder)
    {
        if (!main)
            return null;
        
        if (!main.sortedMaterials.ContainsKey(sortingOrder))
        {
            Material mat = new Material(main.material);
            mat.SetFloat("_Depth", sortingOrder);
            main.sortedMaterials.Add(sortingOrder, mat);
            return mat;
        }

        return main.sortedMaterials[sortingOrder];
    }
}

public interface I_GLDrawer { void Draw(); }