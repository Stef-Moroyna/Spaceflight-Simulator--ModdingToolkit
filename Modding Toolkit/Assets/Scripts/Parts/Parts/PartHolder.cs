using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Parts
{
    public class PartHolder : MonoBehaviour
    {
        public List<Part> parts = new List<Part>();

        public event Action<Part[]> onPartsAdded;
        public event Action<Part[]> onPartsRemoved;
        public event Action onPartsChanged;


        // Parts
        public void AddParts(params Part[] parts)
        {
            this.parts.AddRange(parts);
            ResetModules();
            onPartsAdded?.Invoke(parts);
            onPartsChanged?.Invoke();
        }
        public void RemoveParts(params Part[] parts)
        {
            this.parts.RemoveRange(parts);
            ResetModules();
            onPartsRemoved?.Invoke(parts);
            onPartsChanged?.Invoke();
        }
        public void SetParts(params Part[] newParts)
        {
            Part[] oldParts = parts.ToArray();
            parts = new List<Part>(newParts);
            ResetModules();
            onPartsRemoved?.Invoke(oldParts);
            onPartsAdded?.Invoke(newParts);
            onPartsChanged?.Invoke();
        }
        public void ClearParts()
        {
            Part[] oldParts = parts.ToArray();
            parts.Clear();
            ResetModules();
            onPartsRemoved?.Invoke(oldParts);
            onPartsChanged?.Invoke();
        }

        // Has
        public int IndexOf(Part part)
        {
            return parts.IndexOf(part);
        }
        public bool ContainsPart(Part part)
        {
            return parts.Contains(part);
        }
        
        
        // Modules
        [ShowInInspector] Dictionary<string, object> modules = new Dictionary<string, object>();
        [ShowInInspector] Dictionary<string, int> moduleCount = new Dictionary<string, int>();
        //
        public T[] GetModules<T>()
        {
            string typeName = typeof(T).Name;

            if (!modules.ContainsKey(typeName))
                modules.Add(typeName, CollectModules<T>());

            return (T[])modules[typeName];
        }
        public int GetModuleCount<T>()
        {
            string typeName = typeof(T).Name;

            if (!moduleCount.ContainsKey(typeName))
                moduleCount.Add(typeName, CollectModuleCount<T>());

            return moduleCount[typeName];
        }
        public bool HasModule<T>()
        {
            string typeName = typeof(T).Name;

            if (!moduleCount.ContainsKey(typeName))
                moduleCount.Add(typeName, CollectModuleCount<T>());

            return moduleCount[typeName] > 0;
        }
        //
        T[] CollectModules<T>()
        {
            List<T> output = new List<T>();

            foreach (Part part in parts)
                output.AddRange(part.GetModules<T>());

            return output.ToArray();
        }
        int CollectModuleCount<T>()
        {
            return parts.Sum(part => part.GetModuleCount<T>());
        }
        //
        void ResetModules()
        {
            modules.Clear();
            moduleCount.Clear();
        }
        
        
        // Tracking
        public void TrackParts(Action<Part> onPartAdded, Action<Part> onPartRemoved, Action onPartRemoved_After)
        {
            foreach (Part a in parts)
                onPartAdded.Invoke(a);

            onPartsAdded += addedParts =>
            {
                foreach (Part a in addedParts)
                    onPartAdded.Invoke(a);
            };
            
            onPartsRemoved += removedParts =>
            {
                foreach (Part a in removedParts)
                    onPartRemoved.Invoke(a);
                
                onPartRemoved_After.Invoke();
            };
        }
        public void TrackModules<T>(Action<T> onModuleAdded, Action<T> onModuleRemoved, Action onModulesRemoved_After)
        {
            foreach (T a in GetModules<T>())
                onModuleAdded.Invoke(a);

            onPartsAdded += addedParts =>
            {
                foreach (T a in Part_Utility.GetModules<T>(addedParts))
                    onModuleAdded.Invoke(a);
            };
            
            onPartsRemoved += removedParts =>
            {
                foreach (T a in Part_Utility.GetModules<T>(removedParts))
                    onModuleRemoved.Invoke(a);
                
                onModulesRemoved_After.Invoke();
            };
        }
    }
}