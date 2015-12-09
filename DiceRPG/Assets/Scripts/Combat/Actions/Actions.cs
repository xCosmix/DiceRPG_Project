using UnityEngine;
using System.Collections;

public class Basic : CombatAction {

    public Basic () : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new Effect[] { new Basic_Damage() };
    }
}
public class GoddessShield : CombatAction
{

    public GoddessShield() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new Effect[] { new Damage_Drop2Zero() };
    }

}
public class CriticalHit : CombatAction
{

    public CriticalHit() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new Effect[] { new Critical_100_2() };
    }

}
public class Heal : CombatAction
{

    public Heal() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new Effect[] { new Basic_Heal() };
    }

}
public class BigHeal : CombatAction
{

    public BigHeal() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new Effect[] { new Big_Heal() };
    }

}
public class Meteor : CombatAction
{

    public Meteor() : base()
    {
        targetType = TargetType.group;
        ap_cost = 3;
        targetEffects = new Effect[] { new Group_Damage() };
    }

}
public class RiskyStrike : CombatAction
{

    public RiskyStrike() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new Effect[] { new Risky_Strike() };
    }

}