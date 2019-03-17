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

    // Start is called before the first frame update
    void Start()
    {
        names = new Queue<string>();
        sentences = new Queue<string>();
        sprites = new Queue<Sprite>();
    }

    public void StartDialogue(Dialogue dialogue)
    {

        animator.SetBool("IsOpen", true);

        //nameText.text = dialogue.name;

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
        //dialogueText.text = sentence;
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
    }
}
