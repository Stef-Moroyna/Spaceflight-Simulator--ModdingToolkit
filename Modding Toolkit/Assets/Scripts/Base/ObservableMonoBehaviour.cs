using System;
using SFS.Variables;
using UnityEngine;

public class ObservableMonoBehaviour : MonoBehaviour, I_ObservableMonoBehaviour
{
    Action onDestroyDelegate;
    Action I_ObservableMonoBehaviour.OnDestroy
    {
        get => onDestroyDelegate;
        set => onDestroyDelegate = value;
    }
    protected void OnDestroy()
    {
        onDestroyDelegate?.Invoke();
    }
}