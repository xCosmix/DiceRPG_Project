using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI : MonoBehaviour {

    private Canvas canvas;
    private GameObject turnPanel, gameOverPanel, victoryPanel, levelUpPanel;
    private Button attack_button, escape_button, retry_button, continue_button, ready_button, cancel_button;
    private Button[] upgrades_buttons = new Button[3];
    private Graph_enemy[] enemiesPanels = new Graph_enemy[0];
    private Graph_player playerPanel;
    private bool[] canPayAP4card;
    private Text combat_text_ref, gold_text, exp_text, exp_remaining_text, levelUp_text;
    private Coroutine playerDisplay;
    public static Image action_panel;
    public static GUI instance;

    public class Graph_target : System.Object
    {
        public Entity ref_;
        public Image panel;
        public Text life, name, ap;
        public List<Graph_Action> actions = new List<Graph_Action>();

        public Graph_target (Entity target)
        {
            ref_ = target;
            ref_.guiRef = this;
        } 
        public virtual void Actualize ()
        {
            
        }
        public virtual void Select ()
        {
            
        }
        public virtual void Deselect ()
        {
            
        }
        public virtual void AddAction (string name)
        {

        }
        public virtual void RemoveAction (int pos)
        {
            actions[pos].Remove();
            actions.RemoveAt(pos);
        }
        public virtual void RemoveAll ()
        {
            for (int i = 0; i < actions.Count; i ++)
            {
                actions[i].Remove();
            }
            actions.Clear();
        }
        public void ShowHide (bool show)
        {
            panel.gameObject.SetActive(show);
        }
    }

    public class Graph_enemy : Graph_target
    {

        public static Image panel_pref;
        public static Vector2 fixedPos;
        public static Vector2 fixedDistance;
        public static Color fixedColor;

        public Graph_enemy(Entity target, int index) : base(target)
        {
            if (panel_pref == null)
            {
                panel_pref = GameObject.Find("Enemy_Panel").GetComponent<Image>();
                fixedPos = panel_pref.rectTransform.anchoredPosition;
                fixedDistance = panel_pref.rectTransform.sizeDelta * 1.1f;
                fixedColor = panel_pref.color;
                panel_pref.gameObject.SetActive(false);
            }
            panel = (Image)GUI.Duplicate(panel_pref);
            life = panel.transform.GetChild(0).GetComponent<Text>();
            name = panel.transform.GetChild(1).GetComponent<Text>();
            panel.rectTransform.anchoredPosition -= new Vector2(0.0f, fixedDistance.y * index);
            panel.gameObject.SetActive(true);
        }
        public override void Actualize()
        {
            life.text = "Life " + ref_.myInfo.stats.combat.hp + "/" + ref_.myInfo.stats.main.hp;
        }
        public override void Select()
        {
            base.Select();
            panel.color = new Color(1.0f, 1.0f, 1.0f);
        }
        public override void Deselect()
        {
            base.Deselect();
            panel.color = fixedColor;
        }
    }

    public class Graph_player : Graph_target
    {

        public Color fixedColor;

        public Graph_player (Entity target) : base (target)
        {
            panel = GameObject.Find("Player_panel").GetComponent<Image>();
            fixedColor = panel.color;
            ap = panel.transform.GetChild(1).GetComponent<Text>();
            life = panel.transform.GetChild(0).GetComponent<Text>();
        }
        public override void Actualize()
        {
            life.text = "Life " + ref_.myInfo.stats.combat.hp + "/" + ref_.myInfo.stats.main.hp;
            ap.text = "AP " + ref_.myInfo.stats.combat.ap + "/" + ref_.myInfo.stats.main.ap; 
        }
        public override void Select()
        {
            base.Select();
            panel.color = new Color(1.0f, 1.0f, 1.0f);
        }
        public override void Deselect()
        {
            base.Deselect();
            panel.color = fixedColor;
        }
        public override void AddAction(string name)
        {
            actions.Add(new Graph_Action(name, actions.Count));
        }
    }
    public class Graph_Action : System.Object
    {

        public Image panel;
        public Text text;
        public string name;

        public static Image panel_pref;
        public static Vector2 fixedPos;
        public static Vector2 fixedDistance;
        public static Color fixedColor;

        public Graph_Action(string name, int index) 
        {
            this.name = name;
            if (panel_pref == null)
            {
                panel_pref = GUI.action_panel;
                fixedPos = panel_pref.rectTransform.anchoredPosition;
                fixedDistance = panel_pref.rectTransform.sizeDelta * 1.1f;
                fixedColor = panel_pref.color;
                panel_pref.gameObject.SetActive(false);
            }
            panel = (Image)GUI.Duplicate(panel_pref);
            text = panel.transform.GetChild(0).GetComponent<Text>();
            text.text = name;

            panel.rectTransform.anchoredPosition -= new Vector2(0.0f, fixedDistance.y * index);
            panel.gameObject.SetActive(true);
        }
        public void ShowActive ()
        {
            panel.color = new Color(0.9f, 0.1f, 0.2f);
        }
        public void Remove ()
        {
            Destroy(panel.gameObject);
        }
    }
	// Use this for initialization
	void Start () {

        instance = FindObjectOfType<GUI>();

        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        action_panel = GameObject.Find("action_panel").GetComponent<Image>();
        combat_text_ref = GameObject.Find("Dmg_text").GetComponent<Text>();
        gold_text = GameObject.Find("gold_text").GetComponent<Text>();
        exp_text = GameObject.Find("exp_text").GetComponent<Text>();
        exp_remaining_text = GameObject.Find("expRemaining_text").GetComponent<Text>();
        levelUp_text = GameObject.Find("LevelUp_level").GetComponent<Text>();
        combat_text_ref.gameObject.SetActive(false);
        action_panel.gameObject.SetActive(false);

        attack_button = GameObject.Find("Attack_button").GetComponent<Button>();
       // escape_button = GameObject.Find("Escape_button").GetComponent<Button>();
        continue_button = GameObject.Find("Continue_button").GetComponent<Button>();
        retry_button = GameObject.Find("Retry_button").GetComponent<Button>();
        ready_button = GameObject.Find("Ready_button").GetComponent<Button>();
        cancel_button = GameObject.Find("cancel_button").GetComponent<Button>();
        turnPanel = GameObject.Find("Turn_panel");
        gameOverPanel = GameObject.Find("GameOver_text");
        victoryPanel = GameObject.Find("Loot_panel");
        levelUpPanel = GameObject.Find("LevelUp_panel");

        upgrades_buttons[0] = GameObject.Find("Upgrade_A").GetComponent<Button>();
        upgrades_buttons[1] = GameObject.Find("Upgrade_B").GetComponent<Button>();
        upgrades_buttons[2] = GameObject.Find("Upgrade_C").GetComponent<Button>();

        playerPanel = new Graph_player(Player.instance);

        canPayAP4card = new bool[3];

        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        turnPanel.SetActive(false);
        levelUpPanel.SetActive(false);
	}
    public void SetEnemiesPanels ()
    {
        enemiesPanels = new Graph_enemy[CombatManager.instance.enemies.Length];
        for (int i = 0; i < enemiesPanels.Length; i++)
        {
            enemiesPanels[i] = new Graph_enemy(CombatManager.instance.enemies[i], i);
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Actualize panels
        playerPanel.Actualize();

        foreach (Graph_enemy ep in enemiesPanels)
        {
            ep.Actualize();
        }
	}

    ///Summary 
    /// ap constraint applied to the player input (buttons / cards) 07/12/15
    private void AP_Constraints ()
    {
        if (CombatAction.library["Attack"].Can_PayAP(Player.instance))
            attack_button.interactable = true;
        else
            attack_button.interactable = false;

        for (int i = 0; i < Player.instance.current_hand.Count; i++)
        {
            string card = Player.instance.myInfo.deck[Player.instance.current_hand[i]];
            bool can_pay = CombatAction.library[Card.library[card].action].Can_PayAP(Player.instance); ///THE MOST INNEFICIENT SHIT EVER.
            canPayAP4card[i] = can_pay;
        }
    }
    private bool Ready_Input ()
    {
        if (!ready_button.interactable) return false;

        if (PressedButton(ready_button.image.rectTransform))
        {
            Player.instance.Call_Ready();
            return true;
        }
        return false;
    }
    public void Player_Turn (bool active)
    { 
        if (active)
        {
            StartCoroutine(Player_Display());
        }
        else
        {
            StopCoroutine(playerDisplay);
        }
    }
    public IEnumerator Player_Display ()
    {
        turnPanel.SetActive(true);
        GUI_Cards.instance.set_hide(false);

        yield return playerDisplay = StartCoroutine(Player_Turn_Input());

        GUI_Cards.instance.set_hide(true);
        turnPanel.SetActive(false);
    }
    public IEnumerator Player_Turn_Input ()
    {  
        while (true)
        {
            //Player GUI
            if (!Player.instance.Can_Ready())
                ready_button.interactable = false;
            else
                ready_button.interactable = true;

            AP_Constraints();

            if (!Player.instance.action_Selected()) ///evaluates if your next action is already selected
            {
                GUI_Cards.instance.Actualize();
                Action_Select();
            }
            else
            {
                Cancel_Button();
            }

            if (Ready_Input())
                break;

            yield return null;
        }
    }
    ///Summary 
    /// Actually this works for both
    private void Action_Select()
    {
        cancel_button.interactable = false; ///Dessapear cancel button while choosing next action
        ///for cards>
        bool containing = false;
        GUI_Cards.instance.set_hover(-1);

        for (int i = 0; i < Player.instance.current_hand.Count; i++)
        {
            containing = GUI_Cards.instance.MouseOver(i);

            if (!containing || !canPayAP4card[i]) continue;

            int card_index = Player.instance.current_hand[i];
            GUI_Cards.instance.set_hover(card_index);

            if (Input.GetMouseButtonDown(0))
            {
                string card = Player.instance.myInfo.deck[card_index];
                Player.instance.Call_ActionPick(Card.library[card].action, Player.instance.current_hand[i]);
                return;
            }
        }

        ///for basic attack>
        if (PressedButton(attack_button.image.rectTransform))
        {
            Player.instance.Call_ActionPick("Attack");
        }
    }
    private void Cancel_Button ()
    {
        cancel_button.interactable = true; ///appear cancel button after choosing action
        if (PressedButton(cancel_button.image.rectTransform))
        {
            Player.instance.Cancel_ActionPick();
        }
    }
    public void Victory (CombatManager.BattleReward reward)
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        victoryPanel.SetActive(true);

        StartCoroutine(Victory_Panel(reward));
    }
    public void Defeat ()
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        gameOverPanel.SetActive(true);
    }
    protected IEnumerator Victory_Panel (CombatManager.BattleReward reward)
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Count_Values(reward));
        ///GIVES REWARDS TO THE PLAYER
        CombatManager.instance.Rewards();

        while (true)
        {
            if (PressedButton(continue_button.image.rectTransform))
            {
                CombatSpawner.instance.EndCombat();
            }
            yield return null;
        }
    }
    protected IEnumerator Count_Values (CombatManager.BattleReward reward)
    {
        ///GOLD STEP
        float duration = 1.0f;
        int mountOfFrames = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        float valuePerFrame = ((float)(reward.gold) / (float)mountOfFrames);
        float currentValue = 0;
        int outValue = 0;
        int currentExp = Player.instance.myInfo.exp;

        gold_text.text = "Gold:" + 0;
        exp_remaining_text.text = "To next level: " + currentExp;
        exp_text.text = "Exp:" + 0;

        for (int f = mountOfFrames; f > 0; f--)
        {
            currentValue += valuePerFrame;
            outValue = Mathf.RoundToInt(currentValue);
            gold_text.text = "Gold:" + outValue;
            yield return new WaitForFixedUpdate();
        }
        currentValue = reward.gold;
        gold_text.text = "Gold:" + outValue;
        ///EXP STEP
        
        valuePerFrame = ((float)(reward.exp) / (float)mountOfFrames);
        currentValue = 0;
        outValue = 0;

        int initialExp = currentExp;
        int initialLevel = Player.instance.myInfo.level;
        int used = 0;

        for (int f = mountOfFrames; f > 0; f--)
        {
            currentValue += valuePerFrame;
            outValue = Mathf.RoundToInt(currentValue);
            currentExp = initialExp - (outValue - used);
            if (currentExp <= 0)
            {
                initialLevel++;
                yield return StartCoroutine(LevelUp(initialLevel));
                initialExp = ExpByLvl.get_exp2Level(initialLevel + 1);
                used = outValue;
            }

            exp_text.text = "Exp:" + outValue;
            exp_remaining_text.text = "To next level: " + currentExp;
            yield return new WaitForFixedUpdate();
        }
        currentValue = reward.exp;
        exp_text.text = "Exp:" + outValue;

        yield break;
    }
    protected IEnumerator LevelUp (int level)
    {
        victoryPanel.SetActive(false);
        levelUpPanel.SetActive(true);
        levelUp_text.text = level+"";
        string[] upgrades = Player.instance.BuffOptions();
        for (int i = 0; i < upgrades_buttons.Length; i++)
        {
            upgrades_buttons[i].GetComponentInChildren<Text>().text = upgrades[i];
        }
        bool break_ = false;
        while (!break_)
        {
            for (int i = 0; i < upgrades_buttons.Length; i++)
            {
                if (PressedButton(upgrades_buttons[i].image.rectTransform))
                {
                    Player.instance.AddBuff(upgrades, i);
                    break_ = true;
                    break;
                }
            }
            yield return null;
        }
        victoryPanel.SetActive(true);
        levelUpPanel.SetActive(false);
    }

    public static bool PressedButton (RectTransform rect)
    {
        bool containing = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, Camera.main);
        if (Input.GetMouseButtonDown(0) && containing)
        {
            return true;
        }
        return false;
    }
    public void Damage (int damage, Vector3 pos, bool critical)
    {
        StartCoroutine(DamageShow(damage, pos, critical));
    }
    public void Heal (int heal, Vector3 pos)
    {
        StartCoroutine(HealShow(heal, pos));
    }
    public void Miss (Vector3 pos)
    {
        StartCoroutine(MissShow(pos));
    }
    public IEnumerator DamageShow (int damage, Vector3 pos, bool critical)
    {
        System.Type[] textComps = new System.Type[] { typeof(Text) };
        Text tx = (Text)Duplicate(combat_text_ref);
        Vector3 pos_text = Camera.main.WorldToViewportPoint(pos);
        pos_text.z = 0.0f;
        tx.rectTransform.anchorMin = pos_text;
        tx.rectTransform.anchorMax = pos_text;
        tx.rectTransform.anchoredPosition = Vector3.zero;
        tx.rectTransform.localPosition = new Vector3(tx.rectTransform.localPosition.x, tx.rectTransform.localPosition.y, 0.0f);

        if (critical)
        {
            tx.fontSize = 50;
            tx.color = new Color(0.9f, 0.5f, 0.5f);
        }

        int randomStart = Random.Range(damage * 3, damage * 6);
        float duration = 0.5f;
        int mountOfFrames = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        float valuePerFrame = ((float)(damage - randomStart)/(float)mountOfFrames);
        float currentValue = randomStart;
        int outValue = randomStart;
        tx.text = ""+outValue;

        for (int f = mountOfFrames; f > 0; f--)
        {
            currentValue += valuePerFrame;
            outValue = Mathf.RoundToInt(currentValue);
            tx.text = "" + outValue;
            yield return new WaitForFixedUpdate();
        }
        currentValue = damage;
        tx.text = "" + currentValue;

        yield return new WaitForSeconds(0.5f);

        Destroy(tx.gameObject);
        yield break;

    }
    public IEnumerator HealShow(int life, Vector3 pos)
    {
        System.Type[] textComps = new System.Type[] { typeof(Text) };
        Text tx = (Text)Duplicate(combat_text_ref);
        Vector3 pos_text = Camera.main.WorldToViewportPoint(pos);
        pos_text.z = 0.0f;
        tx.rectTransform.anchorMin = pos_text;
        tx.rectTransform.anchorMax = pos_text;
        tx.rectTransform.anchoredPosition = Vector3.zero;
        tx.rectTransform.localPosition = new Vector3(tx.rectTransform.localPosition.x, tx.rectTransform.localPosition.y, 0.0f);
        tx.color = new Color(0.3f, 0.9f, 0.4f);

        int randomStart = Mathf.RoundToInt(Random.Range(life * 0.2f, life * 0.5f));
        float duration = 0.5f;
        int mountOfFrames = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        float valuePerFrame = ((float)(life - randomStart) / (float)mountOfFrames);
        float currentValue = randomStart;
        int outValue = randomStart;
        tx.text = "" + outValue;

        for (int f = mountOfFrames; f > 0; f--)
        {
            currentValue += valuePerFrame;
            outValue = Mathf.RoundToInt(currentValue);
            tx.text = "" + outValue;
            yield return new WaitForFixedUpdate();
        }
        currentValue = life;
        tx.text = "" + currentValue;

        yield return new WaitForSeconds(0.5f);

        Destroy(tx.gameObject);
        yield break;

    }
    public IEnumerator MissShow(Vector3 pos)
    {
        System.Type[] textComps = new System.Type[] { typeof(Text) };
        Text tx = (Text)Duplicate(combat_text_ref);
        Vector3 pos_text = Camera.main.WorldToViewportPoint(pos);
        pos_text.z = 0.0f;
        tx.rectTransform.anchorMin = pos_text;
        tx.rectTransform.anchorMax = pos_text;
        tx.rectTransform.anchoredPosition = Vector3.zero;

        tx.text = "miss!";

        yield return new WaitForSeconds(0.5f);

        Destroy(tx.gameObject);
        yield break;

    }

    public Vector2 worldSpace2CanvasSpace (Vector3 point, Canvas canvas)
    {
        //first you need the RectTransform component of your canvas
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(point);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        return WorldObject_ScreenPosition;
    }
    public static Graphic Duplicate (Graphic graph)
    {
        Graphic instance = Instantiate(graph);
        instance.transform.SetParent(graph.transform.parent);
        instance.rectTransform.rotation = graph.rectTransform.rotation;
        instance.rectTransform.localScale = graph.rectTransform.localScale;
        instance.rectTransform.anchoredPosition = graph.rectTransform.anchoredPosition;
        instance.rectTransform.localPosition = new Vector3(instance.rectTransform.localPosition.x, instance.rectTransform.localPosition.y, 0.0f);
        instance.gameObject.SetActive(true);
        return instance;
    }
}
