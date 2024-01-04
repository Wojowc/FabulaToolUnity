using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTEntityManager : MonoBehaviour
{
    public static FTEntityManager Instance { get; private set; }
    public List<FTAgent> allAgents = new List<FTAgent>();
    public List<FTEntity> allEntities = new List<FTEntity>();
    public List<FTEntity> toBeDestroyedNextTurn = new List<FTEntity>();


    private void Awake()
    {
        AssureInstance();
    }

    public void AssureInstance()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public FTEntity RandomEntity(List<FTRequirementGroup> requirements)
    {
        List<FTEntity> validEntities = new List<FTEntity>();
        foreach(FTEntity entity in allEntities)
        {
            if (requirements.Count == 0)
            {
                validEntities.Add(entity);
                continue;
            }
            foreach (FTRequirementGroup r in requirements)
            {
                if (r.Qualify(entity.entityState))
                {
                    validEntities.Add(entity);
                    break;
                }
            }
        }

        if (validEntities.Count == 0) return null;
        return validEntities[Random.Range(0, validEntities.Count)];
    }
}
