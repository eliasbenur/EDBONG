using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class DieBoss_KillHit : MonoBehaviour
{
    public float timer, timerTotBeforeDead, timerReturn;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public GameObject camera;
    public GameObject canvas;

    bool checkedMonsters;

    public GameObject dieSmoke1, dieSmoke2;
    bool canSpawn;
    public float cooldown;

    public CinematicBars cinematicDie;

    public GameObject endOfLevelTrap;
    GameManager desactivateGodMode;
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

    private void Awake()
    {
        camera = Camera.main.gameObject;
        desactivateGodMode = Camera.main.GetComponent<GameManager>();
        default_sprite = GetComponent<SpriteRenderer>().material;

        colliderBoss = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        /*if (timer > timerTotBeforeDead && timer < timerReturn)
        {
            GetComponent<Animator>().Play("Die");
            StartCoroutine(Smoke_Dead());
        }*/

        BehaviorCamera();
        if (camera.transform.position.y >= transform.position.y + offset)
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;
            shakeSprite();
            GetComponent<SpriteRenderer>().material = flash_sprite;
            if (transform.localScale.x >= limitScale_Boss)
                transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * scale_Speed, transform.localScale.y - Time.deltaTime * scale_Speed, transform.localScale.z);
            else
            {
                GetComponent<SpriteRenderer>().material = default_sprite;
                speed = 0;
                colliderBoss.size = new Vector2(10, 10);
                colliderBoss.enabled = true;
                ropeCollisions.SetActive(true);
                ropeCollisions.GetComponent<IA_Choice_CUT_SURROUND_DASH>().enabled = true;
                
                camera.GetComponent<Camera_Focus>().enabled = true;
                canvas.SetActive(true);
                var desactivate = GameObject.FindGameObjectsWithTag("player");
                for (int i = 0; i < desactivate.Length; i++)
                {
                    desactivate[i].GetComponent<Animator>().enabled = true;
                    desactivate[i].GetComponent<Player_Movement>().enabled = true;
                    desactivate[i].GetComponent<Player_Movement>().can_move = true;
                }
                desactivateGodMode.timerTotGodMode_p1 = desactivateGodMode.oldValueTimerGod;
                desactivateGodMode.timerTotGodMode_p2 = desactivateGodMode.oldValueTimerGod;
                desactivateGodMode.godMode_p1 = false;
                desactivateGodMode.godMode_p2 = false;
                desactivateGodMode.timerGodMode_p1 = 0;
                desactivateGodMode.timerGodMode_p2 = 0;
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

    

    void shakeSprite()
    {
        var newX = startPosX + Mathf.Sin(Time.time * speed) * amount;
        //var newY = startPosY + Mathf.Sin(Time.time * speed) * amount;
        transform.position = new Vector3(newX, startPosY, 0);
    }

    public IEnumerator Smoke_Dead()
    {
        var position = Random.insideUnitSphere * 5 + transform.position;
        var smokeToSpawn = Random.Range(1, 3);
        var size = Random.Range(1, 6);
        switch (smokeToSpawn)
        {
            case 1:
                var smoke = Instantiate(dieSmoke1, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size, size);
                break;

            case 2:
                smoke = Instantiate(dieSmoke2, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size, size);
                break;
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown);
        if (cooldown > 0.1)
            cooldown *= 0.9f;
        yield return null;
    }

    void BehaviorCamera()
    {
        canvas.SetActive(false);
        camera.GetComponent<Camera_Focus>().enabled = false;
        var desactivate = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < desactivate.Length; i++)
        {
            desactivate[i].GetComponent<Animator>().enabled = false;
            desactivate[i].GetComponent<Player_Movement>().enabled = false;
        }
        camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
    }
}
