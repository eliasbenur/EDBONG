using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class travellingCamera : MonoBehaviour
{
    public GameObject canvas;
    public CinematicBars cinematic;
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
        if (!Load.cinematic)
        {
            GetComponent<Camera_Focus>().enabled = true;
            gameObject.GetComponent<travellingCamera>().enabled = false;
        }
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
        cinematic.Show(200, 0.8f);
        if (Vector3.Distance(transform.position, target.transform.position) < distance)
        {
            i++;
            if(i<targets.Count)
                target = targets[i];
        }
        if(i== targets.Count)
        {         
            
            canvas.SetActive(true);
            for(int j=0; j < playersList.Count;j++)
            {
                playersList[j].Allow_Moving();

            }
            GetComponent<Camera_Focus>().enabled = true;
            gameObject.GetComponent<travellingCamera>().enabled = false;

        }
    }

    private void FixedUpdate()
    {       
        transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(target.transform.position.x, target.transform.position.y - offsetCamera, transform.position.z), ref velocity, smoothTime);
    }
}
