using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieBoss_Phase : MonoBehaviour
{
    public float timer, timerTotBeforeDead, timerReturn;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public GameObject camera;
    public GameObject canvas;

    bool checkedMonsters;

    public GameObject dieSmoke1, dieSmoke2;
    bool canSpawn;
    public float cooldown;

    public CinematicBars cinematicDie;

    private void Awake()
    {
        camera = Camera.main.gameObject;
    }

    private void FixedUpdate()
    {
        cinematicDie.Show(200, 0.8f);

        timer += Time.deltaTime;
        BehaviorCamera();
        if (timer > timerTotBeforeDead && timer < timerReturn)
        {
            GetComponent<Animator>().Play("Die");
            StartCoroutine(Smoke_Dead());
        }

        if (timer > timerReturn)
        {
            camera.GetComponent<Camera_Focus>().enabled = true;
            canvas.SetActive(true);
            var desactivate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < desactivate.Length; i++)
            {
                desactivate[i].GetComponent<Animator>().enabled = true;
                desactivate[i].GetComponent<Player_Movement>().enabled = true;
            }
            cinematicDie.Hide(0.001f);
            Destroy(this.gameObject);
        }
    }


    IEnumerator Smoke_Dead()
    {
        var position = Random.insideUnitSphere * 5 + transform.position;
        var smokeToSpawn = Random.Range(1, 3);
        var size = Random.Range(1, 6);
        switch (smokeToSpawn)
        {
            case 1:
                var smoke = Instantiate(dieSmoke1, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size,size);
                break;

            case 2:
                smoke = Instantiate(dieSmoke2, position, Quaternion.identity);
                smoke.transform.localScale = new Vector2(size, size);
                break;
        }
        canSpawn = false;
        yield return new WaitForSeconds(cooldown);
        if(cooldown > 0.1)
            cooldown *= 0.9f;
        yield return null;
    }

    void BehaviorCamera()
    {
        canvas.SetActive(false);
        camera.GetComponent<Camera_Focus>().enabled = false;
        var desactivate = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < desactivate.Length; i++)
        {
            desactivate[i].GetComponent<Animator>().enabled = false;
            desactivate[i].GetComponent<Player_Movement>().enabled = false;
        }
        camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, camera.transform.position.z), ref velocity, smoothTime);
    }
}
