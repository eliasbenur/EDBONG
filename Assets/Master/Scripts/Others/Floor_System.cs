using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Floor_System : MonoBehaviour
{
    public string Scene_toLoad;
    public Image endScreen;

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

    public void EndScreen()
    {
        StartCoroutine("Example");
    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);
        endScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("Menu_Principal");
    }
}
