using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    public int money;

    [SerializeField]
    public List<GameObject> listItemDisplay;
    public bool KeyPressed;

    //public UParticleSystem_E_Modif RopePowerUp_Malus;
    public Player_Movement checkPlayer;
    public Player_Movement checkPlayer2;

    public string actualRoom;

    // GodMode P1
    public float timerGodMode_p1;
    public float timerTotGodMode_p1;
    public bool godMode_p1;
    // GodMode P2
    public float timerGodMode_p2;
    public float timerTotGodMode_p2;
    public bool godMode_p2;

    public float life;
    public float shieldPoint;

    //Control of the life
    public GameObject gameOverCanvas;
    private Slider displayLife;
    public GameObject lifeDisplay;

    public GameObject shieldGameObject;
    private Slider shieldDisplay;



    //Items Shop
    public float regenLifeItem;
    public float maxLife;
    public AudioSource shopGuy;
    public AudioClip healSound, shieldSound, speedBoost;

    public float regenShieldItem;
    public float speedUp;

    public float oldValueTimerGod;

    public int num_hits = 0;

    public float player_X;
    public float player_Y;


    //Test render Camera MiniMap
    public Camera miniMapRendererCamera;
    public float  smoothTime;
    public Vector3 velocity;
    public GameObject petit1, petit2;
    public GameObject grand;

    public void Awake()
    {     
        oldValueTimerGod = timerTotGodMode_p1;

        listItemDisplay.AddRange(GameObject.FindGameObjectsWithTag("Item"));

        life = 20;

        maxLife = life;

        shieldPoint = 0;

        shieldDisplay = GameObject.Find("SliderShield").GetComponent<Slider>();
        shieldGameObject.SetActive(false);
        Time.timeScale = 1;
        gameOverCanvas.SetActive(false);

        shieldPoint = 0;
        //Display life on UI
        displayLife = GameObject.Find("SliderHealth").GetComponent<Slider>();
        lifeDisplay.SetActive(true);
        displayLife.maxValue = life;
        money = 0;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        SoundManager.Initialize();

        player_X = 0;
        player_Y = 0;
    }

    public void Start()
    {
        if (PlayerPrefs.GetFloat("player_X", Camera.main.GetComponent<GameManager>().player_X) != 0 && PlayerPrefs.GetFloat("player_Y", Camera.main.GetComponent<GameManager>().player_Y) != 0)
        {
            if (checkPlayer != null && checkPlayer2!=null)
            {
                int Nump = GameObject.Find("Rope_System").GetComponent<Rope_System>().NumPoints;
                Rope_System rope = GameObject.Find("Rope_System").GetComponent<Rope_System>();

                rope.Points[0].transform.position = new Vector3(PlayerPrefs.GetFloat("player_X", Camera.main.GetComponent<GameManager>().player_X), PlayerPrefs.GetFloat("player_Y", Camera.main.GetComponent<GameManager>().player_Y), 0);
                rope.Points[Nump - 1].transform.position = new Vector3(PlayerPrefs.GetFloat("player_X", Camera.main.GetComponent<GameManager>().player_X), PlayerPrefs.GetFloat("player_Y", Camera.main.GetComponent<GameManager>().player_Y), 0);

                Vector3 Delta = rope.Points[Nump - 1].transform.position - rope.Points[0].transform.position;
                for (int ParticleIndex = 0; ParticleIndex < Nump; ParticleIndex++)
                {
                    float Alpha = (float)ParticleIndex / (float)(Nump - 1);
                    Vector3 InitializePosition = rope.Points[0].transform.position + (Alpha * Delta);
                    rope.Points[ParticleIndex].transform.position = InitializePosition;
                }
            }
            else
                Debug.Log("Please Check : Player 1 or Player 2 missing");
        }
    }

    private void Update()
    {
        //miniMapRendererCamera.transform.position = Vector3.SmoothDamp(miniMapRendererCamera.transform.position, new Vector3(miniMapRendererCamera.transform.position.x, miniMapRendererCamera.transform.position.y, -200),ref velocity, smoothTime);
        /*var h = petit1.GetComponent<RectTransform>().rect.height;
        h = Mathf.Lerp(h, 1920,0.045f);
        var w = petit1.GetComponent<RectTransform>().rect.width;
        h = Mathf.Lerp(h, 1080, 0.045f);
        petit1.GetComponent<RectTransform>().sizeDelta = new Vector2(h, w);

        h = petit2.GetComponent<RectTransform>().rect.height;
        h = Mathf.Lerp(h, 1920, 0.045f);
        w = petit2.GetComponent<RectTransform>().rect.width;
        h = Mathf.Lerp(h, 1080, 0.045f);
        petit2.GetComponent<RectTransform>().sizeDelta = new Vector2(h, w);

        var hg = grand.GetComponent<RectTransform>().rect.height;
        hg = Mathf.Lerp(h, 1910, 0.045f);
        var wg = grand.GetComponent<RectTransform>().rect.width;
        wg = Mathf.Lerp(h, 1070, 0.045f);
        petit1.GetComponent<RectTransform>().sizeDelta = new Vector2(hg, wg);*/


        if (shieldPoint == 0)
        {
            shieldGameObject.SetActive(false);
        }
        else
        {
            shieldGameObject.SetActive(true);
            shieldDisplay.value = shieldPoint;
        }

        displayLife.value = life;

        for (int i = 0; i < listItemDisplay.Count; i++)
        {
            //  0/2 in case we have items with a price of two numbers
            var priceItem = listItemDisplay[i].GetComponentInChildren<Text>().text.Substring(0, 2);
            if (int.Parse(priceItem) > money)
            {
                listItemDisplay[i].GetComponentInChildren<Text>().color = Color.red;
            }
            else
            {
                listItemDisplay[i].GetComponentInChildren<Text>().color = Color.white;
            }
        }

        /*if (life <= 0)
        {
            gameOverCanvas.SetActive(true);
            lifeDisplay.SetActive(false);
            //checkPlayer.Vibrate_Control(0, 0);
            //checkPlayer.enabled = false;
            Time.timeScale = 0;

        }*/

        if (godMode_p1)
        {
            timerGodMode_p1 += Time.deltaTime;
            if (timerGodMode_p1 > timerTotGodMode_p1)
            {
                godMode_p1 = false;
                timerGodMode_p1 = 0;
            }
        }
        if (godMode_p2)
        {
            timerGodMode_p2 += Time.deltaTime;
            if (timerGodMode_p2 > timerTotGodMode_p2)
            {
                godMode_p2 = false;
                timerGodMode_p2 = 0;
            }
        }

        if (KeyPressed && (Input.GetKeyDown(KeyCode.R) || checkPlayer.rew_player.GetButtonDown("Items") || checkPlayer.rew_player.GetButtonDown("Items_p1") || checkPlayer.rew_player.GetButtonDown("Items_p2"))/* || Input.GetKey(KeyCode.Joystick2Button0)*/&& money >= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2)))
        {
            for (int i = 0; i < listItemDisplay.Count; i++)
            {
                if (listItemDisplay[i] == checkPlayer.collisionItems.gameObject)
                {
                    switch (checkPlayer.collisionItems.gameObject.name)
                    {
                        case "HealthObject":
                            Debug.Log("Health Potion buy");

                            life += regenLifeItem;
                            if (life > maxLife)
                                life = maxLife;

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = healSound;
                            shopGuy.Play();

                            KeyPressed = false;

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);

                            break;

                        case "ShieldObject":
                            Debug.Log("Shield Potion buy");

                            shieldPoint += regenShieldItem;
                            if (shieldPoint > shieldDisplay.maxValue)
                                shieldPoint = shieldDisplay.maxValue;

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = shieldSound;
                            shopGuy.Play();

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);

                            break;

                        case "SpeedBoost":
                            Debug.Log("Speed Potion buy");

                            var players = GameObject.FindGameObjectsWithTag("player");
                            if (players != null)
                            {
                                foreach (GameObject player in players)
                                {
                                    player.GetComponent<Player_Movement>().speed = speedUp;
                                }
                            }

                            money -= int.Parse(checkPlayer.collisionItems.GetComponentInChildren<Text>().text.Substring(0, 2));

                            shopGuy.clip = speedBoost;
                            shopGuy.Play();

                            listItemDisplay.RemoveAt(i);
                            Destroy(checkPlayer.collisionItems.gameObject);
                            break;
                    }
                }
            }
        }
    }

    public void Hit_verification(string player, Vector3 pos, string who_hit)
    {
        if (player == "PlayerOne" && !godMode_p1)
        {
            Hit(player, pos, who_hit);
            godMode_p1 = true;
        }
        else if (player == "PlayerTwo" && !godMode_p2)
        {
            Hit(player, pos, who_hit);
            godMode_p2 = true;
        }
        else if (player == "PlayerUndefined" && !(godMode_p1 && godMode_p2))
        {
            Hit(player, pos, who_hit);
            godMode_p1 = true;
            godMode_p2 = true;
        }
        else if (player == "TwoOfThem" && !(godMode_p1 && godMode_p1))
        {
            Hit(player, pos, who_hit);
            godMode_p1 = true;
            godMode_p2 = true;
        }
    }

    public void Check_GameOver(Vector3 pos, string who_hit)
    {
        if (life <= 0)
        {
            AnalyticsEvent.Custom("Death", new Dictionary<string, object>
            {
                { "pos", pos },
                { "who_hit" , who_hit }
            });

            gameOverCanvas.SetActive(true);
            lifeDisplay.SetActive(false);
            Time.timeScale = 0;
        }

    }

    public void Hit(string player, Vector3 pos, string who_hit)
    {
        AnalyticsEvent.Custom("Hit", new Dictionary<string, object>
        {
            { "player_hit", player },
            { "pos", pos },
            { "who_hit" , who_hit }
        });

        num_hits++;

        SoundManager.PlaySound(SoundManager.Sound.PlayerGetHit, pos);

        if (shieldPoint <= 0)
            life -= 1;
        else
            shieldPoint -= 1;

        //audio_ouff.Play();
        List<Transform> targets = GetComponent<Camera_Focus>().GetCameraTargets();
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].name == "PlayerOne")
                targets[i].GetComponent<Player_Movement>().startBlinking = true;
            else if (targets[i].name == "PlayerTwo")
                targets[i].GetComponent<Player_Movement>().startBlinking = true;
        }

        Check_GameOver(pos, who_hit);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Go_ToMainMenu()
    {
        SceneManager.LoadScene("Menu_Principal");
    }
}
