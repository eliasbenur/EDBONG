using UnityEngine;

public class moneyCollect : MonoBehaviour
{
    private GameManager moneyIncrement;
    public GameObject Ui_text_mone;

    private void Awake()
    {
        moneyIncrement = Camera.main.GetComponent<GameManager>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            moneyIncrement.money++;
            moneyIncrement.Update_UI_money();
            Instantiate(Ui_text_mone, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
