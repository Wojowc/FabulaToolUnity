using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FTModifierExpression
{
    public FTOperation operation;
}

[System.Serializable]
public class FTModifierIntExpression : FTModifierExpression
{
    public FTVariable<int> intVariable;

    public FTModifierIntExpression(FTOperation operation, FTVariable<int> intVariable)
    {
        this.operation = operation;
        this.intVariable = intVariable;
    }

    public void ApplyTo (ref FTVariable<int> initialVar)
    {
        if (operation == FTOperation.Add) initialVar.value += intVariable.value;
        else if (operation == FTOperation.Multiply) initialVar.value *= intVariable.value;
        else initialVar.value = intVariable.value;
    }
}

[System.Serializable]
public class FTModifierFloatExpression : FTModifierExpression
{
    public FTVariable<float> floatVariable;

    public FTModifierFloatExpression(FTOperation operation, FTVariable<float> floatVariable)
    {
        this.operation = operation;
        this.floatVariable = floatVariable;
    }

    public void ApplyTo(ref FTVariable<float> initialVar)
    {
        if (operation == FTOperation.Add)
        {
            initialVar.value += floatVariable.value;
        }

        else if (operation == FTOperation.Multiply)
        {
            initialVar.value *= floatVariable.value;
        }

        else
        {
            initialVar.value = floatVariable.value;
        }
    }
}

[System.Serializable]
public class FTModifierBoolExpression : FTModifierExpression
{
    public FTVariable<bool> boolVariable;

    public FTModifierBoolExpression(FTOperation operation, FTVariable<bool> boolVariable)
    {
        this.operation = operation;
        this.boolVariable = boolVariable;
    }

    public void ApplyTo(ref FTVariable<bool> initialVar)
    {
        initialVar.value = boolVariable.value;
    }
}

[System.Serializable]
public class FTModifierStringExpression : FTModifierExpression
{
    public FTVariable<string> stringVariable;

    public FTModifierStringExpression(FTOperation operation, FTVariable<string> stringVariable)
    {
        this.operation = operation;
        this.stringVariable = stringVariable;
    }

    public void ApplyTo(ref FTVariable<string> initialVar)
    {
        if (operation == FTOperation.Add)
        {
            initialVar.value += stringVariable.value;
        }

        else
        {
            initialVar.value = stringVariable.value;
        }
    }
}