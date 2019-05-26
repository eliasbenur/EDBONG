using UnityEngine;
using UnityEngine.UI;

public class Change_Prop : MonoBehaviour
{
    public Rope_System rope_system;
    public Slider slider_rope_l;
    public Player_Movement player2mov;
    public Player_Movement playermov;
    public Slider slider_dash;
    public Toggle toggle_solo_mode;
    public Slider slider_health;
    private Slider slider_health_ui;
    public Slider slider_speed;
    public Slider slider_gravityP;
    public Slider slider_defense;
    public Slider slider_MoneySpawn;

    public GameObject cheat_menu;

    private Player_Movement p1, p2;

    private GameManager game_manager;

    // Start is called before the first frame update
    void Start()
    {
        p1 = GameObject.Find("PlayerOne").GetComponent<Player_Movement>();
        p2 = GameObject.Find("PlayerTwo").GetComponent<Player_Movement>();

        game_manager = GameObject.Find("Main Camera").GetComponent<GameManager>();

        slider_health_ui = GameObject.Find("SliderHealth").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void change_NumPoints()
    {
        int New_NumPoints = Mathf.RoundToInt(slider_rope_l.value);

        if (New_NumPoints != rope_system.NumPoints)
        {
            int dif_NumPoint = New_NumPoints - rope_system.NumPoints;
            if (dif_NumPoint > 0)
            {
                for (int x = 0; x < dif_NumPoint; x++)
                {
                    Rope_Point particle = Instantiate(rope_system.PrefabPoint, Vector3.zero, Quaternion.identity);

                    Vector3 InitializePosition = rope_system.get_points()[rope_system.NumPoints - 1].transform.position;

                    particle.transform.position = InitializePosition;
                    particle.transform.parent = rope_system.transform;
                    particle.transform.tag = "rope";
                    particle.name = "Point_" + (rope_system.NumPoints + x).ToString();

                    particle.gameObject.layer = 9;

                    if ((rope_system.NumPoints + x)%2 == 0)
                    {
                        particle.GetComponent<SpriteRenderer>().enabled = false;
                    }

                    rope_system.get_points().Add(particle);

                }
                rope_system.NumPoints = rope_system.NumPoints + dif_NumPoint;
            }
            else
            {
                for (int x = 0; x < -dif_NumPoint; x++)
                {
                    Rope_Point rp = rope_system.get_points()[rope_system.NumPoints - 1 - x];
                    rope_system.get_points().RemoveAt(rope_system.NumPoints - 1 - x);
                    Destroy(rp.gameObject);
                }
                rope_system.NumPoints = rope_system.NumPoints + dif_NumPoint;

            }
        }
    }

    public void change_solo_mode()
    {
        if (toggle_solo_mode.isOn)
        {
            toggle_solo_mode.isOn = false;
            p1.modo_solo = false;
            p2.modo_solo = false;
        }
        else
        {
            toggle_solo_mode.isOn = true;
            p1.modo_solo = true;
            p2.modo_solo = true;
        }

        p1.set_Solo_Mode();
        p2.set_Solo_Mode();
    }

    public void change_DashColdown()
    {
        playermov.dash_delay = slider_dash.value;
    }

    public void change_DashColdown2()
    {
        player2mov.dash_delay = slider_dash.value;
    }

    public void setHealth()
    {
        game_manager.life = slider_health.value;
        game_manager.max_Life = slider_health.value;
        slider_health_ui.maxValue = slider_health.value;
    }

    public void resetHealth()
    {
        game_manager.life = game_manager.max_Life;
    }

    public void change_speedp1()
    {
        playermov.speed = slider_speed.value;
    }

    public void change_speedp2()
    {
        player2mov.speed = slider_speed.value;
    }

    public void change_gravityrope()
    {
        Debug.Log(slider_gravityP.value);
    }

    public void change_defense()
    {
        Debug.Log(slider_defense.value);
    }

    public void change_money_spawn()
    {
        Debug.Log(slider_MoneySpawn.value);
    }

    public void hide_cheat_mode()
    {
        if (cheat_menu.activeSelf)
        {
            cheat_menu.SetActive(false);
        }
        else
        {
            cheat_menu.SetActive(true);
        }
        
    }
}
