using UnityEngine;
using System.Collections;

public class Alter : System.Object
{

    public Alter
        (Entity dealer, Entity target,
        int life = 0, int damage = 0, int critic = 0, int hit = 0, int critical_chance = 0, int hit_chance = 100,
        Entity.Condition condition = Entity.Condition.healthy, Entity.Element element = Entity.Element.none,
        Entity.AttackType attackType = Entity.AttackType.normal)
    {
        this.dealer = dealer;
        this.target = target;

        this.life = life;
        this.critic = critic;
        this.hit = hit;
        this.condition = condition;

        this.element = element;
        this.attackType = attackType;

        this.critical_chance = critical_chance;
        this.hit_chance = hit_chance;
    }

    public Entity dealer;
    public Entity target;

    public int life;
    public int damage;
    public int critic;
    public int hit;
    public Entity.Condition condition;

    public Entity.Element element;
    public Entity.AttackType attackType;

    //Alter critical probs && hit
    public int hit_chance;
    public int critical_chance;

    public virtual IEnumerator ApplyManager()
    {
        ///Summary
        /// use coroutiner to avoid errors when target or dealer is destroyed while this routine
        /// 
        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(dealer, CombatAction.Events.atCauseAlter, this));
        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(target, CombatAction.Events.atRecieveAlter, this));

        yield return CombatManager.coroutiner.StartCoroutine(Apply());

        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(dealer, CombatAction.Events.afterCauseAlter, this));
        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(target, CombatAction.Events.afterRecieveAlter, this));

        yield break;
    }
    public IEnumerator Apply()
    {
        //apply all shit
        bool hitted = hit_chance >= Random.Range(1, 101);
        bool critical = critical_chance >= Random.Range(1, 101);

        if (!hitted)
        {
            GUI.instance.Miss(target.transform.position);
            yield break;
        }
        if (critical)
        {
            life = Mathf.RoundToInt(life * Random.Range(2.0f, 3.0f));
        }

        if (life < 0)
        {
            yield return target.StartCoroutine(target.RecieveDamage(dealer, life, element, attackType, critical));
        }
        //APPLY ALL THE REST FIELDS OF THE STATCHANGE
        yield break;
    }
}
