using UnityEngine;

public class moneyCollect : MonoBehaviour
{
    private GameManager moneyIncrement;

    private void Awake()
    {
        moneyIncrement = Camera.main.GetComponent<GameManager>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            moneyIncrement.money++;
            Destroy(this.gameObject);
        }
    }
}
