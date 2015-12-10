using UnityEngine;
using System.Collections;

public class Player_Explore : MonoBehaviour {

    public float speed;
    public float rot_speed;
    public float weight;

    protected CharacterController controller;
    protected Animator animator;
    protected Vector3 move_dir;
    protected Vector3 grav_dir;
    protected Vector3 final_dir;
    protected Vector2 analog_input;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        InputManager();
        Step();

	}

    protected void InputManager ()
    {
        ///Reset all input and then write it inmediatly to avoid shit
        analog_input = Vector2.zero;
        AnalogInput();
    }
    protected void AnalogInput ()
    {
        analog_input.x = Input.GetAxis("Horizontal");
        analog_input.y = Input.GetAxis("Vertical");
        analog_input = analog_input.sqrMagnitude < 0.1f ? Vector2.zero : analog_input; ///Sleep analog margin
    }
    protected void Step ()
    {
        if (analog_input != Vector2.zero) ///Player is moving the analog?
        {
            Move();
        }
        else
        {
            Stop();
        }
        Gravity();
        CharacterController_Apply();
    }
    protected void Move ()
    {
        move_dir.x = analog_input.x;
        move_dir.z = analog_input.y;

        move_dir *= speed;
        ///Summary
        /// apply rotation individually independent from moving dir
        Quaternion targetRot = Quaternion.LookRotation(move_dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rot_speed*Time.deltaTime);
    }
    protected void Stop ()
    {
        move_dir = Vector3.zero;
    }
    protected void Gravity ()
    {
        grav_dir = controller.velocity;
        grav_dir.x = 0.0f;
        grav_dir.z = 0.0f;
        grav_dir += Vector3.down * weight;
    }
    protected void CharacterController_Apply ()
    {
        final_dir = grav_dir + move_dir;
        ///Move with character controller
        controller.Move(final_dir * Time.deltaTime);
    }
}
