using UnityEngine;

public class Bub_DialogueTrigger : MonoBehaviour
{
    public Bub_Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<Bub_DialogueManager>().StartDialogue(dialogue, transform.position, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            TriggerDialogue();
            gameObject.SetActive(false);
        }

    }
}
