using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class OnFrameEnd : MonoBehaviour
{
    public static OnFrameEnd main;
    void Awake() => main = this;

   
    public event Action onBeforeFrameEnd_Once;
    public event Action onBeforeFrameEnd_Permanent;

    void Start()
    {
    }

    void LateUpdate()
    {
        onBeforeFrameEnd_Once?.Invoke();
        onBeforeFrameEnd_Once = null;
        
        onBeforeFrameEnd_Permanent?.Invoke();
    }
}