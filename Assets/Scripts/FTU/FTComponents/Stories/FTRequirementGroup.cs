using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public struct FTRequirementGroup 
{
    
    public string groupName;
    public bool treatAsOr;
    public List<FTRequirement> groupedRequirements;

    [HideInInspector]
    public bool destroyOnInitialize;

    public FTRequirementGroup(string name)
    {
        groupName = name;
        groupedRequirements = new List<FTRequirement>();
        destroyOnInitialize = false;
        treatAsOr = false;
    }

    public bool Qualify(FTEntityState entity)
    {
        if (groupedRequirements.Count == 0) return true;

        if(treatAsOr)
        {
            foreach (FTRequirement r in groupedRequirements)
            {
                if (r.CheckFor(entity)) return true;
            }
            return false;
        }
        else
        {
            foreach (FTRequirement r in groupedRequirements)
            {
                if (!r.CheckFor(entity)) return false;
            }
            return true;
        }
    }

    public bool Qualify(FTEntityState entity, List<FTModifier> conditions)
    {
        if (groupedRequirements.Count == 0) return true;

        if (treatAsOr)
        {
            foreach (FTRequirement r in groupedRequirements)
            {
                if (!r.CheckFor(entity, conditions)) return true;
            }
            return false;
        }
        else
        {
            foreach (FTRequirement r in groupedRequirements)
            {
                if (!r.CheckFor(entity, conditions)) return false;
            }
            return true;
        }
    }
}
