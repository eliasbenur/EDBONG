using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes_Trigge : MonoBehaviour
{
    public GameObject playerone;
    public GameObject playertwo;

    public float scale_speed;

    private float delay;
    private float delay_tmp;

    private float delay_two;
    private float delay_tmp_two;

    private void Start()
    {
        delay = 0.2f;
        delay_tmp = delay;

        delay_two = 0.2f;
        delay_tmp_two = delay;
    }

    private void Update()
    {
        if (playerone != null)
        {
            if (playerone.transform.localScale.x > 0)
            {
                playerone.transform.localScale = new Vector3(playerone.transform.localScale.x - Time.deltaTime * scale_speed, playerone.transform.localScale.y - Time.deltaTime * scale_speed, playerone.transform.localScale.z);
            }
            else
            {
                Vector3 pos_torespawn = gameObject.transform.GetChild(0).transform.position;
                float dist = (gameObject.transform.GetChild(0).transform.position - playerone.transform.position).magnitude;
                for (int x = 1; x < gameObject.transform.childCount ; x++)
                {
                    if ((gameObject.transform.GetChild(x).transform.position - playerone.transform.position).magnitude < dist)
                    {
                        pos_torespawn = gameObject.transform.GetChild(x).transform.position;
                        dist = (gameObject.transform.GetChild(x).transform.position - playerone.transform.position).magnitude;
                    }
                }
                int Nump = GameObject.Find("Rope_System").GetComponent<Rope_System>().NumPoints;
                Rope_System rope = GameObject.Find("Rope_System").GetComponent<Rope_System>();
                rope.Points[0].transform.position = pos_torespawn;
                Vector3 Delta = rope.Points[Nump -1].transform.position - rope.Points[0].transform.position;

                for (int ParticleIndex = 0; ParticleIndex < Nump; ParticleIndex++)
                {

                    float Alpha = (float)ParticleIndex / (float)(Nump - 1);
                    Vector3 InitializePosition = rope.Points[0].transform.position + (Alpha * Delta);

                    rope.Points[ParticleIndex].transform.position = InitializePosition;

                }
                playerone.transform.localScale = new Vector3(0.75f, 0.75f, 1);
                playerone.GetComponent<Player_Movement>().can_move = true;
                GameObject.Find("PlayerTwo").GetComponent<Player_Movement>().can_move = true;
                delay_tmp = 0;

                Camera.main.GetComponent<GameManager>().Hit_verification("PlayerOne", playerone.transform.position, "Fall Damage");
                playerone = null;


            }
        }
        if (playertwo != null)
        {
            if (playertwo.transform.localScale.x > 0)
            {
                playertwo.transform.localScale = new Vector3(playertwo.transform.localScale.x - Time.deltaTime * scale_speed, playertwo.transform.localScale.y - Time.deltaTime * scale_speed, playertwo.transform.localScale.z);
            }
            else
            {
                Vector3 pos_torespawn = gameObject.transform.GetChild(0).transform.position;
                float dist = (gameObject.transform.GetChild(0).transform.position - playertwo.transform.position).magnitude;
                for (int x = 1; x < gameObject.transform.childCount; x++)
                {
                    if ((gameObject.transform.GetChild(x).transform.position - playertwo.transform.position).magnitude < dist)
                    {
                        pos_torespawn = gameObject.transform.GetChild(x).transform.position;
                        dist = (gameObject.transform.GetChild(x).transform.position - playertwo.transform.position).magnitude;
                    }
                }
                int Nump = GameObject.Find("Rope_System").GetComponent<Rope_System>().NumPoints;
                Rope_System rope = GameObject.Find("Rope_System").GetComponent<Rope_System>();
                rope.Points[Nump - 1].transform.position = pos_torespawn;
                Vector3 Delta = rope.Points[Nump - 1].transform.position - rope.Points[0].transform.position;

                for (int ParticleIndex = 0; ParticleIndex < Nump; ParticleIndex++)
                {

                    float Alpha = (float)ParticleIndex / (float)(Nump - 1);
                    Vector3 InitializePosition = rope.Points[0].transform.position + (Alpha * Delta);

                    rope.Points[ParticleIndex].transform.position = InitializePosition;

                }
                playertwo.transform.localScale = new Vector3(0.75f, 0.75f, 1);
                playertwo.GetComponent<Player_Movement>().can_move = true;
                GameObject.Find("PlayerOne").GetComponent<Player_Movement>().can_move = true;
                delay_tmp_two = 0;

                Camera.main.GetComponent<GameManager>().Hit_verification("PlayerTwo", playertwo.transform.position, "Fall Damage");
                playertwo = null;

            }
        }

        if (delay_tmp < delay)
        {
            delay_tmp += Time.deltaTime;
        }

        if (delay_tmp_two < delay_two)
        {
            delay_tmp_two += Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            Player_Movement pm = collision.gameObject.GetComponent<Player_Movement>();
            if (collision.name == "PlayerOne" && delay_tmp >= delay && pm.dash_v < (pm.dash_delay - pm.dash_time))
            {
                playerone = collision.gameObject;
                playerone.GetComponent<Player_Movement>().can_move = false;
                playerone.GetComponent<Player_Movement>().moveX = 0;
                playerone.GetComponent<Player_Movement>().moveY = 0;
                GameObject.Find("PlayerTwo").GetComponent<Player_Movement>().Stop_Moving();
                Debug.Log("aaaaaa");
                SoundManager.PlaySound(SoundManager.Sound.PlayerFalling);
            }
            else if (collision.name == "PlayerTwo" && delay_tmp_two >= delay && pm.dash_v < (pm.dash_delay - pm.dash_time))
            {
                playertwo = collision.gameObject;
                playertwo.GetComponent<Player_Movement>().can_move = false;
                playertwo.GetComponent<Player_Movement>().moveX = 0;
                playertwo.GetComponent<Player_Movement>().moveY = 0;
                GameObject.Find("PlayerOne").GetComponent<Player_Movement>().Stop_Moving();
                SoundManager.PlaySound(SoundManager.Sound.PlayerFalling);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.tag == "player")
        {
            if (collision.name == "PlayerOne")
            {
                playerone = null;
            }
            else if (collision.name == "PlayerTwo")
            {
                playertwo = null;
            }
        }*/
    }
}
