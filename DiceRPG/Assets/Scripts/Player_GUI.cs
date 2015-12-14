using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_GUI : MonoBehaviour {
    public static Player_GUI instance;

    private Entity_Info playerInfo;
    private GameObject stats_panel;
    private Button stats_button;
    private Text level_value, exp_value, life_value, dmg_value, def_value, hit_value, crit_value, counter_value;
    // Use this for initialization
    void Start () {
        instance = GameObject.FindObjectOfType<Player_GUI>();

        playerInfo = GameObject.FindObjectOfType<Player_Explore>().GetComponent<Entity_Info>();
        stats_panel = GameObject.Find("stats_panel");
        stats_button = GameObject.Find("stats_button").GetComponent<Button>();

        level_value = GameObject.Find("level_value").GetComponent<Text>();
        exp_value = GameObject.Find("exp_value").GetComponent<Text>();
        life_value = GameObject.Find("life_value").GetComponent<Text>();
        dmg_value = GameObject.Find("dmg_value").GetComponent<Text>();
        def_value = GameObject.Find("def_value").GetComponent<Text>();
        hit_value = GameObject.Find("hit_value").GetComponent<Text>();
        crit_value = GameObject.Find("critic_value").GetComponent<Text>();
        counter_value = GameObject.Find("counter_value").GetComponent<Text>();
        ToggleStats();
    }
	
	// Update is called once per frame
	void Update () {
	    if (GUI.PressedButton(stats_button.image.rectTransform)){
            ToggleStats();
        }
	}
    protected void ToggleStats ()
    {
        stats_panel.SetActive(!stats_panel.activeSelf);
        level_value.text = playerInfo.level + "";
        exp_value.text = playerInfo.exp + "";
        life_value.text = playerInfo.stats.main.hp + "";
        if (playerInfo.stats.GetValues("Default").hp < playerInfo.stats.main.hp) life_value.color = new Color(0.1f, 1.0f, 0.5f);
        dmg_value.text = playerInfo.stats.main.dmg + "";
        if (playerInfo.stats.GetValues("Default").dmg < playerInfo.stats.main.dmg) dmg_value.color = new Color(0.1f, 1.0f, 0.5f);
        def_value.text = playerInfo.stats.main.def + "";
        if (playerInfo.stats.GetValues("Default").def < playerInfo.stats.main.def) def_value.color = new Color(0.1f, 1.0f, 0.5f);
        hit_value.text = "%" + playerInfo.stats.main.hit;
        if (playerInfo.stats.GetValues("Default").hit < playerInfo.stats.main.hit) hit_value.color = new Color(0.1f, 1.0f, 0.5f);
        crit_value.text = "%" + playerInfo.stats.main.crit;
        if (playerInfo.stats.GetValues("Default").crit < playerInfo.stats.main.crit) crit_value.color = new Color(0.1f, 1.0f, 0.5f);
        counter_value.text = "%" + playerInfo.stats.main.counter;
        if (playerInfo.stats.GetValues("Default").counter < playerInfo.stats.main.counter) counter_value.color = new Color(0.1f, 1.0f, 0.5f);
    }
}
