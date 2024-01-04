using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct FTParametrisedRequirement
{
    public FTEntity preciseTarget;
    public FTRequirement looseTarget;
    public FTRequirement requirement;
    public bool targetSelf;

    public FTRequirement GetPropperRequirement()
    {
        FTRequirement r = (FTRequirement) ScriptableObject.CreateInstance("FTRequirement");
        r.CopyValues(requirement);
        if(preciseTarget != null) r.target = preciseTarget;
        //to be done
        if (looseTarget != null) r.targetType = looseTarget;
        r.targetSelf = targetSelf;
        return r;
        //requirement.target = preciseTarget;
        //requirement.targetType = looseTarget;
        //return requirement;
    }
}
