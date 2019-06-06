using System.Collections;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject playerPrefab;
    private bool canShoot = true;
    public Material flashMat;

    private Player_Movement playerMov;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerMov = GetComponent<Player_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canShoot)
            StartCoroutine(SpriteInstanciation(0.5f));
    }

    IEnumerator SpriteInstanciation(float cooldown)
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        player.GetComponent<SpriteRenderer>().sprite = sprite.sprite;

        if (playerMov.get_MovementX() > 0)
        {
            player.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (playerMov.get_MovementX() < 0)
        {
            player.GetComponent<SpriteRenderer>().flipX = true;
        }
        player.GetComponent<SpriteRenderer>().material = flashMat;
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}
