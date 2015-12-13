using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CombatBridge : System.Object
{
    public CombatAction action;
    public Entity owner;
    public Entity[] target;

    public CombatBridge (string action, Entity owner, Entity target)
    {
        this.action = CombatAction.library[action];
        this.owner = owner;
        this.target = this.action.Target(target);
        owner.add_action(this);
        owner.battle_stats.ap -= this.action.ap_cost;

        owner.guiRef.AddAction(action);
    }
    //Action
    public virtual IEnumerator Act()
    {
        yield return owner.StartCoroutine(owner.Call_Event(CombatAction.Events.target));

        target = action.Target(target[0]); ///Redraw target array to avoid any variation during combat

        foreach (Entity t in target)
        {
            yield return t.StartCoroutine(t.Call_Event(CombatAction.Events.targeted));
        }

        if (owner.guiRef.actions.Count > 0) owner.guiRef.actions[0].ShowActive(); ///Shows graphicly that the action is running

        yield break;
    }
}
public class Attack : CombatBridge
{

    public Attack (string action, Entity owner, Entity target) : base(action, owner, target){}

    public override IEnumerator Act()
    {
        yield return owner.StartCoroutine(base.Act());
        yield return owner.StartCoroutine(owner.Call_Event(CombatAction.Events.attack));

        yield return owner.StartCoroutine(owner.Animation_Attack());

        yield return owner.StartCoroutine(action.Activate(owner, target));

        foreach (Entity t in target)
        {
            yield return t.StartCoroutine(t.Call_Event(CombatAction.Events.attacked));
        }


        if (owner.guiRef.actions.Count > 0) owner.guiRef.RemoveAction(0); ///Remove action at graphic ref

        yield return null;
    }

}
public class CardCall : CombatBridge
{

    public CardCall(string action, Entity owner, Entity target) : base(action, owner, target){ }

    public override IEnumerator Act()
    {

        yield return owner.StartCoroutine(base.Act());
        yield return owner.StartCoroutine(owner.Call_Event(CombatAction.Events.spell));

        yield return owner.StartCoroutine(owner.Animation_Card());

        yield return owner.StartCoroutine(action.Activate(owner, target));

        foreach (Entity t in target)
        {
            yield return t.StartCoroutine(t.Call_Event(CombatAction.Events.spelled));
        }

        if (owner.guiRef.actions.Count > 0) owner.guiRef.RemoveAction(0); ///Remove action at graphic ref

        yield return null;
    }

}
