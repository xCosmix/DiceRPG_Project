using UnityEngine;
using System.Collections;

public class CombatSpawner : MonoBehaviour {

    public static CombatSpawner instance;

    public string[] monsterRegion;
    public float avarageCombatDistance;
    public int combatProbability;

    protected Vector3 lastCombatPoint;
    protected Player_Explore player;
    protected Coroutine evaluate_coroutine;
	// Use this for initialization
	void Start () {

        instance = GameObject.FindObjectOfType<CombatSpawner>();

        player = FindObjectOfType<Player_Explore>();
        lastCombatPoint = player.transform.position;
        evaluate_coroutine = StartCoroutine(Evaluate_Repeat(1.5f));
	}
	IEnumerator Evaluate_Repeat (float r)
    {
        while (true)
        {
            float wait = Random.Range(r * 0.7f, r * 1.3f);
            yield return new WaitForSeconds(r);
            Evaluate();
        }
    }
	void Evaluate () {

        float prob = Random.Range(1, 101);
        if (prob > combatProbability) return;

        float combatDistance = Random.Range(avarageCombatDistance * 0.5f, avarageCombatDistance * 1.5f);
        float distance = (player.transform.position - lastCombatPoint).sqrMagnitude;
       // Debug.Log(distance + " > " + combatDistance);
        if (distance > combatDistance)
        {
            StartCombat();
        }
	}
    void StartCombat()
    {
        lastCombatPoint = player.transform.position;
        StopCoroutine(evaluate_coroutine);
        byte numberOfMonsters = (byte)Random.Range(1, 5);
        string[] monsters = new string[numberOfMonsters];
        for (int i = 0; i < numberOfMonsters; i++)
        {
            int randomMonster = Random.Range(0, monsterRegion.Length);
            monsters[i] = monsterRegion[randomMonster];
        }
        StartCoroutine(CombatAppear.Appear(monsters));
    }
    public void EndCombat ()
    {
        evaluate_coroutine = StartCoroutine(Evaluate_Repeat(1.5f));
        StartCoroutine(CombatAppear.End());
    }
}
