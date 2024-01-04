using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

//[Serializable]
[CreateAssetMenu(fileName = "R_Name", menuName = "FabulaTool/Requirement", order = 2)]
public class FTRequirement : ScriptableObject
{

    public FTRequirement targetType;
    public FTEntity target;

    [HideInInspector]
    public bool destroyOnInitialize = false;
    public bool targetSelf = false;
    private bool valid = true;

    public List<FTEntityType> requiredTypes = new List<FTEntityType>();
    public List<FTEntityType> prohibitedTypes = new List<FTEntityType>();
    public List<FTTag> requiredTags = new List<FTTag>();
    public List<FTTag> prohibitedTags = new List<FTTag>();

    public List<FTLogicalExpression<bool>> boolExpressions = new List<FTLogicalExpression<bool>>();
    public List<FTLogicalExpression<int>> intExpressions = new List<FTLogicalExpression<int>>();
    public List<FTLogicalExpression<float>> floatExpressions = new List<FTLogicalExpression<float>>();
    public List<FTLogicalExpression<string>> stringExpressions = new List<FTLogicalExpression<string>>();

    public void Initialize(FTEntity toExclude)
    {
        valid = true;
        if (targetType == null) return;

        List<FTEntity> adequateEntities = new List<FTEntity>();
        foreach(FTEntity e in FTEntityManager.Instance.allEntities)
        {
            if (targetType.CheckFor(e.entityState) && (targetSelf || e != toExclude))
            {
                adequateEntities.Add(e);
            }
        }

        if (adequateEntities.Count > 0)
        {
            target = adequateEntities[UnityEngine.Random.Range(0, adequateEntities.Count)];
            Debug.Log("The target is " + target);
        }
        else valid = false;
    }


    public void CopyValues(FTRequirement requirement)
    {
        requiredTypes = new List<FTEntityType>(requirement.requiredTypes);
        prohibitedTypes = new List<FTEntityType>(requirement.prohibitedTypes);
        requiredTags = new List<FTTag>(requirement.requiredTags);
        prohibitedTags = new List<FTTag>(requirement.prohibitedTags);
        
        boolExpressions = new List<FTLogicalExpression<bool>>(requirement.boolExpressions);
        intExpressions = new List<FTLogicalExpression<int>>(requirement.intExpressions);
        floatExpressions = new List<FTLogicalExpression<float>>(requirement.floatExpressions);
        stringExpressions = new List<FTLogicalExpression<string>>(requirement.stringExpressions);
    }

    public bool CheckFor(FTEntityState entity, bool testingTarget = false)
    {
        if (!valid) return false;
        if (target != null && !testingTarget)
        {
            return CheckFor(target.entityState, true);
        }


        if (!(requiredTypes.Any(x => x == entity.entityType) || requiredTypes.Count == 0)
            || prohibitedTypes.Contains(entity.entityType)) return false;

        foreach (FTTag t in requiredTags) if (!entity.tagList.Any(x => x == t)) return false;

        foreach (FTTag t in prohibitedTags) if (entity.tagList.Any(x => x == t)) return false;

        foreach (FTLogicalExpression<bool> l in boolExpressions)
            if (!entity.variables.boolVariables.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<int> l in intExpressions)
            if (!entity.variables.intVariables.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<float> l in floatExpressions)
            if (!entity.variables.floatVariables.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<string> l in stringExpressions)
            if (!entity.variables.stringVariables.Any(x => l.Compare(x))) return false;

        return true;
    }


    public bool CheckFor(FTEntityState entity, List<FTModifier> modifiers, bool testingTarget = false)
    {
        if (target != null && !testingTarget)
        {
            return CheckFor(target.entityState, modifiers, true);
        }

        List<FTTag> newTagList = new List<FTTag>(entity.tagList);
        List<FTVariable<bool>> newBoolList = new List<FTVariable<bool>>(entity.variables.boolVariables);
        List<FTVariable<int>> newIntList = new List<FTVariable<int>>(entity.variables.intVariables);
        List<FTVariable<float>> newFloatList = new List<FTVariable<float>>(entity.variables.floatVariables);
        List<FTVariable<string>> newStringList = new List<FTVariable<string>>(entity.variables.stringVariables);


        foreach (FTModifier m in modifiers)
        {
            foreach (FTTag tag in m.addTags) if (!newTagList.Contains(tag)) newTagList.Add(tag);
            foreach (FTTag tag in m.removeTags) if (newTagList.Contains(tag)) newTagList.Remove(tag);

            foreach (FTModifierBoolExpression e in m.boolExpressions)
            {
                if (!newBoolList.Any(x => x.key == e.boolVariable.key)) newBoolList.Add(e.boolVariable);
                else
                {
                    for (int i = 0; i < newBoolList.Count; i++)
                    {
                        FTVariable<bool> v = newBoolList[i];
                        if (newBoolList[i].key == e.boolVariable.key) e.ApplyTo(ref v);
                        newBoolList[i] = v;
                    }
                }
            }
            foreach (FTModifierIntExpression e in m.intExpressions)
            {
                if (!newIntList.Any(x => x.key == e.intVariable.key)) newIntList.Add(e.intVariable);
                else
                {
                    for (int i = 0; i < newIntList.Count; i++)
                    {
                        FTVariable<int> v = newIntList[i];
                        if (newIntList[i].key == e.intVariable.key) e.ApplyTo(ref v);
                        newIntList[i] = v;
                    }
                }
            }
            foreach (FTModifierFloatExpression e in m.floatExpressions)
            {
                if (!newFloatList.Any(x => x.key == e.floatVariable.key)) newFloatList.Add(e.floatVariable);
                else
                {
                    for (int i = 0; i < newFloatList.Count; i++)
                    {
                        FTVariable<float> v = newFloatList[i];
                        if (newFloatList[i].key == e.floatVariable.key) e.ApplyTo(ref v);
                        newFloatList[i] = v;
                    }
                }
            }
            foreach (FTModifierStringExpression e in m.stringExpressions)
            {
                if (!newStringList.Any(x => x.key == e.stringVariable.key)) newStringList.Add(e.stringVariable);
                else
                {
                    for (int i = 0; i < newStringList.Count; i++)
                    {
                        FTVariable<string> v = newStringList[i];
                        if (newStringList[i].key == e.stringVariable.key) e.ApplyTo(ref v);
                        newStringList[i] = v;
                    }
                }
            }
        }

        if (!(requiredTypes.Any(x => x == entity.entityType) || requiredTypes.Count == 0)
            || prohibitedTypes.Contains(entity.entityType)) return false;

        foreach (FTTag t in requiredTags) if (!newTagList.Any(x => x == t) && !modifiers.Any(x => x.addTags.Any(y => y == t))) return false;
        foreach (FTTag t in prohibitedTags) if (newTagList.Any(x => x == t)) return false;

        foreach (FTLogicalExpression<bool> l in boolExpressions)
            if (!newBoolList.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<int> l in intExpressions)
            if (!newIntList.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<float> l in floatExpressions)
            if (!newFloatList.Any(x => l.Compare(x))) return false;

        foreach (FTLogicalExpression<string> l in stringExpressions)
            if (!newStringList.Any(x => l.Compare(x))) return false;

        return true;
    }
}
