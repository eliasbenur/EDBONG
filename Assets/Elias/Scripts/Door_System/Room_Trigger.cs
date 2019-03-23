﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Trigger : MonoBehaviour
{

    private bool players_inside;
    private int NumPlayer_inside = 0;
    public List<GameObject> enemy_list;
    private int num_doors = 0;

    public void Awake()
    {
        for (int x = 0; x < gameObject.transform.childCount; x++)
        {
            if (gameObject.transform.GetChild(0).tag == "door")
            {
                num_doors += 1;
            }
        }
    }


    private void Update()
    {
        if (gameObject.transform.childCount > 0 + num_doors)
        {
            if (gameObject.transform.GetChild(0).gameObject.transform.childCount == 0)
            {
                NextRound();
                Destroy(gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    private void FirstRound()
    {
        
        for (int x = 0; x < gameObject.transform.GetChild(0).gameObject.transform.childCount; x++)
        {
            gameObject.transform.GetChild(0).transform.GetChild(x).gameObject.SetActive(true);
        }
        
    }

    private void NextRound()
    {
        if (gameObject.transform.childCount > 1 + num_doors)
        {
            for (int x = 0; x < gameObject.transform.GetChild(1).gameObject.transform.childCount; x++)
            {
                gameObject.transform.GetChild(1).transform.GetChild(x).gameObject.SetActive(true);
            }
        }
        else{
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside++;

            if (NumPlayer_inside == 2)
            {
                FirstRound();
            }
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside--;
        }
    }
}
