using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public int money;

    [SerializeField]
    public List<GameObject> listItemDisplay;
    public bool KeyPressed;

    public UParticleSystem_E_Modif RopePowerUp_Malus;
    public playerMovement_E_Modif checkPlayer;

    public void Awake()
    {
        listItemDisplay.AddRange(GameObject.FindGameObjectsWithTag("Item"));
    }

    private void Update()
    {
        for(int i =0; i < listItemDisplay.Count; i++)
        {
            //  0/2 in case we have items with a price of two numbers
            var priceItem = listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text.Substring(0,2);
            if (System.Convert.ToInt32(priceItem) > money)
            {
                listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                listItemDisplay[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        if (KeyPressed && Input.GetKey(KeyCode.Space) && money >= System.Convert.ToInt32(checkPlayer.collisionItems.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 2)))
        {
            money -= System.Convert.ToInt32(checkPlayer.collisionItems.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, 2));
            for (int i = 0; i < listItemDisplay.Count; i++)
            {
                if (listItemDisplay[i] == checkPlayer.collisionItems.gameObject)
                {
                    Debug.Log("Object Found");
                    Destroy(checkPlayer.collisionItems.gameObject);
                    listItemDisplay.RemoveAt(i);
                }
            }

            switch (checkPlayer.collisionItems.gameObject.name)
            {
                case "PlusLengthRope":
                    Debug.Log("PlusRope");
                    RopePowerUp_Malus.CableMaxLenght = 60;
                    RopePowerUp_Malus.CableLengthDesired = 50;
                    break;
                case "GravityObject":
                    Debug.Log("Gravity");
                    //RopePowerUp_Malus.gravity = Vector3.zero;
                    break;
                case "MinusLengthRope":
                    Debug.Log("MinusRope");
                    RopePowerUp_Malus.CableMaxLenght = 20;
                    RopePowerUp_Malus.CableLengthDesired = 10;
                    break;
                default:
                    break;
            }
        }
    }
}
