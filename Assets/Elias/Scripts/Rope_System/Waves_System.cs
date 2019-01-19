using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waves_System : MonoBehaviour
{
    public GameObject wave_button1;
    public GameObject wave_button2;

    public Text wave_title;
    public Text wave_title_num;
    public Text wave_txt_num;

    public float delay_wave_tit;
    public float curr_delay_wave_tit;

    public int current_wave;

    public TileData2 tiledata;
    //public TileData2 tiledata2;

    public int num_enemies_dif;
    public List<Transform> current_enemies;
    public List<GameObject> dif_enemies;

    // Start is called before the first frame update
    void Start()
    {
        curr_delay_wave_tit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(tiledata.rows[1].row[0]);

        if (players_ready())
        {
            UI_g();
        }

        if (curr_delay_wave_tit > 0)
        {
            curr_delay_wave_tit -= Time.deltaTime;
        }
        else
        {
            curr_delay_wave_tit = 0;
            wave_title.gameObject.SetActive(false);
            wave_title_num.gameObject.SetActive(false);
        }

        delete_enemies_dead();
        if (current_enemies.Count == 0)
        {
            wave_button1.SetActive(true);
            wave_button2.SetActive(true);
        }
    }

    bool players_ready()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        if (wave_button1.GetComponent<CircleCollider2D>().IsTouchingLayers(mask) && wave_button2.GetComponent<CircleCollider2D>().IsTouchingLayers(mask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UI_g()
    {

        current_wave++;



        curr_delay_wave_tit = delay_wave_tit;

        wave_button1.gameObject.SetActive(false);
        wave_button2.gameObject.SetActive(false);

        wave_title_num.text = current_wave.ToString();
        wave_txt_num.text = current_wave.ToString();
        wave_title.gameObject.SetActive(true);
        wave_title_num.gameObject.SetActive(true);


        for (int x=0; x<num_enemies_dif; x++)
        {
            //Debug.Log(tiledata.rows[current_wave-1].row[x]);
            if (tiledata.rows[current_wave - 1].row[x] > 0)
            {
                for (int y = 0; y < tiledata.rows[current_wave - 1].row[x]; y++)
                {
                    float random_x = Random.Range(0, 41);
                    float random_y = Random.Range(-19, 0);
                    GameObject enem = Instantiate(dif_enemies[x], new Vector3(-5 + random_x, -20 + random_y, 0), Quaternion.identity);
                    current_enemies.Add(enem.transform);
                }
            }
        }
    }

    public void delete_enemies_dead()
    {
        for (int x = 0; x<current_enemies.Count; x++)
        {
            if (current_enemies[x] == null)
            {
                current_enemies.RemoveAt(x);
                delete_enemies_dead();
                break;
            }
        }
    }
}
