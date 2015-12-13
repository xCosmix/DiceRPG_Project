using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// THIS IS THE COMBAT ENTITY 
/// </summary>
public class Entity : MonoBehaviour {

    public Entity_Info myInfo;

    private bool inTurn;
    protected bool dead = false;
    private bool targetable = true;
    protected bool ready = false;

    public List<int> current_deck; //string cuz is a ref to the card library
    public List<int> current_hand; //string cuz is a ref to the card library
    [HideInInspector]
    public Stats battle_stats;

    protected List<CombatBridge> actions = new List<CombatBridge>();
    protected List<Effect> active_effects = new List<Effect>();
    protected List<Effect> temp_removeEffects = new List<Effect>();

    protected bool calling_events;
    protected string current_action;
    protected int current_card;
    protected Entity target;

    protected Coroutine actionPick;

    public GUI.Graph_target guiRef;

    // Use this for initializatio
    public virtual void CombatStart () {
        myInfo = GetComponent<Entity_Info>();
        battle_stats = new Stats(myInfo.stats);
        SetCombatDeckOrder();
        Custom_Start();
	}
	protected void SetCombatDeckOrder ()
    {
        List<int> deckPosition = new List<int>();
        for (int i = 0; i < myInfo.deck.Length; i++)
        {
            deckPosition.Add(i);
        }

        current_deck = new List<int>();
        current_hand = new List<int>();

        for (int i = 0; i < myInfo.deck.Length; i++)
        {
            int randomPos = Random.Range(0, deckPosition.Count);
            int randomCard = deckPosition[randomPos];
            string card = myInfo.deck[randomCard];
            current_deck.Add(randomCard);
            deckPosition.Remove(randomCard);
        }
    }
    //Turn____________________________________________________________________________

    public IEnumerator Turn()
    {
        clean_actions();
        battle_stats.ap = myInfo.stats.ap; //setting everything up before start

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
            current_hand = new List<int>();
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

    public void Call_ActionPick (string action, int card = -1)
    {
        actionPick = StartCoroutine(ActionPick(action, card));
    }
    public void Cancel_ActionPick ()
    {
        StopCoroutine(actionPick);
        ///reset selection
        current_action = "";
        current_card = -1;
    }

    public IEnumerator ActionPick(string action, int card = -1)
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
        if (current_card != -1)
        {
            current_hand.Remove(current_card);
        }
        current_action = "";
        current_card = -1;
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
    public virtual IEnumerator Call_Event(CombatAction.Events event_, Changer alter = null) 
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
    public static IEnumerator Call_Event(Entity at, CombatAction.Events event_, Changer alter = null)
    {
        if (at.dead) yield break;
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
    public void clean_actions() { actions = new List<CombatBridge>(); guiRef.RemoveAll(); } /// <summary>
    /// Cleans local list and info of the actions chosen at the last turn at your graphic panel info
    /// </summary>
    /// <returns></returns>
    public bool get_dead () { return dead; }
    public bool action_Selected () { return current_action != "" && current_action != null; }
    /*
    public void add_effect (Effect effect) { active_effects.Add(effect); }
    public void remove_effect(Effect effect) {
        temp_removeEffects.Add(effect); } //REMOVE TO TEMP 
        */
}
