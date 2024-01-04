using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct FTLogicalExpression<T>
{
    public string variableName;
    public FTSign sign;
    public T argument;
    
    public FTLogicalExpression (string variableName, FTSign sign, T argument)
    {  
        this.variableName = variableName;
        this.sign = sign;
        this.argument = argument;
    }

    public bool Compare(FTVariable<T> variable)
    {
        if (!variable.key.Equals(variableName)) return false;

        if (sign == FTSign.Equal) return variable.value.Equals(argument);

        else if (sign == FTSign.NotEqual) return !variable.value.Equals(argument);

        else if (sign == FTSign.LessOrEqual)
        {
            if (variable.value.Equals(argument)) return true;
            if (float.TryParse(variable.value.ToString(), out float val)
                && float.TryParse(argument.ToString(), out float arg)) return (val <= arg);
            else return false;
        }

        else if (sign == FTSign.GreaterOrEqual)
        {
            if (variable.value.Equals(argument)) return true;
            if (float.TryParse(variable.value.ToString(), out float val)
                && float.TryParse(argument.ToString(), out float arg)) return (val >= arg);
            else return false;
        }

        else if (sign == FTSign.Less)
        {
            if (float.TryParse(variable.value.ToString(), out float val)
                && float.TryParse(argument.ToString(), out float arg)) return (val < arg);
            else return false;
        }

        else if (sign == FTSign.Greater)
        {
            if (float.TryParse(variable.value.ToString(), out float val)
                && float.TryParse(argument.ToString(), out float arg)) return (val > arg);
            else return false;
        }

        else return false;
    }
}

