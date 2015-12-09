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
    public string[] deck; //string cuz is a ref to the card library
    public Condition condition;
    public GUI.Graph_target guiRef;

    [HideInInspector]
    public int current_hp;
    [HideInInspector]
    public int current_ap;
   // [HideInInspector]
    public List<string> current_deck; //string cuz is a ref to the card library
   // [HideInInspector]
    public List<string> current_hand; //string cuz is a ref to the card library
    [HideInInspector]
    public bool dead;

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
    protected string current_card;
    protected Entity target;

	// Use this for initializatio
	void Start () {
        current_ap = ap;
        current_hp = hp;
        SetCombatDeckOrder();
        Custom_Start();
	}
	protected void SetCombatDeckOrder ()
    {
        List<int> deckPosition = new List<int>();
        for (int i = 0; i < deck.Length; i++)
        {
            deckPosition.Add(i);
        }
        current_deck = new List<string>();
        for (int i = 0; i < deck.Length; i++)
        {
            int randomPos = Random.Range(0, deckPosition.Count);
            int randomCard = deckPosition[randomPos];
            string card = deck[randomCard];
            current_deck.Add(card);
            deckPosition.Remove(randomCard);
        }
    }
    //Turn____________________________________________________________________________

    public IEnumerator Turn()
    {
        clean_actions();
        current_ap = ap; //setting everything up before start

        yield return StartCoroutine(Call_Event(CombatAction.Events.startTurn));
        inTurn = true;
        yield return StartCoroutine(CardDraw_Main());
        Custom_Turn();

        while (!ready)
        {
            Turn_On();
            ready = Ready();
            yield return null;
        }
        //Turn end!
        ready = false;
        yield return StartCoroutine(EndTurn());
        yield break;
    }

    public IEnumerator CardDraw_Main ()
    {
        if (current_hand == null || current_hand.Count == 0)
        {
            current_hand = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                DrawCard();
            }
        }
        if (current_hand.Count < 3)
        {
            DrawCard();
        }
        yield break;
    }
    public void DrawCard ()
    {
        if (current_deck.Count == 0) return;
        current_hand.Add(current_deck[current_deck.Count - 1]);
        current_deck.RemoveAt(current_deck.Count - 1);
    }
    public void Call_ActionPick (string action, string card = "")
    {
        StartCoroutine(ActionPick(action, card));
    }
    public IEnumerator ActionPick(string action, string card = "")
    {
        ///Add target for current action pick, Confirm after that
        current_action = action;
        current_card = card;
        target = null;

        while (target == null)
        {
            TargetChoose();
            yield return null;
        }

        ClosePick();
    }
    public void ClosePick()
    {
        ///Summary 
        /// CONFIRMS PICK
        /// determines what kind of combat bridge is going to use
        /// **
        if (current_action == "Attack")
            new Attack(current_action, this, target);
        else
            new CardCall(current_action, this, target);

        CleanPick();
    }
    public void CleanPick()
    {
        ///clean shit
        if (current_card != "")
        {
            current_hand.Remove(current_card);
        }
        current_action = "";
        current_card = "";
    }

    public virtual void TargetChoose(){
        /// <summary>
        /// Custom Target choice depends on every entity
        /// </summary>
    }
    public virtual bool Ready ()
    {
        ///<summary>
        /// Custom for every entity, ready: decides whether a battler wants to end his turn or continue it
        /// </summary>
        return false;
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
        dead = true;
        Graph_remove();
        yield break;
    }
    public virtual void Graph_remove ()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
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

    /// <summary>
    /// Global deff of call event to evaluate iof the target still exist
    /// </summary>
    public static IEnumerator Call_Event(Entity at, CombatAction.Events event_, Alter alter = null)
    {
        if (!at.dead) yield break;
        yield return at.StartCoroutine(at.Call_Event(event_, alter));
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
