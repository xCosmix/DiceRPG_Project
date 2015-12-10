using UnityEngine;
using System.Collections;

public class Entity_Info : MonoBehaviour {

    public new string name;
    public int level;
    public int exp;
    public int gold;

    public Stats stats;
    public string[] deck; //string cuz is a ref to the card library
    public string[] amulets; //not implemented yet
}
