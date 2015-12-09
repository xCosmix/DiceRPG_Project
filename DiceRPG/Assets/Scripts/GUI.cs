using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : MonoBehaviour {

    private Canvas canvas;
    private GameObject turnPanel, gameOverPanel, victoryPanel;
    private Button attack_button, escape_button, retry_button, continue_button, ready_button;
    private Graph_enemy[] enemiesPanels;
    private Graph_player playerPanel;
    private RectTransform[] cardsRef;
    private Text combat_text_ref;
    public static GUI instance;

    public class Graph_target : System.Object
    {
        public Entity ref_;
        public Image panel;
        public Text life, name, ap;

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
            life.text = "Life " + ref_.current_hp + "/" + ref_.hp;
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
            life.text = "Life " + ref_.current_hp + "/" + ref_.hp;
            ap.text = "AP " + ref_.current_ap + "/" + ref_.ap; 
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

	// Use this for initialization
	void Start () {

        instance = FindObjectOfType<GUI>();

        canvas = GetComponent<Canvas>();

        combat_text_ref = GameObject.Find("Dmg_text").GetComponent<Text>();
        combat_text_ref.gameObject.SetActive(false);

        attack_button = GameObject.Find("Attack_button").GetComponent<Button>();
       // escape_button = GameObject.Find("Escape_button").GetComponent<Button>();
        continue_button = GameObject.Find("Continue_button").GetComponent<Button>();
        retry_button = GameObject.Find("Retry_button").GetComponent<Button>();
        ready_button = GameObject.Find("Ready_button").GetComponent<Button>();
        turnPanel = GameObject.Find("Turn_panel");
        gameOverPanel = GameObject.Find("GameOver_text");
        victoryPanel = GameObject.Find("Victory_text");

        playerPanel = new Graph_player(Player.instance);

        cardsRef = new RectTransform[3];

        for (int i = 0; i < cardsRef.Length; i++)
        {
            cardsRef[i] = GameObject.Find((i + 1) + "_Card_Pos").GetComponent<RectTransform>() ;
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
        
        //Player GUI
        if (!Player.instance.Can_Ready())
            ready_button.interactable = false;
        else
            ready_button.interactable = true;

        AP_Constraints();

        if (true) ///evaluates if your next action is already selected
        {
            Action_Select();
        }
        else
        {
            ///cancel option and shit
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
    }

    ///Summary 
    /// Actually this works for both
    private void Action_Select()
    {
        ///for cards>
        bool containing = false;
        for (int i = 0; i < Player.instance.current_hand.Count; i++)
        {
            containing = RectTransformUtility.RectangleContainsScreenPoint(cardsRef[i], Input.mousePosition, Camera.main);

            if (!containing) continue;
            if (Input.GetMouseButtonDown(0))
            {
                Player.instance.Call_ActionPick(Card.library[Player.instance.current_hand[i]].action, Player.instance.current_hand[i]);
                return;
            }
        }

        ///for basic attack>
        containing = RectTransformUtility.RectangleContainsScreenPoint(attack_button.image.rectTransform, Input.mousePosition, Camera.main);
        if (Input.GetMouseButtonDown(0) && containing)
        {
            Player.instance.Call_ActionPick("Attack");
        }
    }
    public void Player_Turn (bool active)
    {
        turnPanel.SetActive(active);

        ///Summary    
        /// show / hide cards here 
        if (active)
        {
            //Debug.Log(Player.instance.current_hand.Count + " " + Player.instance.current_deck.Count);
            for (int i = 0; i < Player.instance.current_hand.Count; i++)
            {
                Image img = cardsRef[i].gameObject.AddComponent<Image>();
                img.sprite = Card.library[Player.instance.current_hand[i]].graphic;
                img.preserveAspect = true;
            }
        }
        else
        {
            for (int i = 0; i < cardsRef.Length; i++)
            {
                Image img = cardsRef[i].gameObject.GetComponent<Image>();
                if (img != null) Destroy(img);
            }
        }
    }
    public void Victory ()
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        victoryPanel.SetActive(true);
    }
    public void Defeat ()
    {
        Player_Turn(false);
        playerPanel.ShowHide(false);
        gameOverPanel.SetActive(true);
    }
    public void Damage (int damage, Vector3 pos, bool critical)
    {
        StartCoroutine(DamageShow(damage, pos, critical));
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
        instance.transform.SetParent(GUI.instance.gameObject.transform);
        instance.rectTransform.rotation = graph.rectTransform.rotation;
        instance.rectTransform.localScale = graph.rectTransform.localScale;
        instance.rectTransform.anchoredPosition = graph.rectTransform.anchoredPosition;
        instance.rectTransform.localPosition = new Vector3(instance.rectTransform.localPosition.x, instance.rectTransform.localPosition.y, 0.0f);
        instance.gameObject.SetActive(true);
        return instance;
    }
}
