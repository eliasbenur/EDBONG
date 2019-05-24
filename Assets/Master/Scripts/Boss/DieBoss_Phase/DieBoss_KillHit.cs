using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class DieBoss_KillHit : MonoBehaviour
{
    public float timer, timerTotBeforeDead, timerReturn;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public Camera_Focus camera;
    public GameObject canvas;
    public float cooldown;
    public CinematicBars cinematicDie;
    public float scale_Speed;
    public float limitScale_Boss;
    public Material default_sprite;
    public Material flash_sprite;
    public float offset;
    public float speed = 1.0f; //how fast it shakes
    public float amount = 1.0f; //how much it shakes
    float startPosX, startPosY;
    private CapsuleCollider2D colliderBoss;
    public GameObject ropeCollisions;
    List<Transform> targets;

    private void Awake()
    {
        camera = Camera.main.GetComponent<Camera_Focus>();
        
        default_sprite = GetComponent<SpriteRenderer>().material;

        colliderBoss = GetComponent<CapsuleCollider2D>();
        targets = Camera.main.GetComponent<Camera_Focus>().GetCameraTargets();
    }

    private void Update()
    {
        BehaviorCamera();
        if(GameObject.Find("LaserBeam").GetComponent<PolygonCollider2D>() != null)
            GameObject.Find("LaserBeam").GetComponent<PolygonCollider2D>().enabled = false;

        if (camera.transform.position.y >= transform.position.y + offset)
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;
            ShakeSprite();
            GetComponent<SpriteRenderer>().material = flash_sprite;
            if (transform.localScale.x >= limitScale_Boss)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 2, transform.position.z);
                transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * scale_Speed, transform.localScale.y - Time.deltaTime * scale_Speed, transform.localScale.z);
            }
            else
            {
                GetComponent<SpriteRenderer>().material = default_sprite;
                speed = 0;
                colliderBoss.size = new Vector2(10, 10);
                colliderBoss.enabled = true;
                ropeCollisions.SetActive(true);

                //Change here
                ropeCollisions.GetComponent<KillBoss_RopeDetection>().enabled = true;

                camera.GetComponent<Camera_Focus>().enabled = true;
                canvas.SetActive(true);
                var desactivate = GameObject.FindGameObjectsWithTag("player");
                for (int i = 0; i < desactivate.Length; i++)
                {
                    desactivate[i].GetComponent<Animator>().enabled = true;
                    desactivate[i].GetComponent<Player_Movement>().enabled = true;
                    desactivate[i].GetComponent<Player_Movement>().can_move = true;
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].GetComponent<God_Mode>().timerTotGodMode = targets[i].GetComponent<God_Mode>().oldValueTimerGod;
                    targets[i].GetComponent<God_Mode>().godMode = false;
                    targets[i].GetComponent<God_Mode>().timerGodMode = 0;
                }

                foreach(Collider2D c in GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                CapsuleCollider2D collider = gameObject.AddComponent<CapsuleCollider2D>();
                collider.size = new Vector2(10, 10);
                collider.offset = new Vector2(0.1334858f, -1.468382f);
                collider.direction = CapsuleDirection2D.Horizontal;
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

    void ShakeSprite()
    {
        var newX = startPosX + Mathf.Sin(Time.time * speed) * amount;
        //var newY = startPosY + Mathf.Sin(Time.time * speed) * amount;
        transform.position = new Vector3(newX, startPosY, 0);
    }

    void BehaviorCamera()
    {
        canvas.SetActive(false);
        camera.enabled = false;
        var desactivate = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < desactivate.Length; i++)
        {
            desactivate[i].GetComponent<Animator>().enabled = false;
            desactivate[i].GetComponent<Player_Movement>().enabled = false;
        }
        camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
    }
}
