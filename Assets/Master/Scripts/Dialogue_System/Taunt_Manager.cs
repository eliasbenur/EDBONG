using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt_Manager : MonoBehaviour
{
    public int taunt_val_max;
    private int taun_val;
    public List<Bub_Dialogue> list_dialogues;
    //If the dialogue is going to be from the same player, or is comming from the other player
    public List<bool> taunt_same_player;
    private Bub_DialogueManager dialogue_mng;
    private GameObject ref_bub_dialogue;
    private GameObject player_ref;

    private GameObject player_one;
    private GameObject player_two;

    // Start is called before the first frame update
    void Start()
    {
        taun_val = taunt_val_max;
        dialogue_mng = GameObject.Find("Bub_DialogueManager").GetComponent<Bub_DialogueManager>();

        player_one = GameObject.Find("PlayerOne");
        player_two = GameObject.Find("PlayerTwo");
    }

    private void Update()
    {
        if (ref_bub_dialogue != null)
        {
            dialogue_mng.Set_position_to(player_ref.transform.position + new Vector3(0,0.5f,0));
        }
    }

    public void Update_Taunt(GameObject player)
    {
        taun_val--;
        int random_var = Random.Range(0, taun_val);
        if (random_var == 0)
        {
            Make_Taunt(player);
        }
    }

    public void Make_Taunt(GameObject player)
    {
        Debug.Log("TAUNT!");
        int random_dialogue = Random.Range(0, list_dialogues.Count);
        if (taunt_same_player[random_dialogue])
        {
            ref_bub_dialogue = FindObjectOfType<Bub_DialogueManager>().StartDialogue(list_dialogues[random_dialogue], player.transform.position + new Vector3(0, 0.5f, 0), true);
            player_ref = player;
        }
        else
        {
            if (player.name == "PlayerOne")
            {
                ref_bub_dialogue = FindObjectOfType<Bub_DialogueManager>().StartDialogue(list_dialogues[random_dialogue], player_two.transform.position + new Vector3(0, 0.5f, 0), true);
                player_ref = player_two;
            }
            else
            {
                ref_bub_dialogue = FindObjectOfType<Bub_DialogueManager>().StartDialogue(list_dialogues[random_dialogue], player_one.transform.position + new Vector3(0, 0.5f, 0), true);
                player_ref = player_one;
            }
        }
        taun_val = taunt_val_max;
    }

    public void Reset_vars()
    {
        player_ref = null;
        ref_bub_dialogue = null;
    }
}
