using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAction : System.Object {

    //Libraries
    public static Dictionary<string, CombatAction> library = new Dictionary<string, CombatAction>()
    {
        {"Attack", new Basic()},
        {"Goddess Shield", new GoddessShield()},
        {"Critical Hit", new CriticalHit()}
    };
    public enum TargetType { unique, group, all };
    public enum Events
    {
        start, end, startTurn,
        endTurn, startRound, endRound, attack,
        spell, target, targeted, spelled,
        attacked, recieveDamage, dealDamage, dead,
        atRecieveAlter,atCauseAlter,
        afterCauseAlter, afterRecieveAlter
    }

    //Action props
    public int ap_cost = 0;
    public TargetType targetType = TargetType.unique;
    public Effect[] targetEffects;
    public Effect[] invokerEffects;

    public virtual IEnumerator Activate (Entity invoker, Entity[] target)
    {
        if (invokerEffects != null)
        {
            foreach (Effect eff in invokerEffects)
            {
                //HAVE TO DO A COPY FOR EACH TARGET INDIVIDUAlly
                Effect copy = eff.Copy();
                copy.invoker = invoker;
                copy.owner = invoker;
                yield return invoker.StartCoroutine(invoker.Add_Effect(eff));
            }
        }

        if (targetEffects != null)
        {
            foreach (Effect eff in targetEffects)
            {
                foreach (Entity tar in target)
                {
                    //HAVE TO DO A COPY FOR EACH TARGET INDIVIDUAlly
                    Effect copy = eff.Copy();
                    copy.invoker = invoker;
                    copy.owner = tar;
                    yield return tar.StartCoroutine(tar.Add_Effect(copy));
                }
            }
        }
        yield break;
    }

    //Action manager
    public bool Can_PayAP(Entity owner)
    {
        int cost = ap_cost;
        return owner.current_ap - cost >= 0;
    }

    public Entity[][] AvailableTargets()
    {
        TargetType type = targetType;
        Entity[][] output = null;
        List<List<Entity>> raw_out = new List<List<Entity>>();

        switch (type)
        {

            case (TargetType.unique):
                foreach (Entity t in CombatManager.instance.battlers)
                {
                    if (t == null) continue;
                    raw_out.Add(new List<Entity>() { t });
                }
                break;

            case (TargetType.group):
                List<Entity> enemies = new List<Entity>();
                foreach (Entity t in CombatManager.instance.enemies)
                {
                    if (t == null) continue;
                    enemies.Add(t);
                }
                raw_out.Add(enemies);
                List<Entity> goodGuys = new List<Entity>();
                foreach (Entity t in CombatManager.instance.goodGuys)
                {
                    if (t == null) continue;
                    goodGuys.Add(t);
                }
                raw_out.Add(goodGuys);
                break;

            case (TargetType.all):
                List<Entity> all = new List<Entity>();
                foreach (Entity t in CombatManager.instance.battlers)
                {
                    if (t == null) continue;
                    all.Add(t);
                }
                raw_out.Add(all);
                break;
        }

        output = new Entity[raw_out.Count][];
        for (int i = 0; i < output.Length; i++)
        {
            Entity[] arr = new Entity[raw_out[i].Count];
            for (int a = 0; a < arr.Length; a++)
            {
                arr[a] = raw_out[i][a];
            }
            output[i] = arr;
        }

        return output;
    }
    public Entity[] Target(Entity t)
    {
        Entity[][] environment = AvailableTargets();
        int groupIndex = GetTargetGroupIndex(t, environment);
        return environment[groupIndex];
    }
    public static int GetTargetGroupIndex(Entity t, Entity[][] environment)
    {
        for (int i = 0; i < environment.Length; i++)
        {
            for (int x = 0; x < environment[i].Length; x++)
            {
                if (t == environment[i][x]) return i;
            }
        }
        return -1;
    }
    public Entity PickRandomTarget(TargetType type) //DEPRECATED HAVE TO ACTUALIZE
    {
        List<int> options = new List<int>();
        for (int i = 0; i < CombatManager.instance.enemies.Length; i++)
        {
            if (CombatManager.instance.enemies[i] != null) options.Add(i);
        }
        int random = Random.Range(0, options.Count);
        return CombatManager.instance.enemies[options[random]];
    }
}
