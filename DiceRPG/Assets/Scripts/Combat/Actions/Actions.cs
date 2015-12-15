using UnityEngine;
using System.Collections;

public class Basic : CombatAction {

    public Basic () : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new string[] { "Basic Damage" };
    }
}
public class GoddessShield : CombatAction
{

    public GoddessShield() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new string[] { "Damage Drop to Zero" };
    }

}
public class CriticalHit : CombatAction
{

    public CriticalHit() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new string[] { "Critical 100 ver2" };
    }

}
public class Heal : CombatAction
{

    public Heal() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new string[] { "Basic Heal" };
    }

}
public class BigHeal : CombatAction
{

    public BigHeal() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 1;
        targetEffects = new string[] { "Big Heal" };
    }

}
public class Meteor : CombatAction
{

    public Meteor() : base()
    {
        targetType = TargetType.group;
        ap_cost = 3;
        targetEffects = new string[] { "Group Damage" };
    }

}
public class RiskyStrike : CombatAction
{

    public RiskyStrike() : base()
    {
        targetType = TargetType.unique;
        ap_cost = 2;
        targetEffects = new string[] { "Risky Strike" };
    }

}