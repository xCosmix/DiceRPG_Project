using UnityEngine;
using System.Collections;

public class Slime : Enemy {

    public Animator animator;

    // Use this for initialization
    public override void Custom_Start () {
        animator = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public override IEnumerator Animation_Attack()
    {
        animator.SetInteger("state", 1);
        yield return new WaitForSeconds(1.0f);
        animator.SetInteger("state", 0);
    }
    public override IEnumerator Animation_Hitted()
    {
        animator.SetInteger("state", 2);
        yield return new WaitForSeconds(0.5f);
        animator.SetInteger("state", 0);
    }
}
