using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Translations;
using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class LES_Module : MonoBehaviour, Rocket.INJ_Rocket
    {
        public Bool_Reference autoDetach;
        public void Draw(List<LES_Module> modules, StatsMenu drawer, PartDrawSettings settings)
        {
            if (settings.build || settings.game)
                drawer.DrawToggle(0, () => Loc.main.Auto_Detach_Capsule, Toggle, () => autoDetach.Value, update => autoDetach.OnChange += update, update => autoDetach.OnChange -= update);

            void Toggle()
            {
                Undo.main.RecordStatChangeStep(modules, () =>
                {
                    bool value = !autoDetach.Value;
                    foreach (LES_Module module in modules)
                        module.autoDetach.Value = value;
                });
            }
        }
        
        public Rocket Rocket { get; set; }
        public void Activate(UsePartData usePartData)
        {
            if (!autoDetach.Value)
                return;

            // Turns off engines
            foreach (EngineModule engineModule in Rocket.partHolder.GetModules<EngineModule>())
                engineModule.engineOn.Value = false;
            
            int i = 0;
            while (Detach(usePartData))
                if (i < 1000)
                    i++;
                else
                    throw new Exception("Infinite loop");
        }

        bool Detach(UsePartData usePartData)
        {
            JointGroup group = Rocket.jointsGroup;
            Part root = GetComponentInParent<Part>();
            
            Queue<Part> queue = new Queue<Part>();
            HashSet<Part> visited = new HashSet<Part> { root };

            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                Part a = queue.Dequeue();
                Part[] connected = group.GetConnectedJoints(a).Select(j => j.GetOtherPart(a)).ToArray();

                foreach (Part next in connected)
                    if (!visited.Contains(next))
                    {
                        if (next.HasModule<DetachModule>())
                        {
                            DetachModule[] modules = next.GetModules<DetachModule>();

                            if (modules.Any(m => m.activatedByLES))
                            {
                                foreach (DetachModule module in modules)
                                    if (module.activatedByLES)
                                        module.Detach(usePartData);
                                
                                PlayerController.main.SetOffset(Vector2.zero, 0.5f); // Overwrites smooth time
                                return true;
                            }
                        }
                        
                        queue.Enqueue(next);
                        visited.Add(next);
                    }
            }

            return false;
        }
    }   
}