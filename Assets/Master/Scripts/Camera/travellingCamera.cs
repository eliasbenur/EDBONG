using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class travellingCamera : MonoBehaviour
{
    public GameObject canvas;

    public float smoothTime = 2f;
    private Vector3 velocity;

    public List<GameObject> targets;
    private GameObject target;
    public float offsetCamera;

    public float distance;
    private int i;
    private List<Player_Movement> playersList;

    private void Awake()
    {
        playersList = Camera.main.GetComponent<GameManager>().players_Movement;
    }

    private void Start()
    {
        target = targets[0];
        canvas.SetActive(false);
        i = 0;
        for (int j = 0; j < playersList.Count; j++)
        {
            playersList[j].Stop_Moving();
        }
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < distance)
        {
            i++;
            if(i<targets.Count)
                target = targets[i];
        }
        if(i== targets.Count)
        {         
            GetComponent<Camera_Focus>().enabled = true;
            canvas.SetActive(true);
            for(int j=0; j < playersList.Count;j++)
            {
                playersList[j].Allow_Moving();

            }
            StartCoroutine(Delay_Cinematique());
            
        }
    }

    IEnumerator Delay_Cinematique()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<travellingCamera>().enabled = false;
    }

    private void FixedUpdate()
    {       
        transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(target.transform.position.x, target.transform.position.y - offsetCamera, transform.position.z), ref velocity, smoothTime);
    }
}
