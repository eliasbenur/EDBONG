using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Change_Prop : MonoBehaviour
{
    public Rope_System rope_system;
    public Slider slider_rope_l;
    public Player2_Movement player2mov;
    public Player_Movement playermov;
    public Slider slider_dash;
    public Slider slider_elasticity;
    public Slider slider_health;
    public Slider slider_speed;
    public Slider slider_gravityP;
    public Slider slider_defense;
    public Slider slider_MoneySpawn;

    public GameObject cheat_menu;

    // Start is called before the first frame update
    void Start()
    {
        
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

                    Vector3 InitializePosition = rope_system.Points[rope_system.NumPoints - 1].transform.position;

                    particle.transform.position = InitializePosition;
                    particle.transform.parent = rope_system.transform;
                    particle.transform.tag = "rope";
                    particle.name = "Point_" + (rope_system.NumPoints + x).ToString();

                    particle.gameObject.layer = 9;

                    if ((rope_system.NumPoints + x)%2 == 0)
                    {
                        particle.GetComponent<SpriteRenderer>().enabled = false;
                    }

                    rope_system.Points.Add(particle);

                }
                rope_system.Points[rope_system.NumPoints - 1].p_free = false;
                rope_system.Points[rope_system.NumPoints - 1 + dif_NumPoint].p_free = true;
                rope_system.NumPoints = rope_system.NumPoints + dif_NumPoint;
                rope_system._lineRenderer.positionCount = rope_system.NumPoints;
            }
            else
            {
                for (int x = 0; x < -dif_NumPoint; x++)
                {
                    Rope_Point rp = rope_system.Points[rope_system.NumPoints - 1 - x];
                    rope_system.Points.RemoveAt(rope_system.NumPoints - 1 - x);
                    Destroy(rp.gameObject);
                }
                rope_system.Points[rope_system.NumPoints - 1 + dif_NumPoint].p_free = true;
                rope_system.NumPoints = rope_system.NumPoints + dif_NumPoint;
                rope_system._lineRenderer.positionCount = rope_system.NumPoints;

            }
        }
    }

    public void change_elasticity()
    {
        Debug.Log("Working Progres :/");
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
        Debug.Log(slider_health.value);
    }

    public void resetHealth()
    {
        Debug.Log("Max Health");
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
