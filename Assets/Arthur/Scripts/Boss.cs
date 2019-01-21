using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    IEnumerator coroutineFire;
    bool canShoot = true;
    public float cooldown;
    public float enemySpeed;
    public GameObject target;
    public List<Transform> all_Childrens;
    public Transform midEye, leftEye, rightEye;
    public float projectLeft, projectRight, projectMid, Monsters_To_Spawn;
    public float cooldown_Between_Projectil, cooldown_Between_Monsters;

    public bool L, R, M;
    public float timer, timerAttack1 , timerAttack2 , timerAttack3, timerAttack4;
    public int numberCycle;

    public List<GameObject> ennemiesList;
    public List<GameObject> spawnPosition;

    private int i, a;
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

    public bool aller, retour;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        foreach(GameObject elements  in GameObject.FindGameObjectsWithTag("spawn"))
        {
            spawnPosition.Add(elements);
        }

        foreach (Transform child in transform)
        {
            if (child.tag == "Boss")
            {
                all_Childrens.Add(child);
                switch (child.name)
                {
                    case "MidlleEye":
                        midEye = child;
                        break;
                    case "LeftEye":
                        leftEye = child;
                        break;
                    case "RightEye":
                        rightEye = child;
                        break;
                    default:
                        break;
                }
            }
        }

        //TODO
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
        if(all_Childrens.Count ==0)
        {
            //Anim de mort
        }
        if (ennemiesList.Count == 0)
        {
            if (i < numberCycle)
            {
                ///Debug.Log(i);
                timer += Time.deltaTime;
                if (all_Childrens.Count == 3)
                {
                    Phase1();
                }                  
                else if (all_Childrens.Count == 2)
                    //Change the second Phase
                    Phase2();
                else if(all_Childrens.Count == 1)
                {
                    //Change the third Phase
                    Phase3();
                }               
            }
            else
            {
               Laser();                
            }               
        }
        else
        {
            for (var i = 0; i < ennemiesList.Count;i++)
            {
                if (ennemiesList[i] == null)
                    ennemiesList.RemoveAt(i);
            }
        }
    }

    void Laser()
    {
        line.enabled = true;
        if (a < numberCycleLaser)
        {
            Debug.Log(a);
            if (aller && angle < 270)
            {
                for (int i = 0; i < (segments + 1); i++)
                {
                    x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                    y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                    //line.SetPosition(0, transform.position);
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

                    //line.SetPosition(0, transform.position);
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
            TransitionPhase();
        }
    }

    void TransitionPhase()
    {
        for (int i = 0; i < spawnPosition.Count; i++)
        {
            var ennemy = Instantiate(Resources.Load("Close_Ennemy"), spawnPosition[i].transform.position, Quaternion.identity) as GameObject;
            ennemy.transform.parent = transform;
            ennemy.GetComponent<IA>().detectionDistance = 50000;
            //transform.parent.GetComponent<Rooms>().stayedRoom = true;
            var parentEnnemy = GameObject.Find(Camera.main.GetComponent<GameManager>().actualRoom);
            ennemy.transform.parent = parentEnnemy.transform;
            ennemiesList.Add(ennemy.gameObject);
        }
        timer = 0;
        i = 0;
    }
/// <summary>
/// ///////////////////////////////////////////////////////////////////////
/// </summary>
    void Phase1()
    {
        if (timer < timerAttack1)
        {
            L = true;
            M = false;
            R = false;
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
            if (canShoot && R)
            {
                coroutineFire = FireCoroutineRight(0.5f);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1 && timer > timerAttack2 && timer < timerAttack3)
        {
            L = true;
            M = false;
            R = false;
            if (canShoot && L)
            {
                coroutineFire = FireCoroutineLeft(0.5f);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1 && timer > timerAttack2 && timer > timerAttack3 && timer < timerAttack4)
        {
            L = false;
            M = false;
            R = true;
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
        }
    }
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////
    /// </summary>
    void Phase2()
    {
        if (timer < timerAttack1)
        {
            L = true;
            M = false;
            R = false;
            if (canShoot && L)
            {
                coroutineFire = FireCoroutineLeft(0.5f);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1 && timer < timerAttack2)
        {
            L = false;
            M = true;
            R = false;
            if (canShoot && M)
            {
                coroutineFire = FireCoroutineMid(0.5f);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack2)
        {
            L = false;
            M = false;
            R = false;
            timer = 0;
            i += 1;
        }
    }
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////
    /// </summary>
    void Phase3()
    {
        if (timer < timerAttack1)
        {
            L = false;
            M = true;
            R = false;
            if (canShoot && M)
            {
                coroutineFire = FireCoroutineMid(0.5f);
                StartCoroutine(coroutineFire);
            }
        }
        if (timer > timerAttack1)
        {
            L = false;
            M = false;
            R = false;
            timer = 0;
            i += 1;
        }
    }

    IEnumerator Spawn_Close_Enemies()
    {
        for (int i = 0; i < Monsters_To_Spawn; i++)
        {
            var enemies = Instantiate(Resources.Load("Close_Ennemy"), spawnPosition[i].transform.position, Quaternion.identity);
            canShoot = false;
            yield return new WaitForSeconds(cooldown_Between_Monsters);
            canShoot = true;
        }
        yield return null;
    }

    IEnumerator FireCoroutineMid(float cooldown)
    {
        for (int i = 0; i < projectMid; i++)
        {
            var instanceAddForce = Instantiate(Resources.Load("Projectile_Boss"), new Vector2(midEye.transform.position.x, midEye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((new Vector3(0, -1, 0) * enemySpeed));
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = false;
            yield return new WaitForSeconds(cooldown_Between_Projectil);
            canShoot = true;
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator FireCoroutineRight(float cooldown)
    {
        for (int i = 0; i < projectRight; i++)
        {
            var instanceAddForce = Instantiate(Resources.Load("Projectile_Boss"), new Vector2(rightEye.transform.position.x, rightEye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((new Vector3(0,-1,0) * enemySpeed));
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
            var instanceAddForce = Instantiate(Resources.Load("Projectile_Boss"), new Vector2(leftEye.transform.position.x, leftEye.transform.position.y), Quaternion.identity) as GameObject;
            instanceAddForce.GetComponent<Rigidbody2D>().AddForce((new Vector3(0, -1, 0) * enemySpeed));
            //We wait a short time, to let the previous element go more forward before spawing an other one 
            canShoot = false;
            yield return new WaitForSeconds(cooldown_Between_Projectil);
            canShoot = true;
        }
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}
