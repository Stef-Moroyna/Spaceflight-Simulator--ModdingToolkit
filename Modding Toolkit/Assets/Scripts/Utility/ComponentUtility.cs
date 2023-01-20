using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Component_Utility
{
    public static T GetOrAddComponent<T>(this Component a) where T : Component
    {
        T component = a.GetComponent<T>();

        if (component == null)
            component = a.gameObject.AddComponent<T>();

        return component;
    }
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();

        if (component == null)
            component = obj.AddComponent<T>();

        return component;
    }
    public static List<T> GetOrAddComponents<T>(this Component a, int count) where T : Component
    {
        List<T> components = new List<T>(a.GetComponents<T>());

        while (components.Count < count)
            components.Add(a.gameObject.AddComponent<T>());

        return components;
    }

    public static T GetComponentInParentTree<T>(this Component a)
    {
        return a.transform.GetComponentInParentTree<T>();
    }
    public static T GetComponentInParentTree<T>(this Transform a)
    {
        while (a != null)
        {
            T component = a.GetComponent<T>();

            if (component != null)
                return component;
            
            a = a.parent;
        }

        return default;
    }

    public static T FindByName<T>(this Component component, string name) where T : Component
    {
        T output = component.GetComponentsInChildren<T>(true).ToList().Find(c => c.name == name);
        
        if (output != null)
            return output;
        
        throw new Exception("Cannot find by name: " + name);
    }
    
    public static bool HasComponent<T>(this GameObject obj) where T:Component
    {
        return obj.GetComponent<T>() != null;
    }
    public static bool HasComponent<T>(this GameObject obj, out T component) where T:Component
    {
        component = obj.GetComponent<T>();
        return component != null;
    }
    
    public static RectTransform GetRect(this Component component)
    {
        return component.transform.GetRect();
    }
    public static RectTransform GetRect(this Transform transform)
    {
        return (RectTransform)transform;
    }
}
