using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    private SpriteRenderer spriteTrap;
    private BoxCollider2D trapCollider;

    public float timerOff, timerTotOff;
    public float timerOn, timerTotOn;

    private void Awake()
    {
        spriteTrap = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timerOff += Time.deltaTime;
        if (timerOff > timerTotOff)
        {
            spriteTrap.enabled = true;
            timerOn += Time.deltaTime;
            trapCollider.enabled = true;
            if(timerOn>timerTotOn)
            {
                spriteTrap.enabled = false;
                trapCollider.enabled = false;
                timerOn = 0;
                timerOff = 0;
            }
        }
            
    }
}
