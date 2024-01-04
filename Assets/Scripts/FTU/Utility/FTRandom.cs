using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTRandom : MonoBehaviour 
{
    public static int RandomWeightedIndex(List<int> weights)
    {
        if (weights.Count == 0) return 0;

        int weightSum = 0;
        foreach (int i in weights)
        {
            weightSum += i;
        }

        int r = Random.Range(0, weightSum);

        for(int i = 0; i < weights.Count; i++)
        {
            r -= weights[i];
            if (r <= 0)
            {
                return i;
            }
        }
        
        return weights.Count - 1;
    }
}
