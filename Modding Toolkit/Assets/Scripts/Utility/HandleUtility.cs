#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class MyHandles
{
    const float HandleSize = 0.12f;
    
    public static Vector2 DrawHandle(Transform transform, Vector3 position, Color color)
    {
        Handles.color = color;
         
        float size = HandleUtility.GetHandleSize(Vector2.zero) * HandleSize;
        Vector3 pointWorld = transform.TransformPoint(position);
        return transform.InverseTransformVector(Handles.FreeMoveHandle(pointWorld, size, Vector2.zero, Handles.CubeHandleCap) - pointWorld);
    }
    public static bool DrawButton(Transform transform, Vector3 position, float size, Color color)
    {
        Handles.color = color;
        size *= HandleUtility.GetHandleSize(Vector2.zero);

        return Handles.Button(transform.TransformPoint(position), Quaternion.identity, size, size, Handles.ConeHandleCap);
    }
    public static void DrawLine(Transform transform, Vector3 start, Vector3 end)
    {
        Handles.color = Color.blue;
        Handles.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
    }
    
    public static Vector2 DrawHandle_NoTrans(Vector3 position, Color color)
    {
        float size = HandleUtility.GetHandleSize(Vector2.zero) * 0.09f;
        Handles.color = color;
        return Handles.FreeMoveHandle(position, size, Vector2.zero, Handles.CubeHandleCap) - position;
    }
    public static bool DrawButton_NoTrans(Vector3 position, Color color)
    {
        float size = HandleUtility.GetHandleSize(Vector2.zero) * 0.09f;
        Handles.color = color;
        return Handles.Button(position, Quaternion.identity, size, size, Handles.CubeHandleCap);
    }
}
#endif