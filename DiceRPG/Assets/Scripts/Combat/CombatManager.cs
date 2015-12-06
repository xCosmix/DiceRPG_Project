using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour {

    public Entity[] battlers; //General
    public Enemy[] enemies; //group A
    public Friendly[] goodGuys; //group b
    public Player player;
    public int playerIndex;
    public int numberOfEnemies;

    public static CombatManager instance;

    public int[] order;

	// Use this for initialization
	void Start () {

        if (instance == null) instance = FindObjectOfType<CombatManager>();

        battlers = FindObjectsOfType<Entity>();
        enemies = FindObjectsOfType<Enemy>();
        goodGuys = FindObjectsOfType<Friendly>();
        numberOfEnemies = enemies.Length;
        order = new int[battlers.Length];
        Sort();
        GUI.instance.SetEnemiesPanels();
        StartCoroutine(Combat());
	}
	private void Sort ()
    {
        for (int o = 0; o < order.Length; o++)
        {
            order[o] = -1;
        }
        for (int m = 0; m < order.Length; m++)
        {
            int pos = order.Length-1;
            for (int i = 0; i < battlers.Length; i++)
            {
                if (m == i) continue;
                if (battlers[m].level > battlers[i].level || (battlers[m].level == battlers[i].level && order[pos]!=-1)) pos--;
            }
            order[pos] = m;
            if (battlers[m].GetType().Equals(typeof(Player))) playerIndex = m;
        }
    }
	public IEnumerator Combat ()
    {
        while (true)
        {
            //Round Start
            foreach (Entity battler in battlers)
            {
                if (battler == null) continue;
                yield return StartCoroutine(battler.Call_Event(CombatAction.Events.startRound));
            }

            //Decicion step
            for (int i = 0; i < battlers.Length; i++)
            {
                Entity battler = battlers[order[i]];
               // Debug.Log("try turn: " + battler.name);
                if (battler == null) continue; //in case a battler is dead, jump to the next one
                if (!battler.CanPickTurn()) continue;
                //Debug.Log("current turn: " + battler.name);
                yield return StartCoroutine(battler.Turn());
            }
            //Fight step
            for (int i = 0; i < battlers.Length; i++)
            {
                if (Player.instance == null) break; //End battle if player die

                Entity battler = battlers[order[i]];
                if (battler == null) continue;
                foreach (CombatBridge cb in battler.get_actions())
                {
                    yield return StartCoroutine(cb.Act());
                }
                //battler.clean_actions; //Clean combataction after acting
            }

            //Round End
            foreach (Entity battler in battlers)
            {
                if (battler == null) continue;
                yield return StartCoroutine(battler.Call_Event(CombatAction.Events.endRound));
            }

            Debug.Log("End of Round");
            //Check if win or lose
            if (numberOfEnemies == 0)
            {
                Victory();
                break;
            }
            else
            {
                if (Player.instance == null)
                {
                    Defeat();
                    break;
                }
            }
        }
    }
    public void Victory ()
    {
        GUI.instance.Victory();
    }
    public void Defeat ()
    {
        GUI.instance.Defeat();
    }
}
