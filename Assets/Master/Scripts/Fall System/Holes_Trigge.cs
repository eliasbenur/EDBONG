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
            Falling(playerone);
        }
        if (playertwo_falling)
        {
            Falling(playertwo);
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

    public void Falling(Player_Movement player)
    {
        if (player.transform.localScale.x > 0)
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x - Time.deltaTime * scale_speed, player.transform.localScale.y - Time.deltaTime * scale_speed, player.transform.localScale.z);
        }
        else
        {
            Vector3 pos_torespawn = Find_bestRespawnPoint(player);

            int Nump = rope.NumPoints;
            if (player.name == "PlayerOne")
            {
                rope.get_points()[0].transform.position = pos_torespawn;
                player.transform.localScale = new Vector3(1, 1, 1);
                godMode_Hole2.Hit_verification("PlayerOne", player.transform.position, "Fall Damage");
                playerone_falling = false;
                delay_tmp = 0;
            }
            else{
                rope.get_points()[Nump - 1].transform.position = pos_torespawn;
                player.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                godMode_Hole2.Hit_verification("PlayerTwo", player.transform.position, "Fall Damage");
                playertwo_falling = false;
                delay_tmp_two = 0;
            }
            Reset_Chain();
            playerone.Allow_Moving();
            playertwo.Allow_Moving();
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
        Vector3 Delta = rope.get_points()[Nump - 1].transform.position - rope.get_points()[0].transform.position;

        for (int ParticleIndex = 0; ParticleIndex < Nump; ParticleIndex++)
        {
            float Alpha = (float)ParticleIndex / (float)(Nump - 1);
            Vector3 InitializePosition = rope.get_points()[0].transform.position + (Alpha * Delta);
            rope.get_points()[ParticleIndex].transform.position = InitializePosition;
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
                Stop_Players();
            }
            else if (collision.name == "PlayerTwo" && delay_tmp_two >= delay && pm.dash_v < (pm.dash_delay - pm.dash_time))
            {
                playertwo_falling = true;
                Stop_Players();
            }
        }
    }

    public void Stop_Players()
    {
        playerone.Stop_Moving();
        playertwo.Stop_Moving();
    }
}
