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
        /*for (int x = 0; x < transform.childCount; x++)
        {
            chains_ui.Add(transform.GetChild(x).gameObject);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (livepoints != gm.life)
        {
            livepoints = gm.life;
            UpdateChainLive();
        }
    }

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
