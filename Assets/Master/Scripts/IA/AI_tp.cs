using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_tp : MonoBehaviour{

    public float delay_tp;
    float curr_delay_tp;

    public List<Transform> targets;

    public Rope_System rope_system;

    // Start is called before the first frame update
    void Start()
    {
        curr_delay_tp = delay_tp;
        targets.Clear();
        targets.Add(rope_system.get_points()[0].transform);
        targets.Add(rope_system.get_points()[rope_system.NumPoints - 1].transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (curr_delay_tp > 0)
        {
            curr_delay_tp -= Time.deltaTime;
        }
        else
        {
            if ((targets[0].transform.position - transform.position).magnitude < 5 || (targets[1].transform.position - transform.position).magnitude < 5)
            {
                Tepe();
                curr_delay_tp = delay_tp;

            }

        }

    }

    void Shoot()
    {
        Debug.Log("PiuPiu");
    }

    void Tepe()
    {
        Vector3 new_pos = GetCenterPoint();
        Vector3 dir = new_pos - transform.position;
        float angle_new_pos = Vector2.SignedAngle(Vector2.right, dir);

        float angle = Random.Range(-45, 45);
        angle = (angle_new_pos + angle) * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        LayerMask lm = LayerMask.GetMask("Walls");
        RaycastHit2D hit = Physics2D.Raycast(new_pos, direction, 15, lm);

        if (hit.collider != null)
        {
            transform.position = new_pos + direction.normalized * (hit.distance -2);
        }
        else
        {
            transform.position = new_pos + direction.normalized * 15;
        }
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
