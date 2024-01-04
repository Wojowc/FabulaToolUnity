using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CM_Name", menuName = "FabulaTool/Cyclic Modifier", order = 4)]
[System.Serializable]
public class FTCyclicModifier : ScriptableObject
{
    public string cyclicModName;
    public int frequency;
    private int currentCounter = 0;
    public FTModifier modifier;

    public FTCyclicModifier(FTCyclicModifier m)
    {
        frequency = m.frequency;
        modifier = m.modifier;
        cyclicModName = m.cyclicModName;
        currentCounter = 0;
    }


    public bool Tick()
    {
        if(currentCounter >= frequency - 1)
        {
            currentCounter = 0;
            return true;
        }
        else
        {
            currentCounter++;
            return false;
        }
    }
}
