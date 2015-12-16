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
        [System.Serializable]
        public class Primitive : System.Object
        {
            public string name;
            public int value;
            public Primitive (string name, int value)
            {
                this.name = name;
                this.value = value;
            }
        }
        protected List<string> values_indexer = new List<string>()
        {
            hp_name,
            ap_name,
            dmg_name,
            def_name,
            hit_name,
            crit_name,
            counter_name
        };
        public Dictionary<string, int> values = new Dictionary<string, int>()
        {
            { hp_name, 0 },
            { ap_name, 0 },
            { dmg_name, 0 },
            { def_name, 0 },
            { hit_name, 0 },
            { crit_name, 0 },
            { counter_name, 0 }
        };

        public const string hp_name = "Life";
        public const string ap_name = "Mana";
        public const string dmg_name = "Damage";
        public const string def_name = "Defense";
        public const string hit_name = "Hit";
        public const string crit_name = "Critical";
        public const string counter_name = "Counter";

        public int hp
        {
            get { return values[hp_name]; }
            set { values[hp_name] = value; }
        }
        public int ap
        {
            get { return values[ap_name]; }
            set { values[ap_name] = value; }
        }
        public int dmg
        {
            get { return values[dmg_name]; }
            set { values[dmg_name] = value; }
        }
        public int def
        {
            get { return values[def_name]; }
            set { values[def_name] = value; }
        }
        public int hit
        {
            get { return values[hit_name]; }
            set { values[hit_name] = value; }
        }
        public int crit
        {
            get { return values[crit_name]; }
            set { values[crit_name] = value; }
        }
        public int counter
        {
            get { return values[counter_name]; }
            set { values[counter_name] = value; }
        }

        public Values (Dictionary<string, int> newValues = null)
        {
            foreach (string val in values_indexer)
            {
                if (newValues == null) break;

                if (newValues.ContainsKey(val))
                    values[val] = newValues[val];
                else
                    values[val] = 0;
            }
        }
        public Values (Values copy)
        {
            foreach (string val in values_indexer)
            {
                values[val] = copy.values[val];
            }
        }
        public Values(Primitive[] primitives)
        {
            for (int i = 0; i < primitives.Length; i++)
            {
                values[primitives[i].name] = primitives[i].value;
            }
        }
        public void Add (Values add)
        {
            foreach (string val in values_indexer)
            {
                values[val] += add.values[val];
            }
        }
        public void Clean ()
        {
            foreach (string val in values_indexer)
            {
                values[val] = 0;
            }
        }
        public string RefByIndex (int index)
        {
            return values_indexer[index];
        }
    }

    public Values.Primitive[] initial = new Values.Primitive[] {
        new Values.Primitive(Values.hp_name, 0),
        new Values.Primitive(Values.ap_name, 0),
        new Values.Primitive(Values.dmg_name, 0),
        new Values.Primitive(Values.def_name, 0),
        new Values.Primitive(Values.hit_name, 0),
        new Values.Primitive(Values.crit_name, 0),
        new Values.Primitive(Values.counter_name, 0),
    };

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
