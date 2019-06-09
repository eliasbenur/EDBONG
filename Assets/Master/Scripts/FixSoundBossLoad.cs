using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSoundBossLoad : MonoBehaviour
{
    #region Properties
    private int NumPlayer_inside = 0;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            NumPlayer_inside++;

            if (NumPlayer_inside == 2)
            {
                AkSoundEngine.StopAll();
                for (int x = 0; x < gameObject.transform.childCount; x++)
                {
                    if (gameObject.transform.GetChild(x).tag == "door")
                        gameObject.transform.GetChild(x).GetComponent<Door_Trigger>().Set_auto_run_1time(true);
                }
            }
        }
    }
}
