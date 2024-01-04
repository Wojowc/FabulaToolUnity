using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
using System.Linq;
using System;

[Serializable]
[CreateAssetMenu(fileName = "A_Name", menuName = "FabulaTool/Action/Base", order = 0)]
public class FTAction : ScriptableObject
{
    public string actionName = "Action";
    public bool extendedRequirements = true;
    public float cost = 1;
    public int duration;
    public int retries = 0;

    public FTEntity actor;
    public FTEntity target; 

    public List<FTRequirementGroup> preconditions;
    public List<FTModifier> effects;

    public bool running = false;

    public FTAction()
    {
        preconditions = new List<FTRequirementGroup>();
        effects = new List<FTModifier>();
    }

    public bool isAchieveable()
    {
        return true;
    }

    public bool isAchieveableGiven(FTEntityState state)
    {
        if (preconditions.Count == 0) return true;
        if (extendedRequirements)
        {
            foreach (FTRequirementGroup r in preconditions)
            {
                //CHANGES
                if (r.Qualify(state)) return r.Qualify(state);
            }

            return false;
        }
        else
        {
            foreach (FTRequirement p in preconditions[0].groupedRequirements)
            {
                if (!p.CheckFor(state)) return false;
            }
            return true;
        }
    }


    public bool isAchieveableGiven(FTEntityState state, List<FTModifier> conditions)
    {
        //if action doesn't have any requirements return true
        if (preconditions.Count == 0) return true;
        //check all the requirements
        if (extendedRequirements)
        {
            foreach (FTRequirementGroup r in preconditions)
            {
                if (r.Qualify(state, conditions)) return r.Qualify(state, conditions);
            }
            return false;
        }
        //check only for the first requirement group
        else
        {
            foreach (FTRequirement p in preconditions[0].groupedRequirements)
            {
                if (!p.CheckFor(state, conditions)) return false;
            }
            return true;
        }
    }

    public virtual void Initialize() {

        List<FTRequirementGroup> tempR = new List<FTRequirementGroup>(preconditions);
        List<FTModifier> tempE = new List<FTModifier>(effects);


        foreach (FTRequirementGroup r in tempR)
        {
            if (r.destroyOnInitialize) preconditions.Remove(r);
        }
        foreach (FTModifier m in tempE)
        {
            if (m.destroyOnInitialize) effects.Remove(m);
        }
        return; 
    }
    public virtual FTActionCompletion PrePerform() { return FTActionCompletion.Full; }
    public virtual bool PostPerform() { return true; }
}
