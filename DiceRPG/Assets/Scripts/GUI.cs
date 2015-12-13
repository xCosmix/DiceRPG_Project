using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI : MonoBehaviour {

    private Canvas canvas;
    private GameObject turnPanel, gameOverPanel, victoryPanel;
    private Button attack_button, escape_button, retry_button, continue_button, ready_button, cancel_button;
    private Graph_enemy[] enemiesPanels = new Graph_enemy[0];
    private Graph_player playerPanel;
    private Image[] cardsRef;
    private bool[] canPayAP4card;
    private Text combat_text_ref;
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
            life.text = "Life " + ref_.battle_stats.hp + "/" + ref_.myInfo.stats.hp;
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
            life.text = "Life " + ref_.battle_stats.hp + "/" + ref_.myInfo.stats.hp;
            ap.text = "AP " + ref_.battle_stats.ap + "/" + ref_.myInfo.stats.ap; 
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
        victoryPanel = GameObject.Find("Victory_text");

        playerPanel = new Graph_player(Player.instance);

        cardsRef = new Image[3];
        canPayAP4card = new bool[3];

        for (int i = 0; i < cardsRef.Length; i++)
        {
            cardsRef[i] = GameObject.Find((i + 1) + "_Card_Pos").GetComponent<Image>() ;
        }

        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        turnPanel.SetActive(false);
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
            if (!can_pay)
                cardsRef[i].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
            else
                cardsRef[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
        //Debug.Log(Player.instance.current_hand.Count + " " + Player.instance.current_deck.Count);
        for (int i = 0; i < Player.instance.current_hand.Count; i++)
        {
            Image img = cardsRef[i];
            img.enabled = true;
            string card = Player.instance.myInfo.deck[Player.instance.current_hand[i]];
            img.sprite = Card.library[card].graphic;
            img.preserveAspect = true;
        }

        yield return playerDisplay = StartCoroutine(Player_Turn_Input());

        for (int i = 0; i < cardsRef.Length; i++)
        {
            Image img = cardsRef[i];
            img.enabled = false;
        }
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
        for (int i = 0; i < Player.instance.current_hand.Count; i++)
        {
            containing = RectTransformUtility.RectangleContainsScreenPoint(cardsRef[i].rectTransform, Input.mousePosition, Camera.main);

            if (!containing || !canPayAP4card[i]) continue;
            if (Input.GetMouseButtonDown(0))
            {
                string card = Player.instance.myInfo.deck[Player.instance.current_hand[i]];
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
    public void Victory ()
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        victoryPanel.SetActive(true);

        StartCoroutine(Victory_Panel());
    }
    public void Defeat ()
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        gameOverPanel.SetActive(true);
    }
    protected IEnumerator Victory_Panel ()
    {
        while (true)
        {
            if (PressedButton(continue_button.image.rectTransform))
            {
                CombatSpawner.instance.EndCombat();
            }
            yield return null;
        }
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
