using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLive : MonoBehaviour
{

    private float livepoints;
    GameManager gm;
    public List<GameObject> chains_ui;

    // Start is called before the first frame update
    void Start()
    {
        gm = Camera.main.GetComponent<GameManager>();
    }
    void Update()
    {
        //If the points life have changed, update the UI
        if (livepoints != gm.life)
        {
            livepoints = gm.life;
            UpdateChainLive();
        }
    }

    /* Update the Live Bar*/
    void UpdateChainLive()
    {
        int x = 1;
        foreach(GameObject go in chains_ui)
        {
            if (x <= livepoints)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
            x++;
        }
    }
}
