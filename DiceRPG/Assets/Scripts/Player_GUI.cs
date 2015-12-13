using UnityEngine;
using System.Collections;

public class Player_GUI : MonoBehaviour {
    public static Player_GUI instance;
    // Use this for initialization
    void Start () {
        instance = GameObject.FindObjectOfType<Player_GUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
