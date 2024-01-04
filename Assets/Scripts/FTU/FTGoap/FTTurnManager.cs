using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FTTurnManager : MonoBehaviour
{
    public static FTTurnManager Instance { get; private set; }
    public bool realTime;
    public int currentTurn = 0;
    public float turnLength;
    private float lastTurnTimestamp;

    public delegate void RunTurn();
    public event RunTurn runTurn;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }


    private void LateUpdate()
    {
        if (realTime)
        {
            if (Time.time - lastTurnTimestamp >= turnLength)
            {
                lastTurnTimestamp = Time.time;
                PlayTurn();
            }
        }
        else if (Input.GetKeyDown(KeyCode.T)) PlayTurn();
    }


    public void PlayTurn()
    {
        currentTurn++;

        FTStoryManager.Instance.ApplyPotentialNodes();

        foreach(FTAgent a in 
            FTEntityManager.Instance.allAgents.OrderBy(x => x.initiative))
        {
            a.RunTurn();
        }
        
        if (runTurn != null) runTurn();        
    }


}
