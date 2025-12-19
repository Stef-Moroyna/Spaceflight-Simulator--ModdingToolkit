using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Parts
{
    public class PartHolder : MonoBehaviour
    {
        Part[] cachedArray;
        public Part[] GetArray()
        {
            if (cachedArray == null)
                cachedArray = parts.ToArray();
            
            return cachedArray;
        }
        
        public List<Part> parts = new List<Part>();
        public HashSet<Part> partsSet = new HashSet<Part>();

        public Action<Part[]> onPartsAdded;
        public Action<Part[]> onPartsRemoved;
        public Action onPartsChanged;

        // Parts
        public void AddParts(params Part[] parts)
        {
            this.parts.AddRange(parts);
            parts.ForEach(part => partsSet.Add(part));
            cachedArray = null;
            ResetModules();
            onPartsAdded?.Invoke(parts);
            onPartsChanged?.Invoke();
        }
        public void AddPartAtIndex(int index, Part part)
        {
            parts.Insert(index, part);
            partsSet.Add(part);
            cachedArray = null;
            ResetModules();
            onPartsAdded?.Invoke(new Part[] { part });
            onPartsChanged?.Invoke();
        }
        public void RemoveParts(params Part[] parts)
        {
            this.parts.RemoveRange(parts);
            foreach (Part part in parts)
                partsSet.Remove(part);
            cachedArray = null;

            ResetModules();
            onPartsRemoved?.Invoke(parts);
            onPartsChanged?.Invoke();
        }
        public void RemovePartAtIndex(int index)
        {
            Part part = parts[index];
            parts.RemoveAt(index);
            partsSet.Remove(part);
            cachedArray = null;
            ResetModules();
            onPartsRemoved?.Invoke(new Part[] { part });
            onPartsChanged?.Invoke();
        }
        public void SetParts(params Part[] newParts)
        {
            Part[] oldParts = GetArray();
            parts = new List<Part>(newParts);
            partsSet = new HashSet<Part>(newParts);
            cachedArray = null;
            ResetModules();
            onPartsRemoved?.Invoke(oldParts);
            onPartsAdded?.Invoke(newParts);
            onPartsChanged?.Invoke();
        }
        public void ClearParts()
        {
            Part[] oldParts = GetArray();
            parts.Clear();
            partsSet.Clear();
            cachedArray = null;
            ResetModules();
            onPartsRemoved?.Invoke(oldParts);
            onPartsChanged?.Invoke();
        }

        // Has
        public bool ContainsPart(Part part)
        {
            return partsSet.Contains(part);
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
            return GetModuleCount<T>() > 0;
        }
        //
        T[] CollectModules<T>()
        {
            List<T> output = new List<T>();

            foreach (Part part in parts)
            {
                IEnumerable<T> collection = part.GetModules<T>();
                
                if (collection.Any())
                    output.AddRange(collection);
            }

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