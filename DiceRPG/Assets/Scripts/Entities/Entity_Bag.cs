using UnityEngine;
using System.Collections.Generic;

public class Entity_Bag : MonoBehaviour {

    public List<string> amulets = new List<string>();
    public List<string> cards = new List<string>();

    public void ObtainCard (string card)
    {
        cards.Add(card);
    }
    public void ObtainAmulet (string amulet)
    {
        amulets.Add(amulet);
    }
}
