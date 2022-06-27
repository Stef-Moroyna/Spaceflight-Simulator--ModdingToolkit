using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourcesLoader : MonoBehaviour
{
    public static ResourcesLoader main;

    void Awake()
    {
        main = this;
    }

    // Utility
    public static Dictionary<string, T> GetFiles_Dictionary<T>(string path) where T : Object
    {
        return GetFiles_Array<T>(path).ToDictionary(x => x.name, x => x);
    }
    public static T[] GetFiles_Array<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}