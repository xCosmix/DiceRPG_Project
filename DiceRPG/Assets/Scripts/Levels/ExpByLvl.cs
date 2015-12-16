using UnityEngine;
using System.Collections;

public static class ExpByLvl : System.Object {

    public static int baseExp = 5;

    public static int get_exp2Level (int level)
    {
        return Mathf.RoundToInt(Mathf.Pow(level, 2) * baseExp);
    }
}
