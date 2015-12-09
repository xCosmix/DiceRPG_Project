using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class Effect : System.Object {

    public Entity invoker;
    public Entity owner;

    public int uses = 1;
    public delegate IEnumerator Generic_event(Alter alter = null);
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

    public IEnumerator Call_Event(CombatAction.Events eventIndex, Alter alter = null)
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

    public virtual IEnumerator Start_Event(Alter alter = null)
    {
        yield break;
    }
    public virtual IEnumerator End_Event(Alter alter = null)
    {
        yield break;
    }
}
public class Basic_Damage : Effect
{

    protected override void createNewInstanceStep2() //copy
    {
        
    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int damage = Mathf.RoundToInt(invoker.dmg * Random.Range(0.7f, 1.3f));
        Alter my_alter = new Alter(invoker, owner, -damage, 0, 0, 0, invoker.critic, invoker.hit);
        yield return invoker.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
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

    public IEnumerator AtRecieveAlter_Event(Alter alter = null)
    {
        if (alter.life < 0)
            alter.life = 0;
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

    public IEnumerator AtCauseAlter_Event(Alter alter = null)
    {
        alter.critical_chance = 100;
        yield break;
    }
}
public class Critical_100_2: Effect
{
    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int damage = Mathf.RoundToInt(invoker.dmg * Random.Range(0.7f, 1.3f));
        Alter my_alter = new Alter(invoker, owner, -damage, 0, 0, 0, 100, invoker.hit);
        yield return CombatManager.coroutiner.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
public class Basic_Heal : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int heal = Mathf.RoundToInt(4 * Random.Range(0.7f, 1.3f));
        Alter my_alter = new Alter(invoker, owner, heal, 0, 0, 0);
        yield return invoker.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
public class Big_Heal : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int heal = Mathf.RoundToInt(8 * Random.Range(0.7f, 1.3f));
        Alter my_alter = new Alter(invoker, owner, heal, 0, 0, 0);
        yield return invoker.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
public class Group_Damage : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int damage = Mathf.RoundToInt(5 * Random.Range(0.7f, 1.3f));
        Alter my_alter = new Alter(invoker, owner, -damage, 0, 0, 0);
        yield return invoker.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
public class Risky_Strike : Effect
{

    protected override void createNewInstanceStep2() //copy
    {

    }

    public override IEnumerator Start_Event(Alter alter = null)
    {
        int damage = Mathf.RoundToInt(invoker.dmg * Random.Range(9.0f, 10.5f));
        Alter my_alter = new Alter(invoker, owner, -damage, 0, 0, 0, 0, 10);
        yield return invoker.StartCoroutine(my_alter.ApplyManager()); //HAVE TO IMPROVE THIS
        yield break;
    }
}
