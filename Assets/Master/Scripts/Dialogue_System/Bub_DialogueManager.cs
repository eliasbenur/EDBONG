﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bub_DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private Queue<float> delays;
    private Queue<float> scales_x;
    private Queue<float> scales_y;
    private Queue<int> font_sizes;
    public GameObject prefab_bubbles;
    private GameObject refObj_bubbles;
    private Vector3 position_to;
    private RectTransform RectT_canvas;
    private bool taunt;
    private Taunt_Manager taunt_mng;
    

    public void Set_position_to(Vector3 new_pos)
    {
        position_to = new_pos;
    }

    void Start()
    {
        sentences = new Queue<string>();
        delays = new Queue<float>();
        scales_x = new Queue<float>();
        scales_y = new Queue<float>();
        font_sizes = new Queue<int>();
        RectT_canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();

        taunt_mng = Camera.main.GetComponent<Taunt_Manager>();
    }

    private void Update()
    {
        Update_Position();
    }

    /*Update the position in the Canvas*/
    public void Update_Position()
    {
        if (refObj_bubbles != null)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(position_to);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * RectT_canvas.sizeDelta.x) - (RectT_canvas.sizeDelta.x * 0.5f) + 60),
            ((ViewportPosition.y * RectT_canvas.sizeDelta.y) - (RectT_canvas.sizeDelta.y * 0.5f) - 15));

            refObj_bubbles.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        }
    }

    /*Load all the data */
    public GameObject StartDialogue(Bub_Dialogue dialogue, Vector3 position, bool taunt_)
    {
        if (refObj_bubbles != null)
        {
            EndDialogue();
        }
        sentences.Clear();
        delays.Clear();
        scales_x.Clear();
        scales_y.Clear();
        font_sizes.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (float delay in dialogue.delays)
        {
            delays.Enqueue(delay);
        }

        foreach (float scale_x in dialogue.Bubble_scale_x)
        {
            scales_x.Enqueue(scale_x);
        }

        foreach (float scale_y in dialogue.Bubble_scale_y)
        {
            scales_y.Enqueue(scale_y);
        }

        foreach (int font_size in dialogue.font_size)
        {
            font_sizes.Enqueue(font_size);
        }

        position_to = position;
        taunt = taunt_;

        refObj_bubbles = Instantiate(prefab_bubbles, Vector3.zero, Quaternion.identity);
        refObj_bubbles.transform.SetParent(GameObject.Find("Canvas").transform);

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(position_to);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * RectT_canvas.sizeDelta.x) - (RectT_canvas.sizeDelta.x * 0.5f) + 60),
        ((ViewportPosition.y * RectT_canvas.sizeDelta.y) - (RectT_canvas.sizeDelta.y * 0.5f) - 15));

        refObj_bubbles.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;

        DisplayNextSentence();

        return refObj_bubbles;
    }

    /* Set Up of the Dialogue */
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        refObj_bubbles.transform.GetChild(0).GetComponent<Text>().fontSize = font_sizes.Dequeue();
        float x_widht = scales_x.Dequeue(); float y_height = scales_y.Dequeue();
        refObj_bubbles.GetComponent<RectTransform>().sizeDelta = new Vector2(x_widht, y_height);
        refObj_bubbles.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(x_widht - 20, y_height - 70);

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    
    /* Delay of the sentences appearing */
    IEnumerator TypeSentence(string sentence)
    {
        refObj_bubbles.transform.GetChild(0).GetComponent<Text>().text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            refObj_bubbles.transform.GetChild(0).GetComponent<Text>().text += letter;
            yield return null;
        }
        yield return new WaitForSeconds(delays.Dequeue());
        DisplayNextSentence();
    }

    void EndDialogue()
    {
        Destroy(refObj_bubbles);
        if (taunt)
        {
            taunt_mng.Reset_vars();
            taunt = false;
        }
    }
}
