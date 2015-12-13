using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class Player : Friendly {

    public static Player instance;
    public static Animator animator;
    private bool private_ready;

	// Use this for initialization
	public override void Custom_Start () {
        instance = GameObject.FindObjectOfType<Player>();
        animator = instance.GetComponentInChildren<Animator>();
	}
    public override void Turn_On()
    {
        //shit here
    }
    public override void Custom_EndTurn()
    {
        //GUI.instance.Player_Turn(false);
    }
    public override void Custom_Turn()
    {
        private_ready = false;
        GUI.instance.Player_Turn(true);
    }
    public void Call_Ready ()
    {
        ///summary
        ///GUI Input call to tell COMBAT MANAGER that the player is ready 
        private_ready = true;
    }
    public bool Can_Ready ()
    {
        return actions.Count > 0;
    }
    public override bool Ready()
    {
        return private_ready;
    }

    public override void TargetChoose()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200.0f))
        {
            Entity over = hit.collider.gameObject.GetComponent<Entity>();
            if (over != null && Input.GetMouseButtonDown(0))
            {
                target = over;
                over = null;
            }
            GraphicTargetShow(over);
        }
    }

    private Entity lastOver;
    private void GraphicTargetShow (Entity over)
    {
        Entity[] group;
        if (over != null)
        {
            if (lastOver != over)
            {
                if (lastOver != null)
                {
                    group = CombatAction.library[current_action].Target(lastOver);
                    foreach (Entity t in group)
                    {
                        t.guiRef.Deselect();
                    }
                }

                group = CombatAction.library[current_action].Target(over);
                foreach (Entity t in group)
                {
                    t.guiRef.Select();
                }
            }
        }
        else
        {
            if (lastOver != null)
            {
                group = CombatAction.library[current_action].Target(lastOver);
                foreach (Entity t in group)
                {
                    t.guiRef.Deselect();
                }
            }
        }

        lastOver = over;
    }
    //Anims________

    public override IEnumerator Animation_Attack()
    {
        animator.SetInteger("state", 1);
        yield return new WaitForSeconds(1.0f);
        animator.SetInteger("state", 0);
    }
    public override IEnumerator Animation_Card()
    {
        animator.SetInteger("state", 1);
        yield return new WaitForSeconds(1.0f);
        animator.SetInteger("state", 0);
    }
    public override IEnumerator Animation_Hitted()
    {
        animator.SetInteger("state", 2);
        yield return new WaitForSeconds(0.4f);
        animator.SetInteger("state", 0);
    }
}
