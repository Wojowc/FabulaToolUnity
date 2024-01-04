using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(fileName = "M_Name", menuName = "FabulaTool/Modifier", order = 3)]
public class FTModifier : ScriptableObject
{
    public FTEntity target;

    public bool destroyOnInitialize = false;

    public List<FTTag> addTags;
    public List<FTTag> removeTags;

    public List<FTModifierBoolExpression> boolExpressions;
    public List<FTModifierIntExpression> intExpressions;
    public List<FTModifierFloatExpression> floatExpressions;
    public List<FTModifierStringExpression> stringExpressions;


    public FTModifier(List<FTTag> addTags, List<FTTag> removeTags, List<FTModifierBoolExpression> boolExpressions, List<FTModifierIntExpression> intExpressions, List<FTModifierFloatExpression> floatExpressions, List<FTModifierStringExpression> stringExpressions)
    {
        this.addTags = new List<FTTag>(addTags);
        this.removeTags = new List<FTTag>(removeTags);
        this.boolExpressions = new List < FTModifierBoolExpression > (boolExpressions);
        this.intExpressions = new List<FTModifierIntExpression>(intExpressions);
        this.floatExpressions = new List<FTModifierFloatExpression>(floatExpressions);
        this.stringExpressions = new List<FTModifierStringExpression>(stringExpressions);
    }

    public FTModifier()
    {
        addTags = new List<FTTag>();
        removeTags = new List<FTTag>();
        boolExpressions = new List<FTModifierBoolExpression>();
        intExpressions = new List<FTModifierIntExpression>();
        floatExpressions = new List<FTModifierFloatExpression>();
        stringExpressions = new List<FTModifierStringExpression>();
    }

    public FTModifier(List<FTTag> addTags, List<FTTag> removeTags)
    {
        this.addTags = new List<FTTag>(addTags);
        this.removeTags = new List<FTTag>(removeTags);
    }

    public FTModifier(List<FTModifierBoolExpression> boolExpressions, List<FTModifierIntExpression> intExpressions, List<FTModifierFloatExpression> floatExpressions, List<FTModifierStringExpression> stringExpressions)
    {
        this.boolExpressions = new List<FTModifierBoolExpression>(boolExpressions);
        this.intExpressions = new List<FTModifierIntExpression>(intExpressions);
        this.floatExpressions = new List<FTModifierFloatExpression>(floatExpressions);
        this.stringExpressions = new List<FTModifierStringExpression>(stringExpressions);
    }

    public void ModifyEntity(FTEntityState entity, bool modTarget = false)
    {
        if (target != null && !modTarget)
        {
            ModifyEntity(target.entityState, true);
            return;
        }

        foreach (FTTag tag in addTags) if (!entity.tagList.Contains(tag)) entity.tagList.Add(tag);
        foreach (FTTag tag in removeTags) if (entity.tagList.Contains(tag)) entity.tagList.Remove(tag);

        foreach (FTModifierBoolExpression e in boolExpressions)
        {
            //if variable of a given type doesn't exist - create one with a modifier variable's value
            if (!entity.variables.boolVariables.Any(x => x.key == e.boolVariable.key)) entity.variables.boolVariables.Add(e.boolVariable);
            else
            {
                //modify variables
                for (int i = 0; i < entity.variables.boolVariables.Count; i++)
                {
                    FTVariable<bool> v = entity.variables.boolVariables[i];
                    if (entity.variables.boolVariables[i].key == e.boolVariable.key) e.ApplyTo(ref v);
                    entity.variables.boolVariables[i] = v;
                }
            }
        }
        foreach (FTModifierIntExpression e in intExpressions)
        {
            if (!entity.variables.intVariables.Any(x => x.key == e.intVariable.key)) entity.variables.intVariables.Add(e.intVariable);
            else
            {
                for (int i = 0; i < entity.variables.intVariables.Count; i++)
                {
                    FTVariable<int> v = entity.variables.intVariables[i];
                    if (entity.variables.intVariables[i].key == e.intVariable.key) e.ApplyTo(ref v);
                    entity.variables.intVariables[i] = v;
                }
            }
        }
        foreach (FTModifierFloatExpression e in floatExpressions)
        {
            if (!entity.variables.floatVariables.Any(x => x.key == e.floatVariable.key)) entity.variables.floatVariables.Add(e.floatVariable);
            else
            {
                for (int i = 0; i < entity.variables.floatVariables.Count; i++)
                {
                    FTVariable<float> v = entity.variables.floatVariables[i];
                    if (entity.variables.floatVariables[i].key == e.floatVariable.key) e.ApplyTo(ref v);
                    entity.variables.floatVariables[i] = v;
                }
            }
        }
        foreach (FTModifierStringExpression e in stringExpressions)
        {
            if (!entity.variables.stringVariables.Any(x => x.key == e.stringVariable.key)) entity.variables.stringVariables.Add(e.stringVariable);
            else
            {
                for (int i = 0; i < entity.variables.stringVariables.Count; i++)
                {
                    FTVariable<string> v = entity.variables.stringVariables[i];
                    if (entity.variables.stringVariables[i].key == e.stringVariable.key) e.ApplyTo(ref v);
                    entity.variables.stringVariables[i] = v;
                }
            }
        }
    }
}
