using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Money_Tmp : MonoBehaviour
{
    public float speed;
    private Text text_ui;
    // Start is called before the first frame update
    void Start()
    {
        text_ui = GetComponent<Text>();
        transform.parent = GameObject.Find("Money").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
        text_ui.color = new Color(text_ui.color.r, text_ui.color.g, text_ui.color.b, text_ui.color.a - speed * Time.deltaTime);
        if (text_ui.color.a < 0){
            Destroy(gameObject);
        }
    }
}
