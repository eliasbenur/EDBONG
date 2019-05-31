using UnityEngine;

public class CameraBehaviorEnterBossRoom : MonoBehaviour
{
    #region Properties
    bool detected;
    public float smoothTime = 2f;
    private Vector3 velocity;
    public GameObject Boss;
    public float timer, timerTot;

    public float offsetCamera;
    public GameObject canvas;
    public CinematicBars cinematic;
    public GameObject miniMap;

    private Camera_Focus camera;
    private StartCombatBossGestion combatBoss;
    #endregion
    private void Awake()
    {
        camera = Camera.main.GetComponent<Camera_Focus>();
        combatBoss = GetComponent<StartCombatBossGestion>();
        offsetCamera = 14;
    }

    private void FixedUpdate()
    {
        if(detected)
        {
            miniMap.SetActive(false);
            cinematic.Show(200, 0.8f);
            canvas.SetActive(false);
            camera.enabled = false;
            var desactivate = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < desactivate.Length; i++)
            {
                desactivate[i].GetComponent<Player_Movement>().Stop_Moving();
            }
            camera.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,new Vector3(Boss.transform.position.x, Boss.transform.position.y - offsetCamera,camera.transform.position.z), ref velocity, smoothTime);
            Boss.transform.position =  Vector2.Lerp(Boss.transform.position, new Vector2(Boss.transform.position.x, Boss.transform.position.y - 0.082f),0.5f);
            offsetCamera = Mathf.Lerp(offsetCamera, 2.5f, 0.007f);
            timer += Time.deltaTime;
            if(timer > timerTot)
            {
                for (int i = 0; i < desactivate.Length; i++)
                {
                    if(Boss.GetComponent<Collider2D>().GetType() == typeof(CapsuleCollider2D)) Boss.GetComponent<Collider2D>().enabled = true;
                }
                combatBoss.enabled = true;
                Boss.GetComponent<Animator>().enabled = true;
                Destroy(this);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player" && !detected)
        {
            detected = true;
            GetComponent<Collider2D>().enabled = false;        
        }
    }
}
