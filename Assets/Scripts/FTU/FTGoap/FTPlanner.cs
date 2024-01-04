using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FTGoapNode
{
    public FTGoapNode parent;
    public float cost;
    public FTEntityState state;
    public FTAction action;

    public FTGoapNode(FTGoapNode parent, float cost, FTEntityState state, FTAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new FTEntityState(state);
        this.action = action;
    }
} 

public class FTPlanner
{
    public Queue<FTAction> Plan (List<FTAction> actions, List<FTRequirement> goal, FTEntityState state)
    {
        List<FTAction> usableActions = new List<FTAction>();
        foreach (FTAction a in actions)
        {
            if (a.isAchieveable())
            {
                usableActions.Add(a);
            }
        }

        List<FTGoapNode> leaves = new List<FTGoapNode> ();
        FTGoapNode start = new FTGoapNode(null, 0, state, null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success) return null;

        FTGoapNode cheapest = null;

        foreach (FTGoapNode leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else if (leaf.cost < cheapest.cost) cheapest = leaf;
        }

        List<FTAction> result = new List<FTAction> ();
        FTGoapNode n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }

            n = n.parent;
        }

        Queue<FTAction> queue = new Queue<FTAction>();
        result.ForEach(x => queue.Enqueue(x));

        return queue;
    }

    public bool BuildGraph(FTGoapNode parent, List<FTGoapNode> leaves, List<FTAction> usableActions, List<FTRequirement> goal)
    {
        bool pathExists = false;
        foreach (FTAction action in usableActions)
        {
            if(action.isAchieveableGiven(parent.state))
            {
                FTEntityState currentState = new FTEntityState(parent.state);
                List<FTModifier> targetModifiers = new List<FTModifier> ();

                foreach (FTModifier eff in action.effects)
                {
                    if (eff.target != null) targetModifiers.Add(eff);
                    else eff.ModifyEntity(currentState);
                }

                FTGoapNode node = new FTGoapNode(parent, parent.cost + action.cost, currentState, action);
                if (GoalAchieved(goal, currentState, targetModifiers))
                {
                    leaves.Add(node);
                    pathExists = true;
                }
                else
                {
                    List<FTAction> subset = ActionSubset(usableActions, action);
                    bool subsetPathExists = BuildGraph(node, leaves, subset, goal);
                    if (subsetPathExists) pathExists = true;
                }
            }
        }
        return pathExists;
    }

    private bool GoalAchieved(List<FTRequirement> goal, FTEntityState state, List<FTModifier> targetModifiers)
    {
        foreach(FTRequirement g in goal)
        {
            List<FTModifier> gMods = targetModifiers.FindAll(x => x.target == g.target);
            if (g.target == null) return g.CheckFor(state);
            else return g.CheckFor(state, gMods);
        }
        return true;
    }


    private List<FTAction> ActionSubset(List<FTAction> actions, FTAction removeMe)
    {
        List<FTAction> subset = new List<FTAction>();
        foreach (FTAction a in actions)
        {
            if(!a.Equals(removeMe))
            {
                subset.Add(a);
            }
        }

        return subset;
    }

}