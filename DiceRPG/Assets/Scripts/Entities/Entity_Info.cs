﻿using UnityEngine;
using System.Collections.Generic;

public class Entity_Info : MonoBehaviour {

    void Start ()
    {
        owner = gameObject.GetComponent<Entity>();
        owner.myInfo = this;
        SetAmulets();
    }

    public new string name;
    public int level;
    public int exp;
    public int gold;

    public Stats stats;
    public List<string> deck = new List<string>(); //string cuz is a ref to the card library
    public List<string> amulets = new List<string>(); //not implemented yet

    protected Entity owner;

    protected void SetAmulets()
    {
        int length = amulets.Count;
        for (int i = 0; i < length; i++)
        {
            string myAmulet = amulets[i];
            Amulet.library[myAmulet].Equip(owner);
        }
    }
}
