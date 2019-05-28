using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class DieBoss_KillHit : MonoBehaviour
{
    #region Properties

    //Camera Behavior
    public float smoothTime = 2f;
    [HideInInspector] public new Camera_Focus camera;
    private Vector3 velocity;
    public float offset;
     
    public GameObject canvas;
    public CinematicBars cinematicDie;

    //Effect on the boss
    public float scale_Speed;
    public float limitScale_Boss;
    [HideInInspector] public Material default_sprite; 
    [HideInInspector] public Material flash_sprite;    
    public float speed; //how fast it shakes
    public float amount; //how much it shakes
    float startPosX, startPosY;
    private CapsuleCollider2D colliderBoss;

    [HideInInspector] public KillBoss_RopeDetection ropeCollisions;

    private PolygonCollider2D laser;
    private SpriteRenderer sprite_Renderer;
    private List<Player_Movement> players;
    private List<God_Mode> players_gm;

    IEnumerator cameraShake;

    #endregion

    private void Awake()
    {
        camera = Camera.main.GetComponent<Camera_Focus>();      
        default_sprite = GetComponent<SpriteRenderer>().material;
        colliderBoss = GetComponent<CapsuleCollider2D>();
        laser = GetComponentInChildren<LaserCollision>().GetComponent<PolygonCollider2D>();
        sprite_Renderer = GetComponent<SpriteRenderer>();
        ropeCollisions = GetComponentInChildren<KillBoss_RopeDetection>();
        players = Camera.main.GetComponent<GameManager>().players_Movement;
        players_gm = Camera.main.GetComponent<GameManager>().players;
    }

    private void Update()
    {
        BehaviorCamera();
        if(laser != null)
            laser.enabled = false;

        if (camera.transform.position.y >= transform.position.y + offset)
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;

            cameraShake = ShakeSprite();
            StartCoroutine(cameraShake);

            sprite_Renderer.material = flash_sprite;
            if (transform.localScale.x >= limitScale_Boss)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 2, transform.position.z);
                transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * scale_Speed, transform.localScale.y - Time.deltaTime * scale_Speed, transform.localScale.z);
            }
            else
            {
                sprite_Renderer.material = default_sprite;

                StopCoroutine(cameraShake);

                colliderBoss.size = new Vector2(10, 10);
                colliderBoss.enabled = true;
                ropeCollisions.gameObject.SetActive(true);

                //Change here
                ropeCollisions.enabled = true;

                camera.enabled = true;
                canvas.SetActive(true);

                for (int i = 0; i < players.Count; i++)
                {
                    players[i].GetComponent<Animator>().enabled = true;
                    players[i].enabled = true;
                    players[i].can_move = true;
                }

                for (int i = 0; i < players_gm.Count; i++)
                {
                    players_gm[i].timerTotGodMode = players_gm[i].oldValueTimerGod;
                    players_gm[i].godMode = false;
                    players_gm[i].timerGodMode = 0;
                }

                foreach(Collider2D c in GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                CapsuleCollider2D collider = gameObject.AddComponent<CapsuleCollider2D>();
                collider.size = new Vector2(10, 10);
                collider.offset = new Vector2(0.1334858f, -1.468382f);

                cinematicDie.Hide(0.001f);

                AnalyticsEvent.Custom("Boss Completed", new Dictionary<string, object>
                    {
                        { "Scene", SceneManager.GetActiveScene().name },
                        { "Room" , transform.name }
                    });

                Destroy(this);
            }
        }           
    }

    IEnumerator ShakeSprite()
    {
        var newX = startPosX + Mathf.Sin(Time.time * speed) * amount;
        transform.position = new Vector3(newX, startPosY, 0);
        yield return null;
    }

    void BehaviorCamera()
    {
        canvas.SetActive(false);
        camera.enabled = false;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<Animator>().enabled = false;
            players[i].enabled = false;
        }
        camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
    }
}
