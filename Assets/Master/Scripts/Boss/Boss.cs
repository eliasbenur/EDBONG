using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    IEnumerator coroutineFire, spawnCoroutine, wait;
    bool canShoot = true;
    bool canChaos = true;
    public bool canSpawn = true;
    public float enemySpeed;
    public GameObject target;
    public List<Transform> all_Childrens;
    public Transform midEye, leftEye, rightEye;
    public float projectLeft, projectRight, projectMid, projectLeftTriangle;
    public float cooldown_Between_Projectil, cooldown_Between_Monsters, cooldown_Between_Projectil_Triangle;

    public bool L, R, M;
    public float timer, timerAttack1, timerAttack2, timerAttack3, timerAttack4;
    public int numberCycle;

    public List<GameObject> ennemiesList;
    public List<GameObject> spawnPosition;

    public int i, a;
    //public int mid, left, right;

    //Laser
    public float x;
    public float y;
    public float z = 0f;

    public float angle = 20f;
    public int segments;
    public float xradius;
    public float yradius;
    public LineRenderer line;

    public float ennemyLaserSpeed;
    public float numberCycleLaser;

    public bool aller, retour, stop;

    public bool checkBeforeNewPhase;

    public float oldCooldownBetweenMonsters;

    Animator bossAnimation;
    //public GameObject laser;

    PolygonCollider2D colliderPol;
    Vector2[] myPoints;

    public bool alreadyPlay;
    public float timerCondition, timerCondition2, timerTotBeforeTransition;

    //Falling Eyes
    public GameObject eyeRightFalling;
    public GameObject eyeLeftFalling;

    public Vector3 position;
    public float cooldownTrianglePhase;
    public float enemyStart;
    public float timeToDo;

    //Chaos Mode
    public float ennemySpeed_Chaos;

    public float angle_Chaos;
    public float projectileToSpawn_Chaos;
    public float angleToADD_Chaos, cooldown_Chaos;
    public int timeToDo_Chaos;

    bool confirmed = true;

    GameObject targetObject;

    bool checkedMonster;
    public GameObject ennemyToSpawn;

    public float smoothTime = 2f;
    private Vector3 velocity;
    public new Camera_Focus camera;
    public bool cameraMoving;
    public bool returnCamera;
    public bool CameraTestOneTime;

    private void Awake()
    {
        cameraMoving = true;
        camera = Camera.main.GetComponent<Camera_Focus>();
        bossAnimation = GetComponent<Animator>();
        oldCooldownBetweenMonsters = cooldown_Between_Monsters;

        foreach (GameObject elements in GameObject.FindGameObjectsWithTag("spawn"))
        {
            spawnPosition.Add(elements);
        }
        target = GameObject.Find("PlayerOne");
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
            var check = GameObject.FindGameObjectsWithTag("Monster_Phase");
            if (check != null)
            {
                foreach (GameObject element in check)
                {
                    Destroy(element);
                }
            }
            Camera.main.GetComponent<Camera_Focus>().enabled = false;
            var desactivate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < desactivate.Length; i++)
            {
                desactivate[i].GetComponent<Player_Movement>().Stop_Moving();
            }
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
            cameraMoving = true;
        }

        if(returnCamera)
        {
            Camera.main.GetComponent<Camera_Focus>().enabled = true;
            Camera.main.GetComponent<Camera_Focus>().Update_cam();
            var activate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < activate.Length; i++)
            {
                activate[i].GetComponent<Animator>().enabled = true;
                activate[i].GetComponent<Player_Movement>().Allow_Moving();
            }
            CameraTestOneTime = true;    
        }

        if (checkBeforeNewPhase)
        {
            cameraMoving = false;
            var check = GameObject.FindGameObjectsWithTag("Monster_Phase");

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
                    if (!GameObject.FindGameObjectWithTag("Eye") && !GameObject.FindGameObjectWithTag("Eye"))
                    {
                        Instantiate(eyeRightFalling, new Vector2(rightEye.position.x, rightEye.position.y - 6), Quaternion.identity);
                    }     
                }
                if(timerCondition > 3.5f)
                {
                    returnCamera = true;
                }
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
                    if (!GameObject.FindGameObjectWithTag("Eye") && !GameObject.FindGameObjectWithTag("Eye"))
                    {
                        Instantiate(eyeLeftFalling, new Vector2(leftEye.position.x, leftEye.position.y - 6), Quaternion.identity);
                    }
                }
                if (timerCondition > 3.5f)
                {
                    returnCamera = true;
                }
            }

            if (check.Length == 0)
            {
                timerCondition2 += Time.deltaTime;
                if (timerCondition2 > timerTotBeforeTransition)
                {
                    x = 0;
                    y = 0;
                    angle = 90;
                    stop = false;
                    retour = false;
                    aller = true;
                    i = 0;
                    a = 0;
                    canSpawn = false;
                    cooldown_Between_Monsters = oldCooldownBetweenMonsters;
                    checkBeforeNewPhase = false;
                    Destroy(GameObject.FindGameObjectWithTag("Eye"));
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

        if (ennemiesList.Count == 0 && !checkBeforeNewPhase)
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
                        var respawns = GameObject.FindGameObjectsWithTag("monster");
                        foreach (GameObject respawn in respawns)
                        {
                            Destroy(respawn);
                        }
                        //GetComponent<DieBoss_Phase>().enabled = true;
                        var desactivate = GameObject.FindGameObjectsWithTag("player");
                        for (int i = 0; i < desactivate.Length; i++)
                        {
                            desactivate[i].GetComponent<Player_Movement>().Stop_Moving();
                        }
                        GetComponent<DieBoss_KillHit>().enabled = true;
                        Destroy(this);
                        
                    }
                }
            }
            else
            {
                cooldown_Between_Monsters = 10000000;
                StartCoroutine(Wait());
            }
        }
        else
        {
            for (var i = 0; i < ennemiesList.Count; i++)
            {
                if (ennemiesList[i] == null)
                    ennemiesList.RemoveAt(i);
            }
        }
    }

    void Laser()
    {
        bossAnimation.SetBool("FireLeftPhase1", false);
        bossAnimation.SetBool("FireRightPhase1", false);


        if (!line.gameObject.GetComponent<PolygonCollider2D>())
        {
            line.gameObject.AddComponent<PolygonCollider2D>();
            line.gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        }
        else
        {
            colliderPol = line.gameObject.GetComponent<PolygonCollider2D>();
            myPoints = colliderPol.points;

            myPoints[1].Set(x, y);
            colliderPol.points = myPoints;
        }

        line.enabled = true;
        if (a < numberCycleLaser)
        {
            //Debug.Log(a);
            if (aller && angle < 270)
            {
                for (int i = 0; i < (segments + 1); i++)
                {
                    x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                    y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                    colliderPol = line.gameObject.GetComponent<PolygonCollider2D>();
                    myPoints = colliderPol.points;

                    myPoints[1].Set(x, y);
                    colliderPol.points = myPoints;

                    line.SetPosition(0, new Vector3(x, y, 0));
                    angle += ennemyLaserSpeed * Time.deltaTime;
                }
            }
            else if (aller && angle > 270)
            {
                aller = false;
                a += 1;
                retour = true;
            }
            if (retour)
            {
                for (int i = 0; i < (segments + 1); i++)
                {
                    x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                    y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                    myPoints = colliderPol.points;

                    myPoints[1].Set(x, y);
                    colliderPol.points = myPoints;

                    line.SetPosition(0, new Vector3(x, y, 0));
                    angle -= ennemyLaserSpeed * Time.deltaTime;
                }
            }
            if (angle < 90)
            {
                aller = true;
                a += 1;
                retour = false;
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
            stop = false;
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
                coroutineFire = FireCoroutineLeft(0.5f);
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
                coroutineFire = FireCoroutineRight(0.5f);
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
                coroutineFire = FireCoroutineLeft(0.5f);
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
                coroutineFire = FireCoroutineRight(0.5f);
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
                coroutineFire = FireCoroutineLeft(0.5f);
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

            projectLeftTriangle = 6;
        }

        if (!checkedMonster)
        {
            var check = GameObject.FindGameObjectsWithTag("Monster_Phase");
            checkedMonster = true;
            if (check != null)
            {
                foreach (GameObject element in check)
                {
                    Destroy(element);
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
        stop = false;
        yield return new WaitForSeconds(2);
        stop = true;
        if (all_Childrens.Count == 3)
            bossAnimation.SetBool("Laser", true);
        else if (all_Childrens.Count == 2)
            bossAnimation.SetBool("Laser2", true);
        stop = false;
        yield return new WaitForSeconds(1.5f);
        stop = true;
        Laser();
    }

    IEnumerator Spawn_Close_Enemies()
    {
        for (int i = 0; i < spawnPosition.Count; i++)
        {
            var monster = Instantiate(ennemyToSpawn, spawnPosition[i].transform.position, Quaternion.identity);
            monster.tag = "Monster_Phase";
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown_Between_Monsters);
        canSpawn = true;
    }

    IEnumerator FireCoroutineRight(float cooldown)
    {
        for (int i = 0; i < projectRight; i++)
        {
            int target = Random.Range(1, 3);
            switch (target)
            {
                case 1:
                    targetObject = GameObject.Find("PlayerOne");
                    break;

                case 2:
                    targetObject = GameObject.Find("PlayerTwo");
                    break;
            }

            int circle = Random.Range(16, 45);
            GetComponent<CircleCollider2D>().radius = circle;

            var instanceAddForce = Instantiate(Resources.Load("Projectile_Boss"), new Vector2(rightEye.transform.position.x, rightEye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((targetObject.transform.position - rightEye.position).normalized * enemySpeed);
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = false;
            yield return new WaitForSeconds(cooldown_Between_Projectil);
            canShoot = true;
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator FireCoroutineLeft(float cooldown)
    {
        for (int i = 0; i < projectLeft; i++)
        {
            int target = Random.Range(1, 3);
            switch (target)
            {
                case 1:
                    targetObject = GameObject.Find("PlayerOne");
                    break;

                case 2:
                    targetObject = GameObject.Find("PlayerTwo");
                    break;
            }

            int circle = Random.Range(16, 45);
            GetComponent<CircleCollider2D>().radius = circle;

            var instanceAddForce = Instantiate(Resources.Load("Projectile_Boss"), new Vector2(leftEye.transform.position.x, leftEye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((targetObject.transform.position - leftEye.position).normalized * enemySpeed);
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
            for (int i = 0; i < projectLeftTriangle; i++)
            {
                var instanceAddForce = Instantiate(Resources.Load("triangleShot"), new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as GameObject;

                position = Random.insideUnitSphere * 10 + transform.position;
                instanceAddForce.GetComponent<Rigidbody2D>().AddForce((position - instanceAddForce.transform.position).normalized * enemyStart);

                var test = position - instanceAddForce.transform.position;

                float rot_z = Mathf.Atan2(test.y, test.x) * Mathf.Rad2Deg;
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
                //A projectile explode in a number of determined projectile in an angle all around him
            }
            canChaos = false;
            yield return new WaitForSeconds(cooldown_Chaos);
            canChaos = true;
        }
        canChaos = false;
    }

}
