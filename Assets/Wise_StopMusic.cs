using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wise_StopMusic : MonoBehaviour
{
    private float delay = 1f;

    // Update is called once per frame
    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else if(delay <= 0 && delay > -1)
        {
            delay = -999;
            Debug.Log("Hey");
            //AkSoundEngine.StopPlayingID(AkSoundEngine.GetIDFromString("play_music"));
            //AkSoundEngine.StopAll();
        }
        else
        {

        }
    }
}
