using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct FTStoryNode
{
    public string nodeName;
    public int activationTurn;
    public FTEntity target;
    public List<FTRequirementGroup> requirements;

    public List<FTModifier> newModifiers;
    public List<FTSubGoal> newGoals;
    public List<FTCyclicModifier> newCyclicModifiers;

    public List<string> goalsToRemove;
    public List<string> cyclicModifiersToRemove;




    private void InitParameters()
    {
        newGoals.ForEach(x => x.PrepareSubGoal());
    }

    public void TryApply()
    {
        Debug.Log("Try Apply");
        InitParameters();
        if (target != null)
        {
            Debug.Log("Target exists");
            Apply(target);
            return;
        }
        if (requirements != null)
        {
            FTEntity entity = FTEntityManager.Instance.RandomEntity(requirements);
            if (entity != null)
            {
                Apply(entity);
            }
        }
    }

    private void Apply(FTEntity entity)
    {
        foreach(FTModifier m in newModifiers)
        {
            m.ModifyEntity(entity.entityState);
        }
        if (entity.GetComponent<FTAgent>())
        {
            FTAgent a = entity.GetComponent<FTAgent>();
            foreach (FTSubGoal s in newGoals)
            {
                Debug.Log(s.name + " goal added to " + a.name);
                a.AddStoryGoal(s);
            }

            foreach (string g in goalsToRemove)
            {
                if (a.goals.Any(x => x.name == g)) a.goals.Remove(a.goals.First(x => x.name == g));
            }


        }
        foreach(FTCyclicModifier c in newCyclicModifiers)
        {
            entity.cyclicModifiers.Add(c);
        }
        foreach (string c in cyclicModifiersToRemove)
        {
            if (entity.cyclicModifiers.Any(x => x.cyclicModName == c)) entity.cyclicModifiers.Remove(entity.cyclicModifiers.First());
        }

    }
}
