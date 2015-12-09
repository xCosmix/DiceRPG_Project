using UnityEngine;
using System.Collections.Generic;

public class Card : System.Object {

    public string name = "none"; //Name of the card
    public string action = "none"; // Key from global dictionary at combatAction
    public int cardCost = 100; //gold cost
    public int oddness = 0; // scale from 1 to 10
    public CardType type = CardType.Offensive; //type of card
    public Sprite graphic = null; //Card graphic 

    public enum CardType { Offensive, Defensive, Assist};

    public Card (string name, string action, int cardCost, int oddness, CardType type)
    {
        this.name = name;
        this.action = action;
        this.cardCost = cardCost; 
        this.oddness = oddness;
        this.type = type;
        string card_res = name.Replace(" ", "");
        graphic = Resources.Load<Sprite>("Cards/card_" + card_res);
    }

    public static Dictionary<string, Card> library = new Dictionary<string, Card>()
    {
        {"Goddess Shield", new Card("Goddess Shield", "Goddess Shield_Action", 0, 0, CardType.Defensive)},
        {"Critical Hit", new Card("Critical Hit", "Critical Hit_Action", 0, 0, CardType.Offensive)},
        {"Heal", new Card("Heal", "Heal_Action", 0, 0, CardType.Assist)},
        {"Big Heal", new Card("Big Heal", "BigHeal_Action", 0, 0, CardType.Assist)},
        {"Meteor", new Card("Meteor", "Meteor_Action", 0, 0, CardType.Offensive)},
        {"Risky Strike", new Card("Risky Strike", "RiskyStrike_Action", 0, 0, CardType.Offensive)}
    };
}
