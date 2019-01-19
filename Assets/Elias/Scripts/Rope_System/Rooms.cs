using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour
{
    private GameManager manager;
    public List<Transform> currentEnnemies;

    private void Update()
    {

    }

    void add_enemies_tolist()
    {
        manager = Camera.main.GetComponent<GameManager>();
        foreach (Transform child in transform)
        {
            if (child.tag == "ennemies")
            {
                currentEnnemies.Add(child);
                //if(no current ennemies, it will allow all doors associated to actual room to be open
            }
        }
    }

}
