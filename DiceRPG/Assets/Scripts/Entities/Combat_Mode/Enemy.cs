using UnityEngine;
using System.Collections;

public class Enemy : Entity {
	
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
    }

    public virtual void ActionChoose ()
    {
        Call_ActionPick("Attack");
    }
    public override void TargetChoose ()
    {
        target = CombatManager.instance.battlers[CombatManager.instance.playerIndex];
    }
    public override bool Ready()
    {
        return actions.Count > 0;
    }

    public override void Custom_Dead()
    {
        CombatManager.instance.numberOfEnemies--;
    }
}
