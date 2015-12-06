using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

    public new string name;
    public int hp;
    public int ap;
    public int dmg;
    public int critic;
    public int hit;
    public int level;
    public int exp;
    public int gold;
    public Condition condition;
    public GUI.Graph_target guiRef;

    public int current_hp;
    public int current_ap;

    public enum Condition { healthy, poisoned, exhausted, silence, doomed }
    public enum Element { none, fire, earth, water, wind }
    public enum AttackType { normal, spell }

    private bool inTurn;
    private bool targetable = true;
    protected bool ready = false;

    protected List<CombatBridge> actions = new List<CombatBridge>();
    protected List<Effect> active_effects = new List<Effect>();
    protected List<Effect> temp_removeEffects = new List<Effect>();

    protected bool calling_events;
    protected string current_action;

	// Use this for initializatio
	void Start () {
        current_ap = ap;
        current_hp = hp;
        Custom_Start();
	}
	
    //Turn____________________________________________________________________________

    public IEnumerator Turn()
    {
        clean_actions();
        current_ap = ap; //setting everything up before start

        yield return StartCoroutine(Call_Event(CombatAction.Events.startTurn));
        inTurn = true;
        Custom_Turn();

        while (!ready)
        {
            Turn_On();
            yield return null;
        }
        //Turn end!
        ready = false;
        yield return StartCoroutine(EndTurn());
        yield break;
    }
    public IEnumerator EndTurn()
    {
        yield return StartCoroutine(Call_Event(CombatAction.Events.endTurn));
        inTurn = false;
        Custom_EndTurn();
        yield return null;
    }
    //Animations____________________________________________________________________________

    public virtual IEnumerator Animation_Attack ()
    {
        yield break;
    }
    public virtual IEnumerator Animation_Card ()
    {
        yield break;
    }
    public virtual IEnumerator Animation_Hitted ()
    {
        yield break;
    }
    public virtual IEnumerator Animation_Die ()
    {
        yield break;
    }
    //Combat____________________________________________________________________________

    public IEnumerator Alter (Alter alteration)
    {
        //apply all shit
        bool hitted = alteration.hit_chance >= Random.Range(1, 101);
        bool critical = alteration.critical_chance >= Random.Range(1, 101);

        if (!hitted)
        {
            GUI.instance.Miss(transform.position);
            yield break;
        }
        if (critical)
        {
            alteration.life = Mathf.RoundToInt(alteration.life * Random.Range(2.0f, 3.0f));
        }

        if (alteration.life < 0)
        {
            yield return StartCoroutine(RecieveDamage(alteration.dealer, alteration.life, alteration.element, alteration.attackType, critical));
        }
        //APPLY ALL THE REST FIELDS OF THE STATCHANGE
        yield break;
    }
    public IEnumerator RecieveDamage (Entity dealer, int damage, Element e, AttackType at, bool critical)
    {
        current_hp += damage;
        if (current_hp < 0) current_hp = 0;

        GUI.instance.Damage(-damage, transform.position, critical);
        yield return StartCoroutine(Call_Event(CombatAction.Events.recieveDamage));

        if (current_hp <= 0)
        {
            yield return StartCoroutine(Die());
            yield break;
        }
        yield return StartCoroutine(Animation_Hitted());

        yield return dealer.StartCoroutine(dealer.Call_Event(CombatAction.Events.dealDamage));

        yield break;
    }
    public IEnumerator Die ()
    {
        yield return StartCoroutine(Call_Event(CombatAction.Events.dead));
        yield return StartCoroutine(Animation_Die());
        Custom_Dead();
        Destroy(gameObject);
        yield break;
    }

    public bool CanTarget(Entity targ)
    {
        return targ.get_targetable();
    }

    //CUSTOM METHODS______________________________________

    public virtual void Custom_Start()
    {
        //override
    }
    public virtual void Custom_Dead ()
    {
        //override
    }
    public virtual void Custom_Turn()
    {
        //override
    }
    public virtual void Custom_EndTurn()
    {
        //override
    }

    public virtual void Turn_On()
    {
        //override
    }
    public bool CanPickTurn()
    {
        //override
        return true;
    }

    //Events_____________________________________________________________________

    public virtual IEnumerator Add_Effect (Effect eff)
    {
        active_effects.Add(eff);
        yield return StartCoroutine(eff.Call_Event(CombatAction.Events.start)); //UNIQUE EVENT
    }
    public virtual IEnumerator Remove_Effect (Effect eff)
    {
        yield return StartCoroutine(eff.Call_Event(CombatAction.Events.end)); //UNIQUE EVENT
        temp_removeEffects.Add(eff);
    }
    public virtual IEnumerator WaitForEnd_Events ()
    {
        while (calling_events) { yield return null; } //Wait for current event call to e
    }
    public virtual IEnumerator Call_Event(CombatAction.Events event_, Alter alter = null) 
    {
        calling_events = true;
        foreach (Effect ef in active_effects)
        {
            yield return StartCoroutine(ef.Call_Event(event_, alter));
        }
        calling_events = false;
        UpdateList();
        yield break;
    }
    public void UpdateList ()
    {
        foreach (Effect eff in temp_removeEffects)
        {
            if (active_effects.Contains(eff))
                active_effects.Remove(eff);
        }
        temp_removeEffects.Clear();
    }
   //Getters & Setters__________________________________________________________

    public bool get_inTurn () { return inTurn; }
    public bool get_targetable () { return targetable; }
    public List<CombatBridge> get_actions () { return actions; }
    public void add_action (CombatBridge action) { actions.Add(action); }
    public void clean_actions() { actions = new List<CombatBridge>(); }
    /*
    public void add_effect (Effect effect) { active_effects.Add(effect); }
    public void remove_effect(Effect effect) {
        temp_removeEffects.Add(effect); } //REMOVE TO TEMP 
        */
}
