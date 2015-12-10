using UnityEngine;
using System.Collections;

public class Day_Cycle : MonoBehaviour {

    public Light light;
    public Color zenit_col;
    public Color dawn_col;
    public Color sunset_col;
    public float dayDuration; /// in seconds
    public float resolution; /// <summary>
    /// Time between each call
    /// </summary>
    public float current_time; /// 0.0f to 23.99f
    public byte hour;
    public byte min;
    public byte sec;

    protected int calls_mount;

	// Use this for initialization
	void Start () {
        calls_mount = Mathf.RoundToInt(dayDuration / resolution);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
