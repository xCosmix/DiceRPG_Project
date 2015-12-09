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

    public virtual IEnumerator Apply()
    {
        yield return dealer.StartCoroutine(dealer.Call_Event(CombatAction.Events.atCauseAlter, this));
        yield return target.StartCoroutine(target.Call_Event(CombatAction.Events.atRecieveAlter, this));

        yield return target.StartCoroutine(target.Alter(this));

        yield return dealer.StartCoroutine(dealer.Call_Event(CombatAction.Events.afterCauseAlter, this));
        yield return target.StartCoroutine(target.Call_Event(CombatAction.Events.afterRecieveAlter, this));
        yield break;
    }
}
