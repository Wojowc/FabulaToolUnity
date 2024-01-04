using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct FTVariablesContainer
{
    public List<FTVariable<bool>> boolVariables;
    public List<FTVariable<int>> intVariables;
    public List<FTVariable<float>> floatVariables;
    public List<FTVariable<string>> stringVariables;

    public FTVariablesContainer (FTVariablesContainer container)
    {
        boolVariables = new List<FTVariable<bool>>(container.boolVariables);
        intVariables = new List<FTVariable<int>>(container.intVariables);
        floatVariables = new List<FTVariable<float>>(container.floatVariables); 
        stringVariables = new List<FTVariable<string>>(container.stringVariables);
    }
}
