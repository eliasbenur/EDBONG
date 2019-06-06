using UnityEngine;

public class ResetMap : MonoBehaviour
{
    private Animator miniMap;

    private void Awake()
    {
        miniMap = GetComponent<Animator>();
    }
    public void Reset()
    {
        miniMap.SetBool("Close", false);
        miniMap.SetBool("Open", false);
    }
}
