using UnityEngine;

public class Holes_Trigge : MonoBehaviour
{
    private Player_Movement playerone;
    private Player_Movement playertwo;
    private bool playerone_falling;
    private bool playertwo_falling;

    public float scale_speed;

    public float delay;
    private float delay_tmp;
    private float delay_tmp_two;
    private Rope_System rope;

    public God_Mode godMode_Hole1, godMode_Hole2;

    private void Start()
    {
        delay = 0.2f;
        delay_tmp = delay;
        delay_tmp_two = delay;
        rope = GameObject.Find("Rope_System").GetComponent<Rope_System>();
        playerone = GameObject.Find("PlayerOne").GetComponent<Player_Movement>();
        playertwo = GameObject.Find("PlayerTwo").GetComponent<Player_Movement>();

        godMode_Hole1 = GameObject.Find("PlayerOne").GetComponent<God_Mode>();
        godMode_Hole2 = GameObject.Find("PlayerTwo").GetComponent<God_Mode>();
    }

    private void Update()
    {
        if (playerone_falling)
        {
            // If the players is falling we scale the sprite to make a falling effect
            if (playerone.transform.localScale.x > 0)
            {
                playerone.transform.localScale = new Vector3(playerone.transform.localScale.x - Time.deltaTime * scale_speed, playerone.transform.localScale.y - Time.deltaTime * scale_speed, playerone.transform.localScale.z);
            }
            //When the scaling ends we reset the position of the player and the chain
            else
            {
                Vector3 pos_torespawn = Find_bestRespawnPoint(playerone);

                int Nump = rope.NumPoints;
                rope.Points[0].transform.position = pos_torespawn;

                Reset_Chain();

                playerone.transform.localScale = new Vector3(1 ,1 , 1);
                playertwo.Allow_Moving();
                playerone.Allow_Moving();
                delay_tmp = 0;

                godMode_Hole1.Hit_verification("PlayerOne", playerone.transform.position, "Fall Damage");
                playerone_falling = false;
            }
        }
        if (playertwo_falling)
        {
            if (playertwo.transform.localScale.x > 0)
            {
                playertwo.transform.localScale = new Vector3(playertwo.transform.localScale.x - Time.deltaTime * scale_speed, playertwo.transform.localScale.y - Time.deltaTime * scale_speed, playertwo.transform.localScale.z);
            }
            else
            {
                Vector3 pos_torespawn = Find_bestRespawnPoint(playertwo);

                int Nump = rope.NumPoints;
                rope.Points[Nump - 1].transform.position = pos_torespawn;

                Reset_Chain();

                playertwo.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                playertwo.Allow_Moving();
                playerone.Allow_Moving();
                delay_tmp_two = 0;

                godMode_Hole2.Hit_verification("PlayerTwo", playertwo.transform.position, "Fall Damage");
                playertwo_falling = false;

            }
        }

        if (delay_tmp < delay)
        {
            delay_tmp += Time.deltaTime;
        }

        if (delay_tmp_two < delay)
        {
            delay_tmp_two += Time.deltaTime;
        }
    }

    public Vector3 Find_bestRespawnPoint(Player_Movement player)
    {
        Vector3 pos_torespawn = gameObject.transform.GetChild(0).transform.position;
        float dist = (gameObject.transform.GetChild(0).transform.position - player.transform.position).magnitude;
        for (int x = 1; x < gameObject.transform.childCount; x++)
        {
            if ((gameObject.transform.GetChild(x).transform.position - player.transform.position).magnitude < dist)
            {
                pos_torespawn = gameObject.transform.GetChild(x).transform.position;
                dist = (gameObject.transform.GetChild(x).transform.position - player.transform.position).magnitude;
            }
        }
        return pos_torespawn;
    }

    // Resets the position of the chain 
    public void Reset_Chain()
    {
        int Nump = rope.NumPoints;
        Vector3 Delta = rope.Points[Nump - 1].transform.position - rope.Points[0].transform.position;

        for (int ParticleIndex = 0; ParticleIndex < Nump; ParticleIndex++)
        {
            float Alpha = (float)ParticleIndex / (float)(Nump - 1);
            Vector3 InitializePosition = rope.Points[0].transform.position + (Alpha * Delta);
            rope.Points[ParticleIndex].transform.position = InitializePosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            Player_Movement pm = collision.gameObject.GetComponent<Player_Movement>();
            if (collision.name == "PlayerOne" && delay_tmp >= delay && pm.dash_v < (pm.dash_delay - pm.dash_time))
            {
                playerone_falling = true;
                playerone.Stop_Moving();
                playertwo.Stop_Moving();
            }
            else if (collision.name == "PlayerTwo" && delay_tmp_two >= delay && pm.dash_v < (pm.dash_delay - pm.dash_time))
            {
                playertwo_falling = true;
                playerone.Stop_Moving();
                playertwo.Stop_Moving();
            }
        }

    }
}
