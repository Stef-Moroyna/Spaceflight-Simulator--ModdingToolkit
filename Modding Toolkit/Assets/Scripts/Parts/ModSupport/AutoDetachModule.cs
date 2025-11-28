using SFS.Parts;
using SFS.Parts.Modules;
using SFS.World;
using System.Collections.Generic;
using UnityEngine;
using static SFS.World.Rocket;

/**
 * Author: dani0105
 * created At: 05/02/2023
 * <summary>
 *  Automatic separation of parts connected with this module.
 * </summary>
 */
public class AutoDetachModule : MonoBehaviour, INJ_Rocket
{
    public Rocket Rocket { set; private get; }
    public Part Part;

    // list of priority parts to detach before the default detach module
    public string[] partToDetach = new string[0];
    // default detach module.
    public DetachModule detachModule;

    short _currentStage;
    List<Part> _visited;
    const short _max_search_level = 3;
    
    void Awake()
    {
        _currentStage = 0;
        _visited = new List<Part>();
    }

    /**
     * <summary>
     *  Search for a specific part of the rocket with a maximum deep search
     * </summary>
     */
    Part SearchPart(string partName, Part startPoint, int level)
    {
        _visited.Add(startPoint);
        if(level >= _max_search_level)
        {
            return null;
        }

        foreach (PartJoint joint in Rocket.jointsGroup.GetConnectedJoints(startPoint))
        {
            Part otherPart = joint.GetOtherPart(startPoint);
            if (AlreadyVisited(otherPart))
            {
                continue;
            }

            if(otherPart.name == partName)
            {
                return otherPart;
            }

            otherPart = SearchPart(partName, otherPart, level + 1);

            if(otherPart != null)
            {
                return otherPart;
            }
        }
        return null;
    }

    /**
     * <summary>
     *  Check is the part was visited
     * </summary>
     */
    bool AlreadyVisited(Part part)
    {
        return _visited.Exists(e=> e == part);
    }

    /**
     * <summary>
     *  Call this method to find and detach the part for the current stage.
     * </summary>
     */
    public void OnDetach(UsePartData data)
    {
        // if was call from stage
        if (data.sharedData.fromStaging)
        {

            if (_currentStage + 1 == partToDetach.Length || partToDetach.Length == 0)
            {
                detachModule.Detach(data);
            }
            
            _currentStage += 1;
            return;
        }
       
        if (partToDetach == null)
            return;

        Part otherPart = SearchPart(partToDetach[_currentStage], Part, 0);

        //clear visited list for the next call
        _visited = new List<Part>();
        _currentStage += 1;

        DetachModule[] otherPartModule = otherPart.GetModules<DetachModule>();

        if(otherPartModule == null || otherPartModule.Length == 0)
        {
            Debug.Log("Other part don't have DetachModule");
            return;
        }

        foreach (DetachModule a in otherPartModule)
            a.Detach(data);
    }
}