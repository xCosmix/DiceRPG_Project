using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {

    public float speed;
    protected Transform player;
    protected Vector3 static_distance;
	// Use this for initialization
	void Start () {
        this.transform.SetParent(null);
        player = FindObjectOfType<Player_Explore>().transform;
        static_distance = transform.position - player.position;
	}
	
	void LateUpdate () {

        Vector3 target = player.position + static_distance;

        transform.position += (target - transform.position) * Time.deltaTime * speed;
	}
}
