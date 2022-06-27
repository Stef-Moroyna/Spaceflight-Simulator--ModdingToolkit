#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace SFS.Parts.Modules
{
    [CustomEditor(typeof(SliderModule))]
    public class SliderEditor : OdinEditor
    {
        SliderModule sliderModule;

        void OnSceneGUI()
        {
            if (!Application.isPlaying)
                return;

            sliderModule = target as SliderModule;

            if (sliderModule == null)
                return;
            
            Vector2 arrowPosition = new Vector2(0, sliderModule.sliderPosition.Value);

            float new_Y = MyHandles.DrawHandle(sliderModule.transform, arrowPosition, Color.green).y;

            sliderModule.sliderPosition.Value += new_Y;
        }
    }
}
#endif