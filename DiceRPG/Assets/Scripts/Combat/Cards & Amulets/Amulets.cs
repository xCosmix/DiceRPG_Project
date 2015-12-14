using UnityEngine;
using System.Collections.Generic;

public class Amulet : System.Object {

    public string name = "none"; //Name of the card
    public int amuletCost = 100; //gold cost
    public int oddness = 0; // scale from 1 to 10

    public string action = "none"; // Key from global dictionary at combatAction
    public Stats.Values buff = null;

    public Amulet(string name, string action, int amuletCost, int oddness, Stats.Values buff)
    {
        this.name = name;
        this.action = action;
        this.amuletCost = amuletCost;
        this.oddness = oddness;
        this.buff = buff;
    }
    public void Equip (Entity target)
    {
        //target.myInfo.amulets.Add(name);
        if (buff != null) target.myInfo.stats.Add(name, buff);
    }
    public static Dictionary<string, Amulet> library = new Dictionary<string, Amulet>()
    {
        {"Willpower", new Amulet("Willpower", null, 0, 0, new Stats.Values(1))},
        {"Warrior", new Amulet("Warrior", null, 0, 0, new Stats.Values(0, 0, 1))},
        {"Tank", new Amulet("Tank", null, 0, 0, new Stats.Values(0, 0, 0, 1))},
        {"Bullseye", new Amulet("Bullseye", null, 0, 0, new Stats.Values(0, 0, 0, 0, 5))},
        {"Stamina", new Amulet("Stamina", null, 0, 0, new Stats.Values(0, 0, 0, 0, 0, 5))},
        {"Revenge", new Amulet("Revenge", null, 0, 0, new Stats.Values(0, 0, 0, 0, 0, 0, 5))}
    };
}
