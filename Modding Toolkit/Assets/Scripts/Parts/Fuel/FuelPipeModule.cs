using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFS.World;

namespace SFS.Parts.Modules
{
    public class FuelPipeModule : MonoBehaviour
    {
        // Refs
        public SurfaceData surface_In, surface_Out;
        public DetachModule detach;
        
        // State
        List<FuelPipeModule> previousPipes = new List<FuelPipeModule>();
        ResourceModule resource_In, resource_Out;

        
        // Fetching
        public void FindNeighbours(Rocket rocket)
        {
            // Reset
            previousPipes.Clear();
            resource_In = null;
            resource_Out = null;
            
            // Setup
            Part part = GetComponent<Part>();
            
        }
        
        public (ResourceModule[], ResourceModule)? FindFlow()
        {
            if (resource_Out == null) // Does this pipe outflow
                return null;

            ResourceModule[] fromResources = FindFromTanks().Where(a => a.resourceType == resource_Out.resourceType).ToArray();

            if (fromResources.Length == 0)
                return null;
            
            return (fromResources, resource_Out);
        }
        public List<ResourceModule> FindFlowsForEngine(FlowModule.FlowType flowType)
        {
            if (flowType == FlowModule.FlowType.Negative)
                return FindFromTanks();

            throw new NotImplementedException();
        }
        
        List<ResourceModule> FindFromTanks()
        {
            List<FuelPipeModule> traversed = new List<FuelPipeModule>();

            Stack<FuelPipeModule> leads = new Stack<FuelPipeModule>();
            leads.Push(this);

            List<ResourceModule> output = new List<ResourceModule>();

            while (leads.Count > 0)
            {
                FuelPipeModule a = leads.Pop();
                
                if (a.resource_In != null && !output.Contains(resource_In))
                    output.Add(a.resource_In);

                foreach (FuelPipeModule pipe_In in a.previousPipes)
                    if (!traversed.Contains(pipe_In))
                    {
                        traversed.Add(pipe_In);
                        leads.Push(pipe_In);
                    }
            }

            return output;
        }
        
        
        // Flowing
        public static void FixedUpdate_FuelPipeFlow(List<(ResourceModule[] froms, ResourceModule to)> flows)
        {
            (ResourceModule[] from, ResourceModule to)[] validFlows = flows.Where(f => f.froms.Any(a => a.ResourceAmount > 0) && f.to.ResourceSpace > 0).ToArray();

            // How many are taking from tank / How many are adding into tank
            Dictionary<ResourceModule, int> takingCount = new Dictionary<ResourceModule, int>();
            Dictionary<ResourceModule, int> addingCount = new Dictionary<ResourceModule, int>();
            //
            foreach ((ResourceModule[] froms, ResourceModule to) in validFlows)
            {
                foreach (ResourceModule from in froms)
                {
                    if (!takingCount.ContainsKey(from))
                        takingCount.Add(from, 0);
                    takingCount[from]++;   
                }

                if (!addingCount.ContainsKey(to))
                    addingCount.Add(to, 0);
                addingCount[to]++;
            }
            
            // How much can be added/taken from each tank per source
            Dictionary<ResourceModule, double> allowedToTake = takingCount.ToDictionary(a => a.Key, b => b.Key.ResourceAmount / b.Value);
            Dictionary<ResourceModule, double> allowedToAdd = addingCount.ToDictionary(a => a.Key, b => b.Key.ResourceSpace / b.Value);

            // Transfers
            foreach ((ResourceModule[] froms, ResourceModule to) in validFlows)
            {
                double transferAmount = Math.Min(to.resourceType.transferRate * Time.fixedDeltaTime, Math.Min(froms.Sum(from => allowedToTake[from]), allowedToAdd[to]));

                foreach (ResourceModule from in froms)
                    from.TakeResource(transferAmount / froms.Length);
                
                to.AddResource(transferAmount);
            }
        }
        
        
        // Separation
        public void DetachPipe(UsePartData data)
        {
            foreach (FuelPipeModule lastPipe in FindLastPipes())
                lastPipe.detach.Detach(data);
        }
        List<FuelPipeModule> FindLastPipes()
        {
            throw new NotImplementedException();

            // List<FuelPipeModule> traversed = new List<FuelPipeModule>();
            //
            // Stack<FuelPipeModule> leads = new Stack<FuelPipeModule>();
            // leads.Push(this);
            //
            // List<FuelPipeModule> output = new List<FuelPipeModule>();

            
            // while (leads.Count > 0)
            // {
            //     FuelPipeModule a = leads.Pop();
            //
            //     if (a.nextPipes.Count == 0)
            //     {
            //         output.Add(this);   
            //     }
            //
            //     if (traversed.Contains(pipe.nextPipe))
            //         return null; // Stuck in loop
            //
            //     // Next pipe
            //     traversed.Add(pipe);
            //     pipe = pipe.nextPipe;
            // }
        }
    }
}
