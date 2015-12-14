using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public abstract class Effect : System.Object {

    public Entity invoker;
    public Entity owner;

    public int uses = 1;
    public delegate IEnumerator Generic_event(Changer alter = null);
    public Dictionary<CombatAction.Events, Generic_event> events;

    public Effect ()
    {
        events = new Dictionary<CombatAction.Events, Generic_event>()
        {
            {CombatAction.Events.start, Start_Event},
            {CombatAction.Events.end, End_Event}
        };
    }

    public Effect Copy ()
    {
        var method = typeof(Effect).GetMethod("createNewInstanceStep1", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(this.GetType());
        var value = method.Invoke(this, null);
        return (Effect)value;
    }
    protected abstract void createNewInstanceStep2();

    protected T createNewInstanceStep1<T>() where T : Effect, new()
    {
        T newInstance = new T(); // works!

        //Copy default properties
        newInstance.owner = owner;
        newInstance.invoker = invoker;

        //Call the new instance's step 2 method, and return the result.
        newInstance.createNewInstanceStep2();
        return newInstance;
    }

    public IEnumerator Call_Event(CombatAction.Events eventIndex, Changer alter = null)
    {
        if (!events.ContainsKey(eventIndex)) yield break;
        uses--;
        yield return owner.StartCoroutine(events[eventIndex](alter));

        if (uses <= 0 && eventIndex != CombatAction.Events.end)
        {
            yield return owner.StartCoroutine(owner.Remove_Effect(this));
        }
        yield break;
    }

    public virtual IEnumerator Start_Event(Changer alter = null)
    {
        yield break;
    }
    public virtual IEnumerator End_Event(Changer alter = null)
    {
        yield break;
    }
}
public class Basic_Damage : Effect
{

    protected override void createNewInstanceStep2() //copy
    {
        
    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;

        int damage = Mathf.RoundToInt(combat.dmg * Random.Range(0.7f, 1.3f));

        Stats.Values adds = new Stats.Values(-damage);
        Changer my_alter = new Changer(invoker, owner, adds, combat.crit, combat.hit);

        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
public class Damage_Drop2Zero : Effect
{

    public Damage_Drop2Zero () : base()
    {
        uses = 2;
        events.Remove(CombatAction.Events.start);
        events.Add(CombatAction.Events.atRecieveAlter, AtRecieveAlter_Event);
    }

    protected override void createNewInstanceStep2() //copy
    {

    }

    public IEnumerator AtRecieveAlter_Event(Changer alter = null)
    {
        if (alter.adds.hp < 0)
            alter.adds.hp = 0;
        yield break;
    }
}
public class Critical_100 : Effect
{

    public Critical_100() : base()
    {
        uses = 1;
        events.Remove(CombatAction.Events.start);
        events.Add(CombatAction.Events.atCauseAlter, AtCauseAlter_Event);
    }

    protected override void createNewInstanceStep2() //copy
    {

    }

    public IEnumerator AtCauseAlter_Event(Changer alter = null)
    {
        alter.critical = true;
        yield break;
    }
}
public class Critical_100_2: Effect
{
    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;

        int damage = Mathf.RoundToInt(combat.dmg * Random.Range(0.7f, 1.3f));

        Stats.Values adds = new Stats.Values(-damage);
        Changer my_alter = new Changer(invoker, owner, adds, 100, combat.hit);
        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
    }
}
public class Basic_Heal : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;

        int heal = Mathf.RoundToInt(4 * Random.Range(0.7f, 1.3f));

        Stats.Values adds = new Stats.Values(heal);
        Changer my_alter = new Changer(invoker, owner, adds, 0, 100);
        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
    }
}
public class Big_Heal : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;

        int heal = Mathf.RoundToInt(8 * Random.Range(0.7f, 1.3f));

        Stats.Values adds = new Stats.Values(heal);
        Changer my_alter = new Changer(invoker, owner, adds, 0, 100);
        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
    }
}
public class Group_Damage : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;

        int damage = Mathf.RoundToInt(5 * Random.Range(0.7f, 1.3f));

        Stats.Values adds = new Stats.Values(-damage);
        Changer my_alter = new Changer(invoker, owner, adds, 0, 100);
        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
    }
}
public class Risky_Strike : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Changer alter = null)
    {
        Stats.Values combat = invoker.myInfo.stats.combat;
        int damage = Mathf.RoundToInt(combat.dmg * Random.Range(9.0f, 10.5f));

        Stats.Values adds = new Stats.Values(-damage);
        Changer my_alter = new Changer(invoker, owner, adds, 0, 10);
        yield return invoker.StartCoroutine(my_alter.Send()); //HAVE TO IMPROVE THIS
    }
}
