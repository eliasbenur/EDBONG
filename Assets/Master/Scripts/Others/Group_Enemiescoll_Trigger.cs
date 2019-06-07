﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group_Enemiescoll_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            foreach (Transform child in transform)
            {
                StartCoroutine(child.GetComponent<Movement_IA_Collant>().Dead());
            }
            Destroy(gameObject);
        }
    }
}
