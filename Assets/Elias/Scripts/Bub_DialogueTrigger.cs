using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bub_DialogueTrigger : MonoBehaviour
{
    public Bub_Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<Bub_DialogueManager>().StartDialogue(dialogue, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            TriggerDialogue();
        }

    }
}
