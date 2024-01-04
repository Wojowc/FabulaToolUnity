using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

[Serializable]
public class FTEntity : MonoBehaviour
{
    //delegate void EntityAction();

    public FTEntityState entityState;
    public List<FTCyclicModifier> cyclicModifiers = new List<FTCyclicModifier>();
    public List<FTEntity> connectedEntities = new List<FTEntity>();

    public void ConnectTo(FTEntity entity, bool reverse = false)
    {
        connectedEntities.Add(entity);
        if (!reverse) entity.ConnectTo(this, true);
    }

    public void DisconnectFrom(FTEntity entity, bool reverse = false)
    {
        connectedEntities.Remove(entity);
        if (!reverse) entity.DisconnectFrom(this, true);
    }


    private void OnValidate()
    {
        UpdateVariables();
    }

    #region Validation
    public void Validate()
    {
        ValidateTagList();
        UpdateVariables();
    }

    public void ValidateTagList()
    {
        List<FTTag> toBeModified = new List<FTTag>();

        foreach (FTTag tag in entityState.tagList)
        {
            if (!ValidateTag(tag))
            {
                toBeModified.Add(tag);
            }
        }

        toBeModified.ForEach(x => entityState.tagList.Remove(x));

        toBeModified.Clear();
        

        foreach (FTTag tag in entityState.tagList)
        {
            AddTagParents(tag, ref toBeModified);
        }

        toBeModified.ForEach(x => entityState.tagList.Add(x));

        entityState.tagList = entityState.tagList.Distinct().ToList();
    }

    private void AddTagParents(FTTag tag, ref List<FTTag> toBeModified)
    {
        foreach (FTTag p in tag.parents)
        {
            if (ValidateTag(p))
            {
                if (!toBeModified.Any(x => x == p))
                {
                    toBeModified.Add(p);
                    AddTagParents(p, ref toBeModified);
                }             
            }
        }
    }

    private void UpdateVariables()
    {
        if (entityState.tagList.Count <= 0) return;

        List<FTVariable<bool>> legalBools = new List<FTVariable<bool>>();
        List<FTVariable<float>> legalFloats = new List<FTVariable<float>>();
        List<FTVariable<int>> legalInts = new List<FTVariable<int>>();
        List<FTVariable<string>> legalStrings = new List<FTVariable<string>>();

        foreach (FTTag tag in entityState.tagList)
        {
            if (entityState.variables.boolVariables.Count > 0)
            legalBools.AddRange(UpdateVariablesList<bool>(tag.variables.boolVariables, ref entityState.variables.boolVariables));
            if (entityState.variables.floatVariables.Count > 0)
                legalFloats.AddRange(UpdateVariablesList<float>(tag.variables.floatVariables, ref entityState.variables.floatVariables));
            if (entityState.variables.intVariables.Count > 0)
                legalInts.AddRange(UpdateVariablesList<int>(tag.variables.intVariables, ref entityState.variables.intVariables));
            if (entityState.variables.stringVariables.Count > 0)
                legalStrings.AddRange(UpdateVariablesList<string>(tag.variables.stringVariables, ref entityState.variables.stringVariables));
        }

        if (entityState.variables.boolVariables.Count > 0)
            RemoveUnwantedVariablesFromList<bool>(legalBools, ref entityState.variables.boolVariables);
        if (entityState.variables.floatVariables.Count > 0)
            RemoveUnwantedVariablesFromList<float>(legalFloats, ref entityState.variables.floatVariables);
        if (entityState.variables.intVariables.Count > 0)
            RemoveUnwantedVariablesFromList<string>(legalStrings, ref entityState.variables.stringVariables);
        if (entityState.variables.stringVariables.Count > 0)
            RemoveUnwantedVariablesFromList<int>(legalInts, ref entityState.variables.intVariables);

        DeleteCopies();
    }

    private List<FTVariable<T>> UpdateVariablesList<T>(List<FTVariable<T>> tagVariableList, ref List<FTVariable<T>> list)
    {
        List<FTVariable<T>> legalList = new List<FTVariable<T>>();

        foreach (FTVariable<T> v in tagVariableList)
        {
            legalList.Add(v);
            if (!list.Any(x => x.key == v.key)) list.Add(v);
        }

        return legalList;
    }

    public void DeleteCopiesFromList<T>(ref List<FTVariable<T>> list)
    {
        List<FTVariable<T>> individuals = new List<FTVariable<T>>();
        foreach (FTVariable<T> i in list)
        {
            if (!individuals.Any(x => x.key == i.key)) individuals.Add(i);
        }

        list = individuals;
    }

    public void DeleteCopies()
    {
        DeleteCopiesFromList(ref entityState.variables.boolVariables);
        DeleteCopiesFromList(ref entityState.variables.floatVariables);
        DeleteCopiesFromList(ref entityState.variables.stringVariables);
        DeleteCopiesFromList(ref entityState.variables.intVariables);
    }

    public void RemoveUnwantedVariablesFromList<T>(List<FTVariable<T>> legal, ref List<FTVariable<T>> targetList)
    {
        List<FTVariable<T>> illegal = new List<FTVariable<T>>();
        foreach (FTVariable<T> v in targetList) if (!legal.Any(x => x.key == v.key)) illegal.Add(v);
        foreach (FTVariable<T> v in illegal) targetList.Remove(v);
    }

    public bool ValidateTag(FTTag tag)
    {
        return tag != null;
    }

    #endregion

    private void Awake()
    {
        if (!FTEntityManager.Instance) FindAnyObjectByType<FTEntityManager>().AssureInstance();

        FTEntityManager.Instance.allEntities.Add(this);
        foreach (FTEntity e in connectedEntities)
        {
            if(!e.connectedEntities.Any(x => x==this))
            {
                e.connectedEntities.Add(this);
            }
        }
        if(entityState.variables.stringVariables.Any(x => x.key == "Location"))
        {
            GameObject g = GameObject.Find(entityState.variables.stringVariables.First(x => x.key == "Location").value);
            if(connectedEntities.Any(x => x == g)) connectedEntities.Add(g.GetComponent<FTEntity>());
        }
    }

    private void Start()
    {
        
        List<FTCyclicModifier> tempCMod = new List<FTCyclicModifier>(cyclicModifiers);
        foreach(FTCyclicModifier c in tempCMod)
        {
            FTCyclicModifier newC = new FTCyclicModifier(c);
            cyclicModifiers.Remove(c);
            cyclicModifiers.Add(newC);
        }

        FTTurnManager.Instance.runTurn += ApplyCyclic;
    }

    private void ApplyCyclic()
    {
        foreach(FTCyclicModifier c in cyclicModifiers)
        {
            if (c.Tick()) c.modifier.ModifyEntity(entityState);
        }
    }    

    private void OnDestroy()
    {
        FTActionManager.Instance.RemoveWhereTarget(this);
        FTActionManager.Instance.RemoveWhereActor(this);
        FTEntityManager.Instance.allEntities.Remove(this);
    }
}
