using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "A_Name", menuName = "FabulaTool/Action/GoTo", order = 0)]
public class ActionGoTo : FTAction
{
    public ActionGoTo()
    {
        //effects.Add(new FTModifier(null,null,null,null,null,));
    }

    public override void Initialize()
    {
        base.Initialize();


        FTRequirementGroup rg = new FTRequirementGroup("Same location");
        rg.treatAsOr = true;
        
        rg.destroyOnInitialize = true;
        preconditions.Add(rg);

        foreach (FTEntity l in target.connectedEntities)
        {
            if (l.entityState.entityType == FTEntityType.Location)
            {
                FTRequirement r = (FTRequirement)CreateInstance("FTRequirement");
                string targetLocation = l.name;
                r.stringExpressions.Add(new FTLogicalExpression<string>("Location", FTSign.Equal, targetLocation));
                rg.groupedRequirements.Add(r);
            }
        }







        FTModifier mod = (FTModifier)CreateInstance("FTModifier");
        mod.stringExpressions.Add(new FTModifierStringExpression(FTOperation.Set, new FTVariable<string>("Location", target.name)));
        mod.destroyOnInitialize = true;
        effects.Add(mod);

        cost = 1; //Vector3.Distance(actor.transform.position, target.transform.position)/3 + 1; 
        duration = (int)cost;// - 1;
    }

    public override bool PostPerform()
    {
        //actor.transform.position = target.gameObject.transform.position;

        if (actor.GetComponent<FTVNode>())
        {
            //actor.connectedEntities.ForEach(x => Debug.Log(x.name + " entity"));
            GameObject exLocation = actor.connectedEntities.Find(x => x.name == 
                actor.entityState.variables.stringVariables.First(x => x.key == "Location").value).gameObject;
            actor.GetComponent<FTVNode>().RemoveJoint(exLocation);
            actor.GetComponent<FTVNode>().AddJoint(target.gameObject);
        }

        actor.DisconnectFrom(actor.connectedEntities.Find(x => x.entityState.entityType == FTEntityType.Location));
        actor.ConnectTo(target);

        Debug.Log(actor.name + " reached the " + target.name);
        foreach (FTModifier mod in effects)
        {
            mod.ModifyEntity(actor.entityState);
        }

        FTActionManager.Instance.ReinitializeWhereActor(typeof(ActionGoTo), actor);
        FTActionManager.Instance.ReinitializeWhereTarget(actor);
        return true;
    }

    public override FTActionCompletion PrePerform()
    {
        Debug.Log(actor.name + " started a journey to the " + target.name);
        return FTActionCompletion.Full;
    }
}
