using UnityEngine;

public class FaddingEffect : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float speed;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - speed * Time.deltaTime);
        if (sprite.color.a < 0)
        {
            Destroy(gameObject);
        }
    }
}
