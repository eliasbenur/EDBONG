using System.Collections.Generic;
using UnityEngine;

public class Rope_System_Elast : MonoBehaviour {

    #region Ini_Properties
    public List<Rope_Point> Points = new List<Rope_Point>();
    public Vector3[] Arr_Points;
    public List<Transform> List_CollPoints;
    public List<GameObject> Objects_Coll = new List<GameObject>();
    public Rope_Point PrefabPoint;
    public Transform prefab_coll_point;
    public Transform PrefabRay_Trig;

    public float MaxLenght_xPoint = 0.2f;
    public int NumPoints = 50;

    public int Iterations;
    public LineRenderer _lineRenderer;

    private float TimeRemainder = 0f;
    private float SubstepTime = 0.02f;

    public Transform CableStart;
    public Transform CableEnd;

    public Vector2 mov_P1;
    public Vector2 mov_P2;

    public AnimationCurve animatedcurve;

    public bool state_surround;

    public GameObject player_One;
    public GameObject player_Two;

    public Sprite chainA;
    public Sprite chainB;




    [System.Serializable] public struct coll_pos
    {
        public int point_nul;
        public float dist;
    }

    // Use this for initialization
    void Start()
    {

        state_surround = false;

        Points.Clear();

        mov_P1 = Vector3.zero;
        mov_P2 = Vector3.zero;

        _lineRenderer = transform.GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        _lineRenderer.positionCount = NumPoints + 2;
        _lineRenderer.startWidth = 0.15f;
        _lineRenderer.endWidth = 0.15f;

        // "Distance" between the 2 start points
        Vector3 Delta = CableEnd.position - CableStart.position;

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++){
            Rope_Point particle = Instantiate(PrefabPoint, Vector3.zero, Quaternion.identity);

            float Alpha = (float)ParticleIndex / (float)(NumPoints-1);
            Vector3 InitializePosition = CableStart.transform.position + (Alpha * Delta);

            //Ini variables of the Particle
            particle.transform.position = InitializePosition;
            particle.transform.parent = this.transform;
            particle.transform.tag = "rope";
            particle.name = "Point_" + ParticleIndex.ToString();

            particle.gameObject.layer = 9;

            //TODO: Move Ignore Layer collision outside of loop
            Physics2D.IgnoreLayerCollision(8, 9);//////////////////
            Physics2D.IgnoreLayerCollision(9, 9);

            Points.Add(particle);

            if (ParticleIndex%2 == 0)
            {
                particle.GetComponent<SpriteRenderer>().sprite = chainA;
                particle.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
            else
            {
                particle.GetComponent<SpriteRenderer>().sprite = chainB;
            }


        }
        //ELST
        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            if (ParticleIndex == 0)
            {

                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.3f;
                spring.frequency = 8;
                spring.connectedBody = player_One.GetComponent<Rigidbody2D>();
            }
            else if (ParticleIndex < NumPoints / 2)
            {

                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.3f;
                spring.frequency = 8;
                spring.connectedBody = Points[ParticleIndex - 1].GetComponent<Rigidbody2D>();
            }
            else if (ParticleIndex == NumPoints / 2)
            {

                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.3f;
                spring.frequency = 8;
                spring.connectedBody = Points[ParticleIndex - 1].GetComponent<Rigidbody2D>();

                SpringJoint2D spring2 = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring2.autoConfigureDistance = false;
                spring2.distance = 0.3f;
                spring2.frequency = 8;
                spring2.connectedBody = Points[ParticleIndex + 1].GetComponent<Rigidbody2D>();
            }
            else if (ParticleIndex == NumPoints - 1)
            {

                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.3f;
                spring.frequency = 8;
                spring.connectedBody = player_Two.GetComponent<Rigidbody2D>();

            }
            else if (ParticleIndex > NumPoints / 2)
            {

                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.3f;
                spring.frequency = 8;
                spring.connectedBody = Points[ParticleIndex + 1].GetComponent<Rigidbody2D>();
            }
        }

    }
    #endregion

    // Update is called once per frame
    void FixedUpdate () {
        Update_LineRender();
    }


    private void Update_LineRender()
    {
        _lineRenderer.SetPosition(0, player_One.transform.position);
        _lineRenderer.SetPosition(NumPoints + 1, player_Two.transform.position);
        // Update render position
        for (int SegmentIndex = 1; SegmentIndex < NumPoints + 1; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Points[SegmentIndex - 1].transform.position);
        }

        for (int SegmentIndex = 0; SegmentIndex < NumPoints - 1; SegmentIndex++)
        {
            Vector3 difference = Points[SegmentIndex+1].transform.position - Points[SegmentIndex].transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            Points[SegmentIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            //Points[SegmentIndex].transform.LookAt(Points[SegmentIndex + 1].transform);
        }
        Vector3 difference2 = player_Two.transform.position - Points[NumPoints - 1].transform.position;
        float rotationZ2 = Mathf.Atan2(difference2.y, difference2.x) * Mathf.Rad2Deg;
        Points[NumPoints - 1].transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ2);

    }
}

public static class MyVector3Extension
{
    public static Vector2[] toVector2Array(this Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, getV3fromV2);
    }

    public static Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}
