using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camera_Manager : MonoBehaviour {

    public static Camera_Manager instance;

    public float speed;
    public float lookSpeed;
    [HideInInspector]
    public AudioSource source;
    public Dictionary<string, AudioClip> music;
    protected Transform player;
    protected Vector3 static_distance;
    protected Quaternion static_rot;

    protected Quaternion current_rot;
    protected Vector3 current_target;
	// Use this for initialization
	void Start () {

        instance = GameObject.FindObjectOfType<Camera_Manager>();

        music = new Dictionary<string, AudioClip>()
        {
            {"Overworld", Resources.Load<AudioClip>("Music/Breath of Fire III - Casually") },
            {"Fight", Resources.Load<AudioClip>("Music/Breath of Fire III - Fight!") },
            {"Victory", Resources.Load<AudioClip>("Music/Breath of Fire III - Victory Fanfare") }
        };

        source = GetComponent<AudioSource>();
        this.transform.SetParent(null);
        player = FindObjectOfType<Player_Explore>().transform;
        static_distance = transform.position - player.position;
        current_target = static_distance;
        static_rot = transform.rotation;
        current_rot = static_rot;
	}
	
	void LateUpdate () {

        Vector3 target = player.position + current_target;
        transform.position += (target - transform.position) * Time.deltaTime * speed;
	}
    public IEnumerator rotate ()
    {
        for (int f = 0; f < lookSpeed; f++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, current_rot, f/lookSpeed);
            yield return null;
        }
    }
    public void SetTarget (Vector3 target)
    {
        current_target = target;
    }
    public void SetRot (Quaternion rot)
    {
        current_rot = rot;
        StartCoroutine(rotate());
    }
    public void Reset ()
    {
        SetRot(static_rot);
        SetTarget(static_distance);
    }
    public void PlayClip (string clip)
    {
        source.clip = music[clip];
        source.Play();
        source.loop = true;
    }
}
