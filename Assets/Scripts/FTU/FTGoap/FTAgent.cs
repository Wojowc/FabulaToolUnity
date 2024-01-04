using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class FTAgent : MonoBehaviour
{
    public int initiative = 10;
    public List<FTSubGoal> goals = new List<FTSubGoal>();
    public List<FTAction> actions = new List<FTAction>();

    FTPlanner planner;
    Queue<FTAction> actionQueue;
    public FTAction currentAction;
    FTSubGoal currentGoal;

    int retries = 0;
    int waitTurns = 0;



    private void OnDestroy()
    {
        FTEntityManager.Instance.allAgents.Remove(this);
    }



    protected virtual void Start()
    {
        goals.ForEach(x => x.PrepareSubGoal());
        //FTTurnManager.Instance.runTurn += RunTurn;
        FTEntityManager.Instance.allAgents.Add(this);
    }

    bool invoked = false;

    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
        retries = 0;
        RemoveGoal();
    }

    public void RegenerateActions()
    {
        FTActionManager.Instance.RegenerateActions(this.GetComponent<FTEntity>());
    }

    public string GetCurrentActionName()
    {
        if (currentAction == null) return "null";
        if (currentAction.target != null) return currentAction.actionName;
        return currentAction.actionName;
    }

    public string GetCurrentGoalName()
    {
        if (currentGoal == null) return "null";
        return currentGoal.name;
    }

    public void AddStoryGoal(FTSubGoal g)
    {
        currentAction.running = false;
        invoked = false;
        retries = 0;
        goals.Add(g);
        actionQueue = null;
        MakePlan();
    }

    public void RunTurn()
    {
        MakePlan();

        foreach (FTAction act in actions.FindAll(a => a == null))
        {
            actions.Remove(act);
        }

        bool acted = false;

        if (waitTurns > 0)
        {
            Debug.Log(name + " continued their action");
            waitTurns--;
            if (waitTurns <= 0)
            {
                CompleteAction();
                acted = true;
            }
            else return;
        }

        

        if (currentAction != null)
        {
            if (currentAction.running)
            {
                retries = 0;
                if (!invoked)
                {
                    waitTurns = currentAction.duration;
                    invoked = true;
                    if (waitTurns <= 0)
                    {
                        CompleteAction();
                        return;
                    }
                }
                return;
            }
        }

        RemakeCurrentPlan();

        if (retries > 0 && currentAction != null)
        {
            if(!currentAction.running)
            {
                acted = true;
                retries--;

                FTActionCompletion completion = currentAction.PrePerform();
                if (completion == FTActionCompletion.Full)
                {
                    currentAction.running = true;
                }
                else if (completion == FTActionCompletion.Failed)
                {
                    retries = 0;
                    actionQueue = null;
                    currentAction = null;
                    RemakeCurrentPlan(true);
                }
            }
        }

        if (actionQueue != null && actionQueue.Count > 0 && !acted)
        {
            //get new action
            currentAction = actionQueue.Dequeue();
            retries = currentAction.retries;
            FTActionCompletion completion = currentAction.PrePerform();

            //check the level of completion and act accordingly
            if (completion == FTActionCompletion.Full)
            {
                currentAction.running = true;
            }
            else if (completion == FTActionCompletion.Failed)
            {
                actionQueue = null;
                currentAction = null;
                RemakeCurrentPlan(true);
            }
        }

        RemoveGoal();
    }


    private void RemoveGoal()
    {
        if (currentGoal == null) return;
        if (currentGoal.steps.All(x => x.CheckFor(GetComponent<FTEntity>().entityState)))
        {
            if (currentGoal.doRemove)
            {
                Debug.Log("Removing goal: " + currentGoal.name);
                goals.Remove(currentGoal);
            }
            planner = null;
        }
    }

    private void RemakeCurrentPlan(bool doAct = false)
    {
        if (actionQueue == null) return;

        planner = new FTPlanner();

        if(doAct == true) Debug.Log(name + " recalculates " + currentGoal.name);

        actionQueue = planner.Plan(actions, currentGoal.steps, GetComponent<FTEntity>().entityState);


        if (actionQueue == null)
        {
            MakePlan();
        }

        else if (actionQueue != null && actionQueue.Count > 0 && doAct)
        {
            currentAction = actionQueue.Dequeue();
            retries = currentAction.retries;

            FTActionCompletion completion = currentAction.PrePerform();

            if (completion == FTActionCompletion.Full)
            {
                currentAction.running = true;
            }
            /*else if (completion == FTActionCompletion.Failed)
            {
                actionQueue = null;
                currentAction = null;
                RemakeCurrentPlan();
            }*/
        }

    }


    private void MakePlan()
    {
        if ((planner == null || actionQueue == null) && retries <= 0)
        {
            //create planner and create a temporary list of goals
            planner = new FTPlanner();
            List<FTSubGoal> tempGoals;

            //fill list with content
            if (goals.Any(x => x.storyGoal)) tempGoals = new List<FTSubGoal>(goals.FindAll(x => x.storyGoal));
            else tempGoals = new List<FTSubGoal>(goals);

            //get random goal and try to make a graphh
            for (int i = 0; i < goals.Count; i++)
            {
                //get random goal
                List<int> weights = new List<int>();
                tempGoals.ForEach(x => weights.Add(x.weight));
                if (tempGoals.Count <= 0) continue;
                FTSubGoal sg = tempGoals[FTRandom.RandomWeightedIndex(weights)];
                //initialize requirements to assure they have a correct target
                foreach(FTRequirement step in sg.steps)
                {
                    step.Initialize(GetComponent<FTEntity>());
                }
                //try to make a plan, if the plan is valid exit loop
                actionQueue = planner.Plan(actions, sg.steps, GetComponent<FTEntity>().entityState);
                if (actionQueue != null)
                {
                    currentGoal = sg;
                    break;
                }

                else tempGoals.Remove(sg);
            }
        }
    }
}
