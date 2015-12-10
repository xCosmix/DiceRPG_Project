using UnityEngine;
using System.Collections;

public class Changer : System.Object
{

    

    public int critical_prov = 0;
    public int hit_prov = 100;
    public CombatAction.Element element = CombatAction.Element.none;
    public CombatAction.AttackType type = CombatAction.AttackType.normal;
    public Stats adds;
    public Entity dealer;
    public Entity target;

    public bool hitted;
    public bool critical;

    public Changer(Entity dealer, Entity target, Stats adds, int critical_prov = 0, int hit_prov = 100, CombatAction.AttackType type = CombatAction.AttackType.normal, CombatAction.Element element = CombatAction.Element.none)
    {
        this.dealer = dealer;
        this.target = target;
        this.adds = adds;
        this.critical_prov = critical_prov;
        this.hit_prov = hit_prov;
        this.type = type;
        this.element = element;
    }

    public IEnumerator Send()
    {

        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(dealer, CombatAction.Events.atCauseAlter, this));
        yield return CombatManager.coroutiner.StartCoroutine(Entity.Call_Event(target, CombatAction.Events.atRecieveAlter, this));

        hitted = hit_prov >= Random.Range(1, 101);
        critical = critical_prov >= Random.Range(1, 101);
        if (critical) { adds.hp = Mathf.RoundToInt(adds.hp * Random.Range(2.0f, 3.0f)); }
        yield return dealer.StartCoroutine(target.battle_stats.TryRecieve(this));
    }
}
