using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[Serializable]
[CreateAssetMenu(fileName = "R_Name", menuName = "FabulaTool/Tag", order = 1)]
public class FTTag : ScriptableObject
{
    public List<FTTag> parents;
    public FTVariablesContainer variables;
}
