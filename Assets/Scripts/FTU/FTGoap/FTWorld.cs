using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTWorld
{
    private static readonly FTWorld instance = new FTWorld();
    private static FTWorldStates world;

    static FTWorld()
    {
        world = new FTWorldStates();
    }

    public static FTWorld Instance
    {
        get { return instance; }
    }

    public FTWorldStates GetWorld()
    {
        return world;
    }
}
