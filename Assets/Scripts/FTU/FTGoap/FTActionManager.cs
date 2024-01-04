using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FTActionManager : MonoBehaviour
{
    public static FTActionManager Instance { get; private set; }


    public List<FTPossibleAction> possibleActionList;
    [HideInInspector]
    public List<FTAction> allActions;
    private List<FTAction> toRemove;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else
        {
            Instance = this;
            toRemove = new List<FTAction>();
        }
    }

    private void Start()
    {
        GenerateActions();
    }

    private void Update()
    {
        foreach(FTAction a in toRemove)
        {
            allActions.Remove(a);
            Destroy(a);
        }        
        toRemove.Clear();
    }

    public void RegenerateActions(FTEntity entity)
    {

        if (entity.gameObject.TryGetComponent<FTAgent>(out FTAgent agent)) agent.actions = new List<FTAction>();
        else return;
        foreach (FTPossibleAction action in possibleActionList)
        {

                if (Qualify(entity, action.agentRequirements))
                {
                    if (action.selfOnly)
                    {
                        FTAction a = Instantiate(action.action);
                        a.actor = agent.GetComponent<FTEntity>();
                        a.actionName = a.actor.gameObject.name + " " + a.actionName;
                        a.Initialize();

                        Debug.Log(a.actionName + " " + a.cost);
                        agent.actions.Add(a);
                        allActions.Add(a);
                    }
                    else
                    {
                        foreach (FTEntity target in FTEntityManager.Instance.allEntities)
                        {
                            if (Qualify(target, action.targetRequirements))
                            {
                                FTAction a = Instantiate(action.action);
                                a.target = target;
                                a.actor = agent.GetComponent<FTEntity>();
                                a.actionName = a.actor.gameObject.name + " " + a.actionName + " " + a.target.gameObject.name;
                                a.Initialize();

                                Debug.Log(a.actionName + " " + a.cost);
                                agent.actions.Add(a);
                                allActions.Add(a);
                            }
                        }
                    }
                }
        }
    }


    public void GenerateActions()
    {
        //iterate through actions
        foreach (FTPossibleAction action in possibleActionList)
        {
            //iterate through possible agent entities
            foreach (FTEntity entity in FTEntityManager.Instance.allEntities)
            {
                //if has agent component and qualifies for the action's requirements proceed
                if (entity.gameObject.TryGetComponent<FTAgent>(out FTAgent agent)
                    && Qualify(entity, action.agentRequirements))
                {
                    //if action is directed on self create it's instance and initialize it
                    if (action.selfOnly)
                    {
                        FTAction a = Instantiate(action.action);
                        a.actor = agent.GetComponent<FTEntity>();
                        a.actionName = a.actor.gameObject.name + " " + a.actionName;
                        a.Initialize();
                        agent.actions.Add(a);
                        allActions.Add(a);
                    }
                    //else find a suitable target, instantiate and initialize
                    else
                    {
                        foreach (FTEntity target in FTEntityManager.Instance.allEntities)
                        {
                            if (Qualify(target, action.targetRequirements))
                            {
                                FTAction a = Instantiate(action.action);
                                a.target = target;
                                a.actor = agent.GetComponent<FTEntity>();
                                a.actionName = a.actor.gameObject.name + " " + a.actionName + " " + a.target.gameObject.name;
                                a.Initialize();
                                agent.actions.Add(a);
                                allActions.Add(a);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ReinitializeWhereActor(System.Type type, FTEntity actor)
    {
        foreach (FTAction a in allActions)
        {
            if(a.GetType() == type && a.actor == actor) a.Initialize();
        }
    }

    public void ReinitializeWhereTarget(System.Type type, FTEntity target)
    {
        foreach (FTAction a in allActions)
        {
            if (a.GetType() == type && a.target == target) a.Initialize();
        }
    }

    public void ReinitializeWhereTarget(FTEntity target)
    {
        foreach (FTAction a in allActions)
        {
            if (a.target == target) a.Initialize();
        }
    }

    public void RemoveWhereActor(FTEntity actor)
    {
        foreach (FTAction a in allActions)
        {
            if (a.actor == actor)
            {
                actor.GetComponent<FTAgent>().actions.Remove(a);
                toRemove.Add(a);
            }
        }
    }

    public void RemoveWhereTarget(FTEntity target)
    {
        foreach (FTAction a in allActions)
        {
            if (a.target == target && a.actor != null)
            {
                a.actor.GetComponent<FTAgent>().actions.Remove(a);
                toRemove.Add(a);
            }
        }
            
    }

    public bool Qualify(FTEntity entity, List<FTRequirementGroup> requirements)
    {
        if (requirements.Count == 0) return true;
        foreach (FTRequirementGroup r in requirements)
        {
            if (r.Qualify(entity.entityState)) return true;
        }

        return false;
    }
}
