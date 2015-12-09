using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAction : System.Object {

    //Libraries
    public static Dictionary<string, CombatAction> library = new Dictionary<string, CombatAction>()
    {
        {"Attack", new Basic()},
        {"Goddess Shield_Action", new GoddessShield()},
        {"Critical Hit_Action", new CriticalHit()},
        {"Heal_Action", new Heal()},
        {"BigHeal_Action", new BigHeal()},
        {"Meteor_Action", new Meteor()},
        {"RiskyStrike_Action", new RiskyStrike()}
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
                    if (t.dead) continue;
                    raw_out.Add(new List<Entity>() { t });
                }
                break;

            case (TargetType.group):
                List<Entity> enemies = new List<Entity>();
                foreach (Entity t in CombatManager.instance.enemies)
                {
                    if (t.dead) continue;
                    enemies.Add(t);
                }
                raw_out.Add(enemies);
                List<Entity> goodGuys = new List<Entity>();
                foreach (Entity t in CombatManager.instance.goodGuys)
                {
                    if (t.dead) continue;
                    goodGuys.Add(t);
                }
                raw_out.Add(goodGuys);
                break;

            case (TargetType.all):
                List<Entity> all = new List<Entity>();
                foreach (Entity t in CombatManager.instance.battlers)
                {
                    if (t.dead) continue;
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
        List<Entity> target = new List<Entity>();
        Entity[] output;

        foreach (Entity e in environment[groupIndex])
        {
            if (!e.dead)
            {
                target.Add(e);
            }
        }

        output = new Entity[target.Count];
        for (int i = 0; i < target.Count; i++)
        {
            output[i] = target[i];
        }
        return output;
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
}
