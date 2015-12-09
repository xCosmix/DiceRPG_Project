using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stats : System.Object
{
    public int
        hp,
        ap,
        dmg,
        def,
        hit,
        crit,
        counter = 0;
    public Entity.Condition condition;
    public Stats staticStats;

    public Stats(int hp = 0, int ap = 0, int dmg = 0, int def = 0, int hit = 0, int crit = 0, int counter = 0, Entity.Condition condition = Entity.Condition.healthy, Stats staticStats = null)
    {
        this.hp = hp;
        this.ap = ap;
        this.dmg = dmg;
        this.def = def;
        this.hit = hit;
        this.crit = crit;
        this.counter = counter;
        this.condition = condition;
        this.staticStats = staticStats;
    }
    public Stats(Stats copy)
    {
        hp = copy.hp;
        ap = copy.ap;
        dmg = copy.dmg;
        def = copy.def;
        hit = copy.hit;
        crit = copy.crit;
        counter = copy.counter;
        condition = copy.condition;
        //COPYY && LINK!!!!!!!!!!!!!
        staticStats = copy;
    }
    /// <summary>
    /// Alteration functions
    /// </summary>
    public IEnumerator TryRecieve(Changer changer)
    {

        if (changer.hitted)
        {
            yield return changer.target.StartCoroutine(Recieve(changer));
        }
        else
        {
            yield return changer.target.StartCoroutine(Miss(changer));
        }

        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(changer.dealer, CombatAction.Events.afterCauseAlter, changer));
        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(changer.target, CombatAction.Events.afterRecieveAlter, changer));

        yield return CombatManager.coroutiner.StartCoroutine(Counter(changer));
    }

    public IEnumerator Miss(Changer changer)
    {
        GUI.instance.Miss(changer.target.transform.position);
        yield return null;
        //Empty for now
    }
    public IEnumerator Recieve(Changer changer)
    {
        Stats change = changer.adds;
        if (change.hp < 0) { yield return changer.target.StartCoroutine(Damage(changer)); }
        if (change.hp > 0) { yield return changer.target.StartCoroutine(Heal(changer)); }
        ///Do shit with all the rest of variables
        yield break;
    }
    public IEnumerator Counter (Changer changer) //THIS IS PROVISORIAL, COUNTER ABBILITY WILL BE AN CONSTANT EFFECT AT ENTITIES
    {
        bool counter_ = counter >= Random.Range(1, 101);
        if (!counter_) yield break;

        CombatBridge count = new Attack("Attack", changer.target, changer.dealer);
        yield return changer.target.StartCoroutine(count.Act());
    }

    public IEnumerator Damage(Changer changer)
    {
        int damage = changer.adds.hp * -1; //Change it to possitive for opperations
        int finalDef = Mathf.RoundToInt(def * Random.Range(0.5f, 1.3f));
        
        int finalBoost = 0; //HAVE TO CALCULATE ADDS TO DAMAGE DEPENDING ON ELEMENT WEAKNESS
        int finaldamage = finalBoost + damage - finalDef;
        finaldamage = finaldamage < 0 ? 0 : finaldamage;

        hp -= finaldamage;
        hp = hp < 0 ? 0 : hp;

        ///Anim step
        yield return changer.target.StartCoroutine(Damage_Animations(changer.target, changer.dealer, finaldamage, changer.critical));
    }
    public IEnumerator Damage_Animations(Entity target, Entity dealer, int damage, bool critical)
    {
        GUI.instance.Damage(damage, target.transform.position, critical);
        yield return target.StartCoroutine(target.Call_Event(CombatAction.Events.recieveDamage));

        if (hp == 0)
        {
            yield return target.StartCoroutine(target.Die());
            yield break;
        }

        yield return target.StartCoroutine(target.Animation_Hitted());
        yield return dealer.StartCoroutine(dealer.Call_Event(CombatAction.Events.dealDamage));

        yield break;
    }
    public IEnumerator Heal(Changer changer)
    {
        hp += changer.adds.hp;
        hp = hp > staticStats.hp ? staticStats.hp : hp;

        ///Anim step
        yield return changer.target.StartCoroutine(Heal_Animations(changer.target, changer.dealer, changer.adds.hp));
    }
    public IEnumerator Heal_Animations(Entity target, Entity dealer, int heal)
    {
        GUI.instance.Heal(heal, target.transform.position);
        yield break;
    }
}
