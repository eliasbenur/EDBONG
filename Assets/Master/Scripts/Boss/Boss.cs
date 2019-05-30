﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    #region Settings
    [Header("General Settings Phase")]
    public float timerAttack1;
    public float timerAttack2;
    public float timerAttack3;
    public float timerAttack4;
    public int numberCycle;
    public float timerTotBeforeTransition;

    [Header("Monster Phase Settings")]
    public List<GameObject> spawnPosition;
    public GameObject ennemyToSpawn;

    [Header("Projectile Settings")]
    public float projectile_ToShoot;
    public float Triangle_ToShoot;
    public float cooldown_Between_Projectil;
    public float cooldown_Between_Monsters;
    public float cooldown_Between_Projectil_Triangle;
    public float enemySpeed;
    public float cooldownTrianglePhase;
    public float timeToDo;
    public GameObject Triangle_Projectile;
    public GameObject Projectile_Boss;

    [Header("Laser Settings")]
    public LineRenderer line;
    public float ennemyLaserSpeed;
    public float numberCycleLaser;

    [Header("Eyes Settings")]
    public List<Transform> all_Childrens; 
    public Transform leftEye;
    public Transform rightEye;
    //Falling Eyes
    public GameObject eyeRightFalling;
    public GameObject eyeLeftFalling;

    [Header("Chaos Settings")]
    //Chaos Mode
    public float ennemySpeed_Chaos;
    public float angle_Chaos;
    public float projectileToSpawn_Chaos;
    public float angleToADD_Chaos;
    public float cooldown_Chaos;
    public int timeToDo_Chaos;

    [Header("Camera Speed Cinematic Settings")]
    public float smoothTime = 2f;
    #endregion

    #region Properties
    IEnumerator coroutineFire, spawnCoroutine;
    bool canShoot = true;
    bool canChaos = true;
    bool canSpawn = true;
    private bool L, R, M;
    [HideInInspector] public float timer;     
    private int i, a;
    [HideInInspector] public float x,y,z;
    [HideInInspector] public float angle = 20f;
    [HideInInspector] public int segments;
    [HideInInspector] public float xradius;
    [HideInInspector] public float yradius;    
    [HideInInspector] public bool aller, retour;
    [HideInInspector] public bool checkBeforeNewPhase;
    private float oldCooldownBetweenMonsters;
    Animator bossAnimation;
    Vector2[] myPoints;
    [HideInInspector] public bool alreadyPlay;
    [HideInInspector] public float timerCondition, timerCondition2;     
    private Vector3 position;    
    bool confirmed = true;
    GameObject targetObject;
    bool checkedMonster;   
    private Vector3 velocity;
    private new Camera_Focus camera;
    [HideInInspector]public bool cameraMoving;
    [HideInInspector] public bool returnCamera;
    [HideInInspector] public bool CameraTestOneTime;
    private PolygonCollider2D line_Collider;
    private List<Player_Movement> players;
    [HideInInspector] public List<GameObject> ennemies;
    [HideInInspector] public List<GameObject> eyes;    
    #endregion

    private void Awake()
    {
        cameraMoving = true;
        camera = Camera.main.GetComponent<Camera_Focus>();
        bossAnimation = GetComponent<Animator>();
        oldCooldownBetweenMonsters = cooldown_Between_Monsters;
        foreach (GameObject elements in GameObject.FindGameObjectsWithTag("spawn"))        
            spawnPosition.Add(elements);
        players = Camera.main.GetComponent<GameManager>().players_Movement;
    }

    private void Start()
    {
        i = 0;
        a = 0;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        angle = 90;
        aller = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!cameraMoving && !CameraTestOneTime)
        {
            if(ennemies.Count != 0)
            {
                for(int i =0; i < ennemies.Count; i++)
                {
                    Destroy(ennemies[i]);
                    ennemies.RemoveAt(i);
                }
            }

            camera.enabled = false;

            for (int i = 0; i < players.Count; i++)            
                players[i].Stop_Moving();
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
            cameraMoving = true;
        }

        if(returnCamera)
        {
            camera.enabled = true;
            camera.Update_cam();
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Animator>().enabled = true;
                players[i].Allow_Moving();
            }
            CameraTestOneTime = true;    
        }

        if (checkBeforeNewPhase)
        {
            cameraMoving = false;

            if (all_Childrens.Count == 3)
            {
                if (!alreadyPlay)
                {
                    alreadyPlay = true;
                    bossAnimation.Play("LooseEye_Phase1");
                }
                timerCondition += Time.deltaTime;
                if (timerCondition > 2.5f)
                {
                    if (eyes.Count == 0)
                    {
                        var eyesInstance = Instantiate(eyeRightFalling, new Vector2(rightEye.position.x, rightEye.position.y - 6), Quaternion.identity);
                        eyes.Add(eyesInstance);
                    }
                }
                if(timerCondition > 3.5f)
                    returnCamera = true;
            }
            else if (all_Childrens.Count == 2)
            {
                if (!alreadyPlay)
                {
                    alreadyPlay = true;
                    bossAnimation.Play("LooseEyes_Phase2");
                }
                timerCondition += Time.deltaTime;
                if (timerCondition > 2.5f)
                {
                    if (eyes.Count == 0)
                    {
                        var eyesInstance = Instantiate(eyeLeftFalling, new Vector2(leftEye.position.x, leftEye.position.y - 6), Quaternion.identity);
                        eyes.Add(eyesInstance);
                    }
                }
                if (timerCondition > 3.5f)
                    returnCamera = true;
            }

            if (ennemies.Count == 0)
            {
                timerCondition2 += Time.deltaTime;
                if (timerCondition2 > timerTotBeforeTransition)
                {
                    x = 0;
                    y = 0;
                    angle = 90;
                    retour = false;
                    aller = true;
                    i = 0;
                    a = 0;
                    canSpawn = false;
                    cooldown_Between_Monsters = oldCooldownBetweenMonsters;
                    checkBeforeNewPhase = false;                   
                    timerCondition = 0;
                    timerCondition2 = 0;
                    alreadyPlay = false;
                    if (all_Childrens.Count == 3)
                        bossAnimation.Play("idle_start");
                    else if (all_Childrens.Count == 2)
                        bossAnimation.Play("Idle_Phase2");
                        
                    cameraMoving = true;
                    returnCamera = false;
                    CameraTestOneTime = false;
                    confirmed = false;
                }
                else
                {
                    canSpawn = false;
                    confirmed = false;
                }
            }
        }

        if (canSpawn)
            Monster();
        else
        {
            spawnCoroutine = Spawn_Close_Enemies();
            StopCoroutine(spawnCoroutine);
        }

        if (!checkBeforeNewPhase)
        {
            if (i < numberCycle)
            {
                timer += Time.deltaTime;
                if (all_Childrens.Count == 3)
                {
                    Phase1();
                }
                else if (all_Childrens.Count == 2)
                    Phase2();
                else if (all_Childrens.Count == 1)
                {
                    Phase3();
                    if (i > 1)
                    {
                        StopAllCoroutines();
                        for (int i = 0; i < players.Count; i++)
                            players[i].Stop_Moving();
                        GetComponent<DieBoss_KillHit>().enabled = true;
                        Destroy(this);                     
                    }
                }
                if (eyes.Count != 0)
                    eyes.Clear();
            }
            else
            {
                cooldown_Between_Monsters = 10000000;
                StartCoroutine(Wait());
            }
        }
    }

    void Laser()
    {
        bossAnimation.SetBool("FireLeftPhase1", false);
        bossAnimation.SetBool("FireRightPhase1", false);
        if (line_Collider == null)
        {
            line.gameObject.AddComponent<PolygonCollider2D>();
            line_Collider = line.GetComponent<PolygonCollider2D>();
            line_Collider.isTrigger = true;
        }
        else
        {
            myPoints = line_Collider.points;

            myPoints[1].Set(x, y);
            line_Collider.points = myPoints;
        }

        line.enabled = true;
        if (a < numberCycleLaser)
        {
            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                myPoints = line_Collider.points;

                myPoints[1].Set(x, y);
                line_Collider.points = myPoints;

                line.SetPosition(0, new Vector3(x, y, 0));
                if (aller)
                    angle += ennemyLaserSpeed * Time.fixedDeltaTime;
                else
                    angle -= ennemyLaserSpeed * Time.fixedDeltaTime;
            }
            if (angle < 90)
            {
                aller = true;
                a += 1;
                retour = false;
            }
            else if(angle >270)
            {
                aller = false;
                a += 1;
                retour = true;
            }
        }
        else
        {
            line.enabled = false;
            a = 0;
            i = 0;
            checkBeforeNewPhase = true;
            bossAnimation.SetBool("Laser", false);
            bossAnimation.SetBool("Laser2", false);
            StopAllCoroutines();
            i = 0;
            a = 0;
            x = 0;
            y = 0;
            angle = 90;
            retour = false;
            aller = true;
            canSpawn = true;
        }
    }

    void Monster()
    {
        spawnCoroutine = Spawn_Close_Enemies();
        StartCoroutine(spawnCoroutine);
    }

    void Phase1()
    {
        cooldown_Between_Monsters = oldCooldownBetweenMonsters;
        
        if (!confirmed)
        {
            confirmed = true;
            canSpawn = true;
        }
        if (timer < timerAttack1)
        {
            L = true;
            M = false;
            R = false;
            bossAnimation.SetBool("FireLeftPhase1", true);
            bossAnimation.SetBool("FireRightPhase1", false);
            if (canShoot && L)
            {
                coroutineFire = FireCoroutine(0.5f, leftEye);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1 && timer < timerAttack2)
        {
            L = false;
            M = false;
            R = true;
            bossAnimation.SetBool("FireLeftPhase1", false);
            bossAnimation.SetBool("FireRightPhase1", true);
            if (canShoot && R)
            {
                coroutineFire = FireCoroutine(0.5f, rightEye);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack2 && timer < timerAttack3)
        {
            L = true;
            M = false;
            R = false;
            bossAnimation.SetBool("FireLeftPhase1", true);
            bossAnimation.SetBool("FireRightPhase1", false);
            if (canShoot && L)
            {
                coroutineFire = FireCoroutine(0.5f, leftEye);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack3 && timer < timerAttack4)
        {
            L = false;
            M = false;
            R = true;
            bossAnimation.SetBool("FireLeftPhase1", false);
            bossAnimation.SetBool("FireRightPhase1", true);
            if (canShoot && R)
            {
                coroutineFire = FireCoroutine(0.5f,rightEye);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack4)
        {
            L = false;
            M = false;
            R = false;
            timer = 0;
            i += 1;
            bossAnimation.SetBool("FireLeftPhase1", false);
            bossAnimation.SetBool("FireRightPhase1", false);
        }
    }

    void Phase2()
    {
        cooldown_Between_Monsters = oldCooldownBetweenMonsters;
        if (!confirmed)
        {
            confirmed = true;
            canSpawn = true;
            canShoot = true;
        }
        if (timer < timerAttack1)
        {
            L = true;
            M = false;
            R = false;
            bossAnimation.SetBool("FireLeftPhase2", true);
            if (canShoot && L)
            {
                coroutineFire = FireCoroutine(0.5f, leftEye);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1 && timer < timerAttack2)
        {
            L = false;
            M = false;
            R = false;
        }
        if (timer > timerAttack2 && timer < timerAttack3)
        {
            L = false;
            M = true;
            R = false;
            bossAnimation.SetBool("FireLeftPhase2", false);
            if (canShoot && M)
            {
                coroutineFire = Phase_Triangle_Shot();
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack3 +2)
        {
            L = false;
            M = false;
            R = false;
            timer = 0;
            i += 1;
            canShoot = true;
        }
    }

    void Phase3()
    {
        cooldown_Between_Monsters = 10000000;
        canSpawn = false;

        if (!confirmed)
        {
            confirmed = true;
            canShoot = true;
        }

        if (!checkedMonster)
        {
            checkedMonster = true;
            if(ennemies.Count != 0)
            {
                for(int i =0; i < ennemies.Count; i++)
                {
                    Destroy(players[i]);
                    ennemies.RemoveAt(i);
                }
            }
        }     
        if (timer < timerAttack1)
        {
            L = false;
            M = true;
            R = false;
            if (canShoot && M)
            {
                coroutineFire = Phase_Triangle_Shot();
                StartCoroutine(coroutineFire);
            }
        }

        if (timer > timerAttack1 && timer < timerAttack3)
        {
            if (canChaos)
            {
                coroutineFire = Final_Chaos();
                StartCoroutine(coroutineFire);
            }
        }

        if (timer > timerAttack3)
        {
            L = false;
            M = false;
            R = false;
            angle = 0;
            timer = 0;
            i += 1;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        if (all_Childrens.Count == 3)
            bossAnimation.SetBool("Laser", true);
        else if (all_Childrens.Count == 2)
            bossAnimation.SetBool("Laser2", true);
        yield return new WaitForSeconds(1.5f);
        Laser();
    }

    IEnumerator Spawn_Close_Enemies()
    {
        for (int i = 0; i < spawnPosition.Count; i++)
        {
            var monster = Instantiate(ennemyToSpawn, spawnPosition[i].transform.position, Quaternion.identity);
            monster.tag = "Monster_Phase";
            ennemies.Add(monster);
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown_Between_Monsters);
        canSpawn = true;
    }

    IEnumerator FireCoroutine(float cooldown,Transform eye)
    {
        for (int i = 0; i < projectile_ToShoot; i++)
        {
            int target = Random.Range(1, 3);
            switch (target)
            {
                case 1:
                    targetObject = players[0].gameObject;
                    break;
                case 2:
                    targetObject = players[1].gameObject;
                    break;
            }
            int circle = Random.Range(16, 45);
            GetComponent<CircleCollider2D>().radius = circle;
            var instanceAddForce = Instantiate(Projectile_Boss, new Vector2(eye.transform.position.x, eye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((targetObject.transform.position - eye.transform.position).normalized * enemySpeed);
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = false;
            yield return new WaitForSeconds(cooldown_Between_Projectil);
            canShoot = true;
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator Phase_Triangle_Shot()
    {
        for (int j = 0; j < timeToDo; j++)
        {
            for (int i = 0; i < Triangle_ToShoot; i++)
            {
                var instanceAddForce = Instantiate(Triangle_Projectile, new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;
                position = Random.insideUnitSphere * 10 + transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce((position - instanceAddForce.transform.position).normalized * enemySpeed);
                var direction = position - instanceAddForce.transform.position;
                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                instanceAddForce.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
                canShoot = false;
                yield return new WaitForSeconds(cooldown_Between_Projectil_Triangle);
                canShoot = true;
            }
            canShoot = false;
            yield return new WaitForSeconds(cooldownTrianglePhase);
            canShoot = true;
        }
        canChaos = true;
    }

    IEnumerator Final_Chaos()
    {
        for (int j = 0; j < timeToDo_Chaos; j++)
        {
            for (int i = 0; i < projectileToSpawn_Chaos; i++)
            {
                angle_Chaos += angleToADD_Chaos;
                Vector3 direction = new Vector3(Mathf.Cos(angle_Chaos), Mathf.Sin(angle_Chaos), 0);
                var instanceAddForce = Instantiate(Resources.Load("ShotDistance_Chaos"), transform.position + direction, Quaternion.identity) as GameObject;
                var directionVect = instanceAddForce.transform.position - transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce(directionVect.normalized * ennemySpeed_Chaos);
            }
            canChaos = false;
            yield return new WaitForSeconds(cooldown_Chaos);
            canChaos = true;
        }
        canChaos = false;
    }

}
