using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {      
        PlayerPrefs.SetFloat("player_X", Camera.main.GetComponent<GameManager>().transform.position.x);
        PlayerPrefs.SetFloat("player_Y", Camera.main.GetComponent<GameManager>().transform.position.y);


        if (collision.tag == "player")
        {
            TriggerDialogue();
            Destroy(gameObject); 
        }
    }

}
