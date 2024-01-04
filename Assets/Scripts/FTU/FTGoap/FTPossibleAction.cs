using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FTPossibleAction
{
    public FTAction action;
    public bool selfOnly = false;
    public List<FTRequirementGroup> agentRequirements;
    public List<FTRequirementGroup> targetRequirements;
}
