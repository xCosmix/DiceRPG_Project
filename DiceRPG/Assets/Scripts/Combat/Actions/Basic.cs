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
        targetEffects = new Effect[] { new Critical_100() };
    }

}