using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "A_Name", menuName = "FabulaTool/Action/Attack", order = 0)]
public class ActionAttack : FTAction
{
    public ActionAttack()
    {
        //effects.Add(new FTModifier(null,null,null,null,null,));
    }

    public override void Initialize()
    {
        base.Initialize();

        cost = target.entityState.variables.intVariables.First(x => x.key == "HP").value;
        if (target == actor) cost = 100;

        FTRequirement r = (FTRequirement) CreateInstance("FTRequirement");
        string targetLocation = target.entityState.variables.stringVariables.First(x => x.key == "Location").value;
        r.stringExpressions.Add(new FTLogicalExpression<string>("Location", FTSign.Equal, targetLocation));

        FTRequirementGroup rg = new FTRequirementGroup("Same location");
        rg.groupedRequirements.Add(r);
        rg.destroyOnInitialize = true;
        preconditions.Add(rg);

        FTModifier mod = (FTModifier)CreateInstance("FTModifier");
        mod.intExpressions.Add(new FTModifierIntExpression(FTOperation.Set, new FTVariable<int>("HP", 0)));
        mod.target = target;
        mod.destroyOnInitialize = true;
        effects.Add(mod);
    }

    public override bool PostPerform()
    {
        FTActionManager.Instance.RemoveWhereTarget(target);
        FTActionManager.Instance.RemoveWhereActor(target);
        Destroy(target.gameObject);
        Destroy(target);
        return true;
    }

    public override FTActionCompletion PrePerform()
    {
        if (preconditions.All(x => x.Qualify(actor.entityState)))
        {
            FTModifier mod = (FTModifier)CreateInstance("FTModifier");
            mod.intExpressions.Add(new FTModifierIntExpression(FTOperation.Add, new FTVariable<int>("HP", -3)));
            mod.ModifyEntity(target.entityState);


            if (target.entityState.variables.intVariables.First(x => x.key == "HP").value <= 0) return FTActionCompletion.Full;
            else return FTActionCompletion.Partial;
        }

        return FTActionCompletion.Failed;
    }
}
