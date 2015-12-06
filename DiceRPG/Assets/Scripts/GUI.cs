using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : MonoBehaviour {

    private Canvas canvas;
    private GameObject turnPanel, gameOverPanel, victoryPanel;
    private Button attack_button, escape_button, retry_button, continue_button, ready_button;
    private Graph_enemy[] enemiesPanels;
    private Graph_player playerPanel;
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

        public Image panel_pref;
        public Vector2 fixedPos;
        public Vector2 fixedDistance;
        public Color fixedColor;

        public Graph_enemy(Entity target, int index) : base(target)
        {
            if (panel_pref == null)
            {
                panel_pref = Resources.Load<Image>("GUI/Enemy_panel");
                fixedPos = panel_pref.rectTransform.anchoredPosition;
                fixedDistance = panel_pref.rectTransform.sizeDelta * 1.1f;
                fixedColor = panel_pref.color;

            }
            panel = Instantiate(panel_pref); panel.transform.SetParent(GUI.instance.transform);
            panel.rectTransform.sizeDelta = panel_pref.rectTransform.sizeDelta;
            panel.rectTransform.anchoredPosition = panel_pref.rectTransform.anchoredPosition;
            life = panel.transform.GetChild(0).GetComponent<Text>();
            name = panel.transform.GetChild(1).GetComponent<Text>();
            panel.rectTransform.anchoredPosition -= new Vector2(0.0f, fixedDistance.y * index);
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
        attack_button = GameObject.Find("Attack_button").GetComponent<Button>();
       // escape_button = GameObject.Find("Escape_button").GetComponent<Button>();
        continue_button = GameObject.Find("Continue_button").GetComponent<Button>();
        retry_button = GameObject.Find("Retry_button").GetComponent<Button>();
        ready_button = GameObject.Find("Ready_button").GetComponent<Button>();
        turnPanel = GameObject.Find("Turn_panel");
        gameOverPanel = GameObject.Find("GameOver_text");
        victoryPanel = GameObject.Find("Victory_text");

        playerPanel = new Graph_player(Player.instance);

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

        if (CombatAction.library["Attack"].Can_PayAP(Player.instance))
            attack_button.interactable = true;
        else
            attack_button.interactable = false;
	}
    public void Player_Turn (bool active)
    {
        turnPanel.SetActive(active);
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
        Text tx = Instantiate(Resources.Load<Text>("GUI/Dmg_text"));
        tx.transform.SetParent(this.gameObject.transform);
        tx.rectTransform.anchoredPosition = worldSpace2CanvasSpace(pos, canvas);

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
        Text tx = Instantiate(Resources.Load<Text>("GUI/Dmg_text"));
        tx.transform.SetParent(this.gameObject.transform);
        tx.rectTransform.anchoredPosition = worldSpace2CanvasSpace(pos, canvas);

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
}
