using UnityEngine;

public class StuffDisplay : MonoBehaviour
{
    public Stuff stuff;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = stuff.name;
        GetComponent<SpriteRenderer>().sprite = stuff.sprite;
        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }
}
