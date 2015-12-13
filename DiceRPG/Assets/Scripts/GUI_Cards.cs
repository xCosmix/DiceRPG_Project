using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUI_Cards : MonoBehaviour {

    public static GUI_Cards instance;

    private float speed = 10.0f;
    private GameObject card_ref;
    private GameObject card_delete_ref;
    private Transform[] cardsRef;
    private RectTransform[] cardsBounds;

    private List<int> player_hand = new List<int>();
    private List<int> player_handLocal = new List<int>();/// <summary>
                                                         /// ref to local dynamic list from player hand
                                                         /// </summary>
    private int currentHover;
    private bool hide;

    private Dictionary<int, MeshRenderer> cardGraphic = new Dictionary<int, MeshRenderer>();
    private Dictionary<int, int> cardPositions = new Dictionary<int, int>(); /// <summary>
                                                                             /// ref to card ref position in index
                                                                             /// </summary>
    private Coroutine process;
   
    
    // Use this for initialization
    void Start () {

        instance = GameObject.FindObjectOfType<GUI_Cards>();

        cardsRef = new Transform[5];
        for (int i = 0; i < cardsRef.Length; i++)
        {
            cardsRef[i] = GameObject.Find((i + 1) + "_Card_Pos").transform;
        }

        cardsBounds = new RectTransform[5];
        for (int i = 0; i < cardsRef.Length; i++)
        {
            cardsBounds[i] = GameObject.Find((i + 1) + "_Card_Rect").GetComponent<RectTransform>();
        }

        card_ref = GameObject.Find("card_ref");
        card_delete_ref = GameObject.Find("card_delete_ref");
    }
	public void Actualize()
    {
        player_hand = Player.instance.current_hand;
        if (process != null) return;
        
        if (player_hand.Count > player_handLocal.Count) process = StartCoroutine(Draw());
        else if (player_hand.Count < player_handLocal.Count) process = StartCoroutine(Remove());
    }
    void Update ()
    {
        for (int i = 0; i < cardPositions.Count; i++)
        {
            if (i >= cardPositions.Count) continue;

            int card = player_handLocal[i];
            Move(cardsRef[cardPositions[card]], cardGraphic[card].transform, currentHover == card);
        }
        ///NO MOTION FOR REMOVING CARD YET
    }
    private IEnumerator Draw ()
    {
        for (int i = cardPositions.Count; i < player_hand.Count; i++)
        {
            yield return StartCoroutine(DrawIndividual(i));
        }
        process = null;
    }
    private IEnumerator DrawIndividual (int i)
    {
        GameObject card_instance = Instantiate(card_ref, card_ref.transform.position, card_ref.transform.rotation) as GameObject;
        MeshRenderer card_mesh = card_instance.GetComponent<MeshRenderer>();

        card_instance.transform.SetParent(card_ref.transform.parent);
        card_instance.transform.localScale = card_ref.transform.localScale;
        card_mesh.material.mainTexture = Card.library[Player.instance.myInfo.deck[player_hand[i]]].graphic;

        cardGraphic.Add(player_hand[i], card_mesh);
        player_handLocal.Add(player_hand[i]);
        SortCards();

        yield return new WaitForSeconds(0.3f);
    }
    private IEnumerator Remove ()
    {
        for (int i = 0; i < player_handLocal.Count; i++)
        {
            int card = player_handLocal[i];
            if (player_hand.Contains(card)) continue;
            yield return StartCoroutine(RemoveIndividual(card));
        }
        process = null;
    }
    private IEnumerator RemoveIndividual (int card)
    {
        cardPositions.Remove(card);
        player_handLocal.Remove(card);

        SortCards();

        yield return new WaitForSeconds(0.3f);

        Destroy(cardGraphic[card].gameObject);
        cardGraphic.Remove(card);
    }
    /// <summary>
    /// sort is called after gui_cards && player_current hand are equals
    /// </summary>
    private void SortCards ()
    {
        switch (player_handLocal.Count)
        {
            case 1:
                MoveCard2Pos(1, 0);
                break;
            case 2:
                MoveCard2Pos(4, 0);
                MoveCard2Pos(3, 1);
                break;
            case 3:
                MoveCard2Pos(0, 0);
                MoveCard2Pos(1, 1);
                MoveCard2Pos(2, 2);
                break;
        }
    }
    private void MoveCard2Pos (int card_ref_index, int card_index)
    {
        if (card_index+1 > cardPositions.Count)
        {
            cardPositions.Add(player_handLocal[card_index], card_ref_index);
        }
        else
        {
            cardPositions[player_handLocal[card_index]] = card_ref_index;
        }
    }
    private void Move (Transform target, Transform origin, bool hover)
    {
        Vector3 alteration = hide ? new Vector3(0.0f, -300.0f, 0.0f) : hover ? new Vector3(0.0f, +60.0f, 0.0f) : new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 delta = (target.localPosition+alteration) - origin.localPosition;
        origin.localPosition += delta * Time.deltaTime * speed;
        origin.transform.rotation = Quaternion.Slerp(origin.rotation, target.rotation, Time.deltaTime*speed);
    }
    public bool MouseOver (int cardIndex)
    {
        if (cardIndex + 1 > cardPositions.Count) return false; ///return false while cards are loading at the coroutine above

        int card = player_hand[cardIndex];
        RectTransform myRect = cardsBounds[cardPositions[card]];
        return RectTransformUtility.RectangleContainsScreenPoint(myRect, Input.mousePosition, Camera.main);
    }

    /// <summary>
    /// Setters and getters
    /// </summary>
    public void set_hide (bool hide)
    {
        this.hide = hide;
    }
    public void set_hover (int hover)
    {
        this.currentHover = hover;
    }
}
