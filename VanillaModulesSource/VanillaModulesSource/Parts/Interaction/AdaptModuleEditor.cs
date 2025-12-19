#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(AdaptModule))]
    public class AdaptModuleEditor : OdinEditor
    {
        AdaptModule adaptModule;

        void OnSceneGUI()
        {
            adaptModule = target as AdaptModule;

            foreach (AdaptModule.Point point in adaptModule.adaptPoints)
            {
                Handles.color = Color.green;

                // Input point
                Handles.DrawSolidDisc(adaptModule.transform.TransformPoint(point.position.Value), Vector3.forward, 0.1f);

                // Input area
                if (point.reciverType == AdaptModule.ReceiverType.Area)
                {
                    Rect rect = new Rect(adaptModule.transform.TransformPoint(point.inputArea.Value.position), adaptModule.transform.TransformVector(point.inputArea.Value.size));
                    Handles.color = new Color(0, 1, 1, 0.1f);
                    Handles.DrawAAConvexPolygon(rect.min, new Vector2(rect.min.x, rect.max.y), rect.max, new Vector2(rect.max.x, rect.min.y));
                }
            }
        }
    }
}
#endif