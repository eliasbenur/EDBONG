using UnityEngine;
using UnityEngine.SceneManagement;

public class FallingFloor_HoleDetection : MonoBehaviour
{
    public Player_Movement playerone;
    public Player_Movement playertwo;
    private bool playerone_falling;
    private bool playertwo_falling;

    public float scale_speed;
    public float spin_speed;

    public float delay;
    private float delay_tmp;
    private float delay_tmp_two;
    private Rope_System rope;

    private God_Mode godMode_Hole1, godMode_Hole2;
    public FloorFalling detection;

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
            player.transform.Rotate(0, 0, spin_speed);
        }
        else
        {
            if(detection != null)
                detection.colliderDestroy.enabled = true;
            PlayerData data = SaveSystem.LoadPlayer();
            if (data != null)
            {
                Load.load = true;
                SceneManager.LoadScene(data.level, LoadSceneMode.Single);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            Player_Movement pm = collision.gameObject.GetComponent<Player_Movement>();
            if (collision.name == "PlayerOne" && delay_tmp >= delay && !pm.Dashing() && !playerone_falling)
            {
                playerone_falling = true;
                AkSoundEngine.PostEvent("play_falling", Camera.main.gameObject);
                Stop_Players();
            }
            else if (collision.name == "PlayerTwo" && delay_tmp_two >= delay && !pm.Dashing() && !playertwo_falling)
            {
                playertwo_falling = true;
                AkSoundEngine.PostEvent("play_falling", Camera.main.gameObject);
                Stop_Players();
            }
        }
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

    public void Stop_Players()
    {
        playerone.Stop_Moving();
        playertwo.Stop_Moving();
    }
}
