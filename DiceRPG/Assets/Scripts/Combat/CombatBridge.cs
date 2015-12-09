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
        owner.current_ap -= this.action.ap_cost;
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

        yield return null;
    }

}
