using UnityEngine;

public class FaddingEffect : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float speed;
    private ParticleSystem particle;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        particle = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - speed * Time.deltaTime);
        if(particle.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
