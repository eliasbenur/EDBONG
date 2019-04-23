using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Floor_System : MonoBehaviour
{

    public string Scene_toLoad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name, new Dictionary<string, object>
            {
                { "HP", Camera.main.GetComponent<GameManager>().life },
                { "time_passed", Time.timeSinceLevelLoad },
                { "num_hits" , Camera.main.GetComponent<GameManager>().num_hits }
            });
            SceneManager.LoadScene(Scene_toLoad);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("LD1");
    }
}
