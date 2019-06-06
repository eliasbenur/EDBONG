using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Shield : MonoBehaviour
{
    public void Destroy()
    {
        StartCoroutine("Destroy_ui");
    }

    public IEnumerator Destroy_ui()
    {
        transform.parent = transform.parent.parent;
        GetComponent<Animator>().SetBool("destroy", true);
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
}
