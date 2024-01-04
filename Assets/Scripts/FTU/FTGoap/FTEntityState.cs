using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FTEntityState
{
    public FTEntityType entityType;
    public List<FTTag> tagList;
    public FTVariablesContainer variables;

    public FTEntityState (FTEntityType entityType, List<FTTag> tagList, FTVariablesContainer variables)
    {
        this.entityType = entityType;
        this.tagList = new List<FTTag>(tagList);
        this.variables = new FTVariablesContainer(variables);
    }

    public FTEntityState(FTEntityState state)
    {
        entityType = state.entityType;
        tagList = new List<FTTag> (state.tagList);
        variables = new FTVariablesContainer(state.variables);
    }
}
