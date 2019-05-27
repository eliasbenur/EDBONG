﻿using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {      
        if (collision.tag == "player")
        {
            TriggerDialogue();
            Destroy(gameObject); 
        }
    }
}
