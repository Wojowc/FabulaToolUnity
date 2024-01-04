using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FTSubGoal
{
    
    public string name;
    public int weight;
    public bool storyGoal;
    public bool doRemove;
    public List<FTParametrisedRequirement> requirementsToAccomplish;
    
    [HideInInspector]
    public List<FTRequirement> steps;

    public FTSubGoal(FTRequirement requirement, bool remove, int weight)
    {
        requirementsToAccomplish = new List<FTParametrisedRequirement>();
        steps = new List<FTRequirement>();
        steps.Add(requirement);
        doRemove = remove;
        this.weight = weight;
    }

    public void PrepareSubGoal()
    {
        foreach (FTParametrisedRequirement r in requirementsToAccomplish)
        {
            steps.Add(r.GetPropperRequirement());
        }        
    }
}