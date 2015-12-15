using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stats : System.Object
{

   // public enum Condition { healthy, poisoned, exhausted, silence, doomed }
    [System.Serializable]
    public class Values : System.Object
    {
        public int
            hp,
            ap,
            dmg,
            def,
            hit,
            crit,
            counter = 0;

        public Values (int hp = 0, int ap = 0, int dmg = 0, int def = 0, int hit = 0, int crit = 0, int counter = 0)
        {
            this.hp = hp;
            this.ap = ap;
            this.dmg = dmg;
            this.def = def;
            this.hit = hit;
            this.crit = crit;
            this.counter = counter;
        }
        public Values (Values copy)
        {
            hp = copy.hp;
            ap = copy.ap;
            dmg = copy.dmg;
            def = copy.def;
            hit = copy.hit;
            crit = copy.crit;
            counter = copy.counter;
        }
        public void Add (Values add)
        {
            hp += add.hp;
            ap += add.ap;
            dmg += add.dmg;
            def += add.def;
            hit += add.hit;
            crit += add.crit;
            counter += add.counter;
        }
        public void Clean ()
        {
            hp = 0;
            ap = 0;
            dmg = 0;
            def = 0;
            hit = 0;
            crit = 0;
            counter = 0;
        }
    }

    public Values initial;

    /// <summary>
    /// ONLY READABLE
    /// </summary>
    [HideInInspector]
    public Values combat
    {
        get { return Combat(); }
        private set { }
    }
    [HideInInspector]
    public Values main
    {
        get { return Main(); }
        private set { }
    }
    private Values Main()
    {
        if (!values.ContainsKey("Default")) Add("Default", new Values(initial));
        if (!values.ContainsKey("Main")) Add("Main", new Values());
        values["Main"].Clean();

        for (int i = 0; i < values.Count; i++)
        {
            KeyValuePair<string, Values> entry = new KeyValuePair<string, Values>(references[i], values[references[i]]);

            if (entry.Key == "Main" || entry.Key == "Receptor" || entry.Key == "Combat" || entry.Key == "Emissor") continue;
            values["Main"].Add(entry.Value);
        }
        return values["Main"];
    }
    private Values Combat()
    {
        if (!values.ContainsKey("Combat")) Add("Combat", new Values());
        values["Combat"].Clean();

        for (int i = 0; i < values.Count; i++)
        {
            KeyValuePair<string, Values> entry = new KeyValuePair<string, Values>(references[i], values[references[i]]);

            if (entry.Key == "Combat" || entry.Key == "Main") continue;
            values["Combat"].Add(entry.Value); 
        }
        return values["Combat"];
    }
    /// <summary>
    /// ONLY READABLE
    /// </summary>

    protected Dictionary<string, Values> values = new Dictionary<string, Values>();
    protected List<string> references = new List<string>();

    /// <summary>
    /// TYPES OF VALUES AGRUPATIONS
    /// </summary>
    /// <returns></returns>
    
    public void Add(string key, Values add)
    {
        if (references.Contains(key)) return;
        references.Add(key);
        values.Add(key, add);
    }
    public void Remove(string key)
    {
        if (!references.Contains(key)) return;
        references.Remove(key);
        values.Remove(key);
    }
    public Values GetValues(string key)
    {
        if (!values.ContainsKey(key))
                Add(key, new Values());
        return values[key];
    }
    public void RemoveAllCombat ()
    {
        Remove("Combat");
        Remove("Emissor");
        Remove("Receptor");
    }
    /// <summary>
    /// END

    public void Calculate(Values input)
    {
        if (input.hp < 0)
        {
            int damage = input.hp;
            int def = Mathf.RoundToInt(combat.def * Random.Range(0.5f, 1.3f));

            int finalDamage = damage + def;
            input.hp = finalDamage;
        }
        if (input.hp > 0)
        {
            ///no shit here
        }
    }
    public void BoundariesCorrection (Values input)
    {
        int receptor = GetValues("Receptor").hp;
        int limitA = -main.hp - receptor;
        int limitB = -receptor;
        input.hp = Mathf.Clamp(input.hp, limitA, limitB);
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
        Values original = new Values(changer.adds);
        Values change = changer.adds;
        
        if (!changer.ignoreCalculate)
            Calculate(changer.adds);

        BoundariesCorrection(changer.adds);

        int healOrDamage = original.hp > 0 ? 1 : original.hp < 0 ? 2 : 0;

        GetValues("Receptor").Add(change);

        if (healOrDamage == 2) { yield return changer.target.StartCoroutine(Damage(changer)); }
        if (healOrDamage == 1) { yield return changer.target.StartCoroutine(Heal(changer)); }
        ///Do shit with all the rest of variables
        yield break;
    }
    
    public IEnumerator Counter (Changer changer) //THIS IS PROVISORIAL, COUNTER ABBILITY WILL BE AN CONSTANT EFFECT AT ENTITIES
    {
        bool counter_ = combat.counter >= Random.Range(1, 101);
        if (!counter_) yield break;

        CombatBridge count = new Attack("Attack", changer.target, changer.dealer);
        yield return changer.target.StartCoroutine(count.Act());
    }

    public IEnumerator Damage(Changer changer)
    {
        ///Anim step
        yield return changer.target.StartCoroutine(Damage_Animations(changer.target, changer.dealer, changer.adds.hp, changer.critical));
    }
    public IEnumerator Damage_Animations(Entity target, Entity dealer, int damage, bool critical)
    {
        GUI.instance.Damage(-damage, target.transform.position, critical);
        yield return target.StartCoroutine(target.Call_Event(CombatAction.Events.recieveDamage));

        if (combat.hp == 0)
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
        ///Anim step
        yield return changer.target.StartCoroutine(Heal_Animations(changer.target, changer.dealer, changer.adds.hp));
    }
    public IEnumerator Heal_Animations(Entity target, Entity dealer, int heal)
    {
        GUI.instance.Heal(heal, target.transform.position);
        yield break;
    }
}
