using UnityEngine;
using UnityEngine.UI;

public class StuffDisplay : MonoBehaviour
{
    public Stuff stuff;
    private GameManager moneyManager;
    private Text costText;

    private void Awake()
    {
        moneyManager = Camera.main.GetComponent<GameManager>();
        costText = GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = stuff.name;
        GetComponent<SpriteRenderer>().sprite = stuff.sprite;
        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        costText.text = stuff.cost.ToString() + " ß";
    }

    private void Update()
    {
        if (moneyManager.money >= stuff.cost)
            costText.color = Color.white;
        else
            costText.color = Color.red;
    }
}
