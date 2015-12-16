using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CombatAppear : System.Object {

    public static Dictionary<string, GameObject> monster_library = new Dictionary<string, GameObject>()
    {
        {"Slime", Resources.Load<GameObject>("Entities/Enemies/Slime")}
    };

    public static IEnumerator Appear (string[] listOfEnemies)
    {
        Player_Explore player_explore;
        Player player_combat;
        GameObject player;
        ///Player appear and set up for battle
        player_explore = GameObject.FindObjectOfType<Player_Explore>();
        player_combat = GameObject.FindObjectOfType<Player>();
        player = player_explore.gameObject;
        player_combat.enabled = true;
        ///Call the equivalent to start function when combat starts
        player_combat.CombatStart();
        player_explore.enabled = false;

        yield return player_combat.StartCoroutine(BattleIntro(player_combat, Camera_Manager.instance));
        ///Enemy appear
        
        foreach (string en in listOfEnemies)
        {
            Vector3 appearPos = AppearPosition(player);
            GameObject enemy = monster_library[en];
            GameObject en_Instance = GameObject.Instantiate(enemy, appearPos, enemy.transform.rotation) as GameObject;
            ///Call the equivalent to start function when combat starts
            Entity en_entity = en_Instance.GetComponent<Entity>();
            en_entity.CombatStart();

            ///Enemy variations
            int levelPlus = Random.Range(0, 4);
            for (int i = 0; i < levelPlus; i++)
            {
                ///SET UPGRADES TO ENEMIES
                en_entity.LevelUpBuff();
            }
            en_entity.myInfo.level += levelPlus;
            en_entity.myInfo.gold = en_entity.myInfo.level * Random.Range(3, 6);
        }

        ///Appear combat manager
        GameObject combatManager = new GameObject("CombatManager", new System.Type[] { typeof(CombatManager) });
    }
    public static IEnumerator BattleIntro (Player player, Camera_Manager cam)
    {
        Camera_Manager.instance.source.Stop();

        yield return new WaitForSeconds(0.6f);
        Vector3 cameraPos = player.transform.position + player.transform.forward * 4.0f;
        cameraPos += player.transform.right * 20.0f;
        cameraPos.y = player.transform.position.y + 8.0f;

        Vector3 lookDir = (player.transform.position + player.transform.forward * 4.0f) - cameraPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);

        cam.SetTarget(cameraPos - player.transform.position);
        cam.SetRot(lookRot);

        Camera_Manager.instance.PlayClip("Fight");///Music shit!!!
    }
    public static Vector3 AppearPosition (GameObject player)
    {
        float distance2Player = Random.Range(4.0f, 8.0f);
        float distanceX = Random.Range(-5.0f, 2.0f);
        Vector3 pos = player.transform.position + (player.transform.forward * distance2Player + player.transform.right * distanceX);
        return pos;
    }

    public static IEnumerator End ()
    {
        GameObject overlay_GUI = GameObject.FindObjectOfType<GUI>().gameObject;
        GameObject.Destroy(overlay_GUI);
        Player_GUI.instance.gameObject.SetActive(true);

        Player_Explore player_explore;
        Player player_combat;
        GameObject player;
        ///Player appear and set up for battle
        player_explore = GameObject.FindObjectOfType<Player_Explore>();
        player_combat = GameObject.FindObjectOfType<Player>();
        player = player_explore.gameObject;
        player_combat.enabled = false;
        player_explore.enabled = true;

        yield return player_explore.StartCoroutine(BattleEnd(Camera_Manager.instance));

        CombatManager combatManager = GameObject.FindObjectOfType<CombatManager>();
        for (int i = 0; i < combatManager.battlers.Length; i++)
        {
            if (i != combatManager.playerIndex)
                GameObject.Destroy(combatManager.battlers[i].gameObject);
        }
        GameObject.Destroy(combatManager.gameObject);
    }

    public static IEnumerator BattleEnd (Camera_Manager cam)
    {
        cam.Reset();

        Camera_Manager.instance.PlayClip("Overworld");///Music shit!!!

        yield break;
    }
}
