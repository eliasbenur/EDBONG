using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_MovePosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(gameObject.name + "is moving ///" + gameObject.transform.position);
        gameObject.GetComponent<Rigidbody2D>().MovePosition(new Vector2(0,0));
        Debug.Log(gameObject.name + "is moving ///" + gameObject.transform.position);
    }
}
