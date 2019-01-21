using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Focus_Rope : MonoBehaviour
{
    public List<GameObject> node_Target = new List<GameObject>();
    public List<GameObject> all_Players = new List<GameObject>();
    public float detectionDistance;
    public GameObject target;
    public float enemySpeed;
    LineRenderer LR;

    public float gotaGoFast;

    public bool yes;


    private void Awake()
    {
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("player"))
        {
            all_Players.Add(Obj);
        }
    }

    private void Start()
    {
        LR = gameObject.GetComponent<LineRenderer>();
        LR.startWidth = 0.2f;
        LR.endWidth = 0.2f;
        LR.startColor = Color.yellow;
        LR.endColor = Color.yellow;
        LR.SetPosition(1, gameObject.transform.position);
        LR.SetPosition(0, gameObject.transform.position);
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        LR.material = whiteDiffuseMat;
    }

    // Update is called once per frame
    void Update()
    {
        if(yes)
        {
            var test = target.GetComponent<Transform>();
            var test2 = -test.up;
            Debug.Log(test2);
            //Vector2 test = Vector2.Perpendicular(target.transform.position);
/*
            Vector3 side1 = all_Players[1].transform.position - transform.position;
            Vector3 side2 = Vector2.Perpendicular(transform.position);

            Vector3 perp = Vector3.Cross(side1, side2);*/

            /*var perpLength = perp.magnitude;
            perp /= perpLength;*/

            //Debug.Log(perp);
            
            /*LR.SetPosition(1, new Vector2(transform.position.x, transform.position.y));
            LR.SetPosition(0, new Vector2(transform.position.x, transform.position.y) + test.normalized * 10);
            Debug.Log(-test);*/

            //Vector2 direction = (-test - new Vector2(transform.position.x, transform.position.y)).normalized;

            transform.position += new Vector3(test2.x,test2.y,0) * gotaGoFast * Time.deltaTime;


        }


        if (/*transform.parent.GetComponent<Rooms>().stayedRoom &&*/ target == null)
        {
            foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("rope"))
            {
                node_Target.Add(Obj);
            }

            var TargetToFound = Mathf.Round(node_Target.Count / 2);
            for(int i = 0; i <node_Target.Count; i++)
            {
                if(i == TargetToFound)
                {
                    target = node_Target[i];
                }
            }
        }

        if (target != null && !yes)
        {
            if (GetDistance(target) < detectionDistance)
            {
                AttackTheRope();
            }

            //Look at the Target
            if (target != null)
            {
                transform.LookAt(target.transform.position);
                transform.Rotate(new Vector2(0, 90));
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            }
        }       
    }

    float GetDistance(GameObject obj)
    {
        float distance = Vector2.Distance(obj.transform.position, transform.position);
        return distance;
    }

    void AttackTheRope()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * enemySpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "rope")
        {
            /*Vector3 side1 = all_Players[0].transform.position - transform.position;
            Vector3 side2 = all_Players[1].transform.position - transform.position;

            Vector3 perp = Vector3.Cross(side1, side2);

            var perpLength = perp.magnitude;
            perp /= perpLength;

            LR.SetPosition(1, gameObject.transform.position);
            LR.SetPosition(0, gameObject.transform.position + perp.normalized * 10);*/

            //Debug.Log(perp);

            /*Vector3 side1 = all_Players[0].transform.position - all_Players[1].transform.position;
            Vector3 side2 = target.transform.position - transform.position;

            Vector3 perp = Vector3.Cross(side1, side2);

            Debug.Log(perp);

            var perpLength = perp.magnitude;
            perp /= perpLength;

            LR.SetPosition(1, gameObject.transform.position);
            LR.SetPosition(0, gameObject.transform.position + perp.normalized * 10); */

            
            yes = true;
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        /*if (collision.gameObject.tag == "rope")
        {
            var TargetToFound = Mathf.Round(node_Target.Count / 2);
            for (int i = 0; i < node_Target.Count; i++)
            {
                if (i == TargetToFound)
                {
                    target = node_Target[i];
                }
            }
        }*/
    }
}
