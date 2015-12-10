using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CombatAppear : System.Object {

    public static Dictionary<string, GameObject> monster_library = new Dictionary<string, GameObject>()
    {
        {"Slime", Resources.Load<GameObject>("Entities/Enemies/Slime")}
    };

    public static void Appear (string[] listOfEnemies)
    {
        Player_Explore player_explore;
        Player player_combat;
        GameObject player;
        ///Player appear and set up for battle
        player_explore = GameObject.FindObjectOfType<Player_Explore>();
        player_combat = GameObject.FindObjectOfType<Player>();
        player = player_explore.gameObject;
        player_combat.enabled = true;
        player_explore.enabled = false;

        ///Enemy appear
        
        foreach (string en in listOfEnemies)
        {
            Vector3 appearPos = AppearPosition(player);
            GameObject enemy = monster_library[en];
            GameObject.Instantiate(enemy, appearPos, enemy.transform.rotation);
        }

        ///Appear combat manager
        GameObject combatManager = new GameObject("CombatManager", new System.Type[] { typeof(CombatManager) });
    }
    public static Vector3 AppearPosition (GameObject player)
    {
        float distance2Player = Random.Range(4.0f, 8.0f);
        float distanceX = Random.Range(-5.0f, 2.0f);
        Vector3 pos = player.transform.position + (player.transform.forward * distance2Player + player.transform.right * distanceX);
        return pos;
    }
}
