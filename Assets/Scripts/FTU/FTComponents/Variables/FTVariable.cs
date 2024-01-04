using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct FTVariable<T>
{
    public string key;
    public T value;

    public FTVariable(string key, T value)
    {
        this.key = key;
        this.value = value;
    }
}