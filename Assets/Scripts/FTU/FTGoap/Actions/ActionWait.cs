using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "A_Name", menuName = "FabulaTool/Action/Wait", order = 0)]
public class ActionWait : FTAction
{
    public ActionWait()
    {
        //effects.Add(new FTModifier(null,null,null,null,null,));
    }

    public override void Initialize() { return; }


    public override bool PostPerform()
    {
        return true;
    }

    public override FTActionCompletion PrePerform()
    {
        Debug.Log(actor.name + " waited");
        foreach (FTModifier mod in effects)
        {
            mod.ModifyEntity(actor.entityState);
        }
        return FTActionCompletion.Full;
    }
}
