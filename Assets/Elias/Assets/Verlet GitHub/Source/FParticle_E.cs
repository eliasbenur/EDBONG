using UnityEngine;
using System.Collections;

// Using Verlet Integration to solve constrains and velocity
// This iteration is working much better then my first try
// at making a rope with physics. I've tried to do the same
// but with a different approach and it didn't work out.
// This code is all from UE4 CableComponent.
// Converted it in C# to test within Unity, due slow pc at home.
public class FParticle_E : MonoBehaviour {
	#region public Properties
	public Vector3 OldPosition;
    public float new_force;
	public bool bFree;
	#endregion

	#region private Properties
	private Transform _transform;
	#endregion

	#region Unity Methods
	void Awake() {
		_transform = transform;
	}

    void Start()
    {
        OldPosition = _transform.position;
        position = _transform.position;
        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.1f;
        /*var collider = gameObject.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.5f, 0.1f);
        collider.direction = CapsuleDirection2D.Horizontal;*/
        //collider.isTrigger = true;
        var Rb = gameObject.AddComponent<Rigidbody2D>();
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Rb.gravityScale = 0;

    }
    #endregion

    #region public Getters & Setters
    public Vector3 position {
		get { return _transform.position; }
		set { _transform.position = value; }
	}
    #endregion

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(gameObject.name + " collided with " + col.gameObject.name);
        if (col.gameObject.layer == 9)
        {
            Physics2D.IgnoreCollision(col.transform.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
    }

    /*void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.gameObject.layer == 9)
        {
            Physics2D.IgnoreCollision(col.transform.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        else
        {
            float distance = Mathf.Abs(gameObject.GetComponent<CircleCollider2D>().Distance(col.GetComponent<CircleCollider2D>()).distance);

            Vector3 Delta = col.transform.position - transform.position;
            float CurrentDistance = Delta.magnitude;
            Vector3 P_B = distance * Vector3.Normalize(Delta);
            transform.position -= P_B / 2;
            col.transform.position += P_B / 2;

            //Debug.Log(gameObject.name + " triggered " + col.gameObject.name);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {

        if (col.gameObject.layer == 9)
        {
            Physics2D.IgnoreCollision(col.transform.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        else
        {
            float distance = Mathf.Abs(gameObject.GetComponent<CircleCollider2D>().Distance(col.GetComponent<CircleCollider2D>()).distance);

            Vector3 Delta = col.transform.position - transform.position;
            float CurrentDistance = Delta.magnitude;
            Vector3 P_B = distance * Vector3.Normalize(Delta);
            transform.position -= P_B / 2;
            col.transform.position += P_B / 2;

            //Debug.Log(gameObject.name + " triggered " + col.gameObject.name);
        }
    }*/
}