using UnityEngine;
using System.Collections;

public class Enemy : Entity {

    public Entity target;
	
    public override void Custom_Turn()
    {
        StartCoroutine(Think());
    }
    public IEnumerator Think()
    {
        float thinkTime = Random.Range(0.2f, 2.0f);
        //Debug.Log("think time:" + thinkTime);
        yield return new WaitForSeconds(thinkTime);

        ActionChoose();
        TargetChoose();
        CloseAction();
    }

    public virtual void ActionChoose ()
    {
        current_action = "Attack";
    }
    public virtual void TargetChoose ()
    {
        target = CombatManager.instance.battlers[CombatManager.instance.playerIndex];
    }
    public virtual void CloseAction ()
    {
        new Attack(current_action, this, target);
        ready = true;
    }
    public override void Custom_Dead()
    {
        CombatManager.instance.numberOfEnemies--;
    }
}
