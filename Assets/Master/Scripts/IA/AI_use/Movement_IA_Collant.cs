using System.Collections;
using UnityEngine;

public class Movement_IA_Collant : MonoBehaviour
{
    #region Properties
    //Distance limit where players can be detected
    public float detectionDistance;
    //Movement speed of the monster
    public float enemySpeed;
    [HideInInspector] public GameObject point_to_coll;
    private Initialisation_Rope init_IA;
    private Detection_dash_Distance checkDistance;
    private GameManager players;
    [HideInInspector] public Rope_System rope_system;
    #endregion

    private void Awake()
    {
        init_IA = GetComponent<Initialisation_Rope>();
        players = Camera.main.GetComponent<GameManager>();
        checkDistance = GetComponent<Detection_dash_Distance>();
        rope_system = GameObject.Find("Rope_System").GetComponent<Rope_System>();
    }

    private void Update()
    {
        if (init_IA.target == null)
        {
            for(int i = 0; i< players.players_Movement.Count; i++)
                init_IA.allPlayers.Add(players.players_Movement[i].gameObject);

            if (rope_system.get_points().Count > 0)
                init_IA.target = rope_system.get_points()[init_IA.rope_system.NumPoints / 2].gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (init_IA.target != null)
        {
            if (checkDistance.GetDistance(init_IA.target) < detectionDistance)
                Follow();
        }
    }

    void Follow()
    {
        if (point_to_coll != null)
            transform.position = point_to_coll.transform.position;
        else
        {
            Vector3 Delta = init_IA.target.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + Delta.normalized * Time.fixedDeltaTime * enemySpeed);
        }
    }

    public IEnumerator Dead()
    {
        GetComponent<Animator>().SetBool("dead", true);
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
