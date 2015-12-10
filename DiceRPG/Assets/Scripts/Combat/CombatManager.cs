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
    public static MonoBehaviour coroutiner;

    public int[] order;

	/// <summary>
    /// Combat manager is the one which is in charge for set everything up before the combat starts
    /// </summary>
	void Start () {

        if (instance == null) instance = FindObjectOfType<CombatManager>();
        if (coroutiner == null) coroutiner = new GameObject("Coroutiner", new System.Type[]{ typeof(MonoBehaviour) }).GetComponent<MonoBehaviour>();

        StartCoroutine(SetUp());
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
                if (battlers[m].myInfo.level > battlers[i].myInfo.level || (battlers[m].myInfo.level == battlers[i].myInfo.level && order[pos]!=-1)) pos--;
            }
            order[pos] = m;
            if (battlers[m].GetType().Equals(typeof(Player))) playerIndex = m;
        }
    }
    public IEnumerator SetUp ()
    {
        Application.LoadLevelAdditive("overlay_combat_GUI");

        yield return null;

        battlers = FindObjectsOfType<Entity>();
        enemies = FindObjectsOfType<Enemy>();
        goodGuys = FindObjectsOfType<Friendly>();
        numberOfEnemies = enemies.Length;
        order = new int[battlers.Length];
        Sort();
        GUI.instance.SetEnemiesPanels();
        StartCoroutine(Combat());
    }
	public IEnumerator Combat ()
    {
        while (true)
        {
            //Round Start
            foreach (Entity battler in battlers)
            {
                if (battler.get_dead()) continue;
                yield return StartCoroutine(battler.Call_Event(CombatAction.Events.startRound));
            }

            //Decicion step
            for (int i = 0; i < battlers.Length; i++)
            {
                Entity battler = battlers[order[i]];
               // Debug.Log("try turn: " + battler.name);
                if (battler.get_dead()) continue; //in case a battler is get_dead(), jump to the next one
                if (!battler.CanPickTurn()) continue;
                //Debug.Log("current turn: " + battler.name);
                yield return StartCoroutine(battler.Turn());
            }
            //Fight step
            for (int i = 0; i < battlers.Length; i++)
            {
                if (Player.instance == null) break; //End battle if player die

                Entity battler = battlers[order[i]];
                if (battler.get_dead()) continue;
                foreach (CombatBridge cb in battler.get_actions())
                {
                    yield return StartCoroutine(cb.Act());
                }
                //battler.clean_actions; //Clean combataction after acting
            }

            //Round End
            foreach (Entity battler in battlers)
            {
                if (battler.get_dead()) { battler.gameObject.SetActive(false); continue; } ///DDISABLE get_dead() ENEMY AT END OF ROUND

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
                if (Player.instance.get_dead())
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
