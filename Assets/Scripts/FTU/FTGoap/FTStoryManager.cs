using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTStoryManager : MonoBehaviour
{
    public static FTStoryManager Instance { get; private set; }
    public List<FTStoryNode> storyNodes;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ApplyPotentialNodes()
    {
        Debug.Log("============================");
        foreach(FTStoryNode s in storyNodes)
        {
            if(s.activationTurn == FTTurnManager.Instance.currentTurn)
            {
                Debug.Log("Correct");
                s.TryApply();
            }
        }
    }

}
