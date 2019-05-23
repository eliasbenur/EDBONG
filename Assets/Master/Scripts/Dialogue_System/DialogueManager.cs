using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour{

    private Queue<string> names;
    private Queue<string> sentences;
    private Queue<Sprite> sprites;
    public Text nameText;
    public Text dialogueText;
    public Image image;
    public Animator animator;

    private Player_Movement p1, p2;

    // Start is called before the first frame update
    void Start()
    {
        names = new Queue<string>();
        sentences = new Queue<string>();
        sprites = new Queue<Sprite>();

        p1 = GameObject.Find("PlayerOne").GetComponent<Player_Movement>();
        p2 = GameObject.Find("PlayerTwo").GetComponent<Player_Movement>();
    }

    private void Update()
    {
        /* Next Dialogue When Dash Button is Used */
        if (p1.rew_player.GetButtonDown("Dash") || p2.rew_player.GetButtonDown("Dash"))
        {
            if (sentences.Count != 0)
            {
                DisplayNextSentence();
            }else if (sentences.Count == 0 && animator.GetBool("IsOpen"))
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {

        animator.SetBool("IsOpen", true);

        sentences.Clear();
        names.Clear();
        sprites.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (string sentence in dialogue.names)
        {
            names.Enqueue(sentence);
        }

        foreach (Sprite sentence in dialogue.sprites)
        {
            sprites.Enqueue(sentence);
        }

        p1.Stop_Moving();
        p2.Stop_Moving();

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        Sprite sprite = sprites.Dequeue();
        image.sprite = sprite;

        string name = names.Dequeue();
        nameText.text = name;

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        GameObject.Find("PlayerOne").GetComponent<Player_Movement>().can_move = true;
        GameObject.Find("PlayerTwo").GetComponent<Player_Movement>().can_move = true;
    }


}
