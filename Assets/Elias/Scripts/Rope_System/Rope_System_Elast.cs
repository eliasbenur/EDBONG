using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope_System_Elast : MonoBehaviour {

    #region Ini_Properties
    public List<Rope_Point> Points = new List<Rope_Point>();
    public List<GameObject> Objects_Coll = new List<GameObject>();
    public Rope_Point PrefabPoint;
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

            //RAYCAST TRIGER
            //Transform pref_tri = Instantiate(PrefabRay_Trig, newTransform.position, Quaternion.identity) as Transform;
           // pref_tri.gameObject.transform.parent = newTransform;

            Points.Add(particle);

            //TODO: Delete
            if (ParticleIndex == 0 || ParticleIndex == (NumPoints - 1)){

                /*var collider = particle.gameObject.AddComponent<CircleCollider2D>();
                collider.radius = 0.5f;*/
                particle.p_free = false;
            }
            else{
                particle.p_free = true;
            }

            if (ParticleIndex%2 == 0)
            {
                //particle.GetComponent<SpriteRenderer>().enabled = false;
                particle.GetComponent<SpriteRenderer>().sprite = chainA;
            }
            else
            {
                particle.GetComponent<SpriteRenderer>().sprite = chainB;
            }

            //ELST
            if (ParticleIndex == 0)
            {
                particle.GetComponent<SpringJoint2D>().connectedBody = player_One.GetComponent<Rigidbody2D>();
            }
            else
            {
                particle.GetComponent<SpringJoint2D>().connectedBody = Points[ParticleIndex - 1].GetComponent<Rigidbody2D>();
            }
        }

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            //ELST
            if (ParticleIndex == (NumPoints - 1))
            {
                SpringJoint2D spring =  Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.5f;
                spring.frequency = 5;
                spring.connectedBody = player_Two.GetComponent<Rigidbody2D>();
                //Points[ParticleIndex].GetComponent<SpringJoint2D>().connectedBody = player_Two.GetComponent<Rigidbody2D>();
            }
            else
            {
                //Points[ParticleIndex].GetComponent<SpringJoint2D>().connectedBody = Points[ParticleIndex + 1].GetComponent<Rigidbody2D>();
                SpringJoint2D spring = Points[ParticleIndex].gameObject.AddComponent<SpringJoint2D>();
                spring.autoConfigureDistance = false;
                spring.distance = 0.5f;
                spring.frequency = 5;
                spring.connectedBody = Points[ParticleIndex + 1].GetComponent<Rigidbody2D>();
            }
        }

        //ELST
        //SpringJoint2D spring_j = player_Two.transform.GetChild(0).gameObject.AddComponent<SpringJoint2D>();
        SpringJoint2D spring_j =  player_Two.AddComponent<SpringJoint2D>();
        spring_j.autoConfigureDistance = false;
        spring_j.distance = 0.5f;
        spring_j.frequency = 5;
        spring_j.connectedBody = Points[NumPoints - 1].GetComponent<Rigidbody2D>();

        SpringJoint2D spring_j2 = player_One.AddComponent<SpringJoint2D>();
        spring_j2.autoConfigureDistance = false;
        spring_j2.distance = 0.5f;
        spring_j2.frequency = 5;
        spring_j2.connectedBody = Points[0].GetComponent<Rigidbody2D>();

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            BoxCollider2D bc2d = Points[ParticleIndex].gameObject.AddComponent<BoxCollider2D>();
            bc2d.offset = new Vector2(0.46f, 0f);
            bc2d.size = new Vector2(0.75f, 0.1f);
        }
    }
    #endregion

    // Update is called once per frame
    void FixedUpdate () {
        TimeRemainder += Time.fixedDeltaTime;
        while (TimeRemainder > SubstepTime)
        {
            PreformSubstep(mov_P1, mov_P2);
            TimeRemainder -= SubstepTime;
        }
    }

    public void PreformSubstep(Vector3 P1_mov, Vector3 P2_mov)
    {
        //DistMaxConstraints(P1_mov, P2_mov);
        Update_LineRender();
    }

    private void DistMaxConstraints(Vector3 P1_mov, Vector3 P2_mov)
    {
        Points[0].new_pos = P1_mov;
        Points[NumPoints - 1].new_pos = P2_mov;

        float dist_avg = Dist_Avarage();

        New_Positions(dist_avg);

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + (Vector2)ParticleA.new_pos);

            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
            ParticleA.new_pos = Vector3.zero;
        }
    }

    private void New_Positions(float dist_avg){

        for (int PointIndex = 0; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];
            
            if (ParticleB.p_free)
            {
                Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
                float curr_dist = (float)Math.Round(Delta.magnitude, 2);
                if (curr_dist > dist_avg)
                {
                    ParticleB.new_pos = (curr_dist - dist_avg) * Delta.normalized;
                }
                else
                {
                    ParticleB.new_pos = (dist_avg - curr_dist) * Delta.normalized;
                }
            }
            else
            {
                Vector3 Delta = (ParticleB.transform.position + ParticleB.new_pos) - (ParticleA.transform.position + ParticleA.new_pos);
                float curr_dist = (float)Math.Round(Delta.magnitude, 2);
                if (curr_dist > dist_avg)
                {
                    ParticleA.new_pos = (curr_dist - dist_avg) * Delta.normalized;
                }
                else
                {
                    ParticleA.new_pos = (dist_avg - curr_dist) * Delta.normalized;
                }
            }

        }
        /*
        for (int PointIndex = NumPoints - 1; PointIndex > 1; PointIndex--)
        {

            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];
            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos_p2 + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
            if (Delta.magnitude > dist_avg)
            {
                ParticleB.new_pos = (Delta.magnitude - dist_avg) * Delta.normalized;
            }
            else
            {
                ParticleB.new_pos = (Delta.magnitude - dist_avg) * Delta.normalized * -1;
            }
        }*/

    }

    private float Dist_Avarage()
    {
        float dist_tot = 0f;
        for (int PointIndex = 0; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];
            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
            dist_tot += Delta.magnitude;
        }
        return (float) Math.Round(dist_tot / (NumPoints - 1), 2);
    }

    private void DistMaxP1()
    {
        bool stop = false;

        for (int PointIndex = 0; PointIndex < NumPoints - 2 && !stop; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];
            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos_p1 + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint)
            {
                if (!ParticleB.enemie_coll)
                {
                    Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
                    ParticleB.new_pos_p1 = P_B;
                }
                else
                {
                    Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * (Delta.normalized * 0.2f);
                    ParticleB.new_pos_p1 = P_B;
                }
            }
            else
            {
                stop = true;
            }
        }
    }

    private void DistMaxP2()
    {
        bool stop = false;

        for (int PointIndex = NumPoints-1; PointIndex > 1 && !stop; PointIndex--)
        {

            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];

            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos_p2 + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);

            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint)
            {
                if (!ParticleB.enemie_coll)
                {
                    Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
                    ParticleB.new_pos_p2 = P_B;
                }
                else
                {
                    Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * (Delta.normalized * 0.2f);
                    ParticleB.new_pos_p2 = P_B;
                }

            }
            else
            {
                stop = true;
            }
        }
    }

    private void CheckCollision()
    {
        for (int y = 0; y < Objects_Coll.Count; y++)
        {
            CircleCollider2D collid = Objects_Coll[y].GetComponent<CircleCollider2D>();
            Collider2D[] coll2D = new Collider2D[NumPoints];
            ContactFilter2D contf = new ContactFilter2D();
            contf.NoFilter();
            int num_coll = collid.OverlapCollider(contf, coll2D);

            for (int x = 0; x < num_coll; x++)
            {
                if (coll2D[x].transform.tag == "rope" || coll2D[x].transform.tag == "Objects")
                {
                    ColliderDistance2D coll_distance = coll2D[x].Distance(collid);
                    coll2D[x].transform.position += (Vector3)coll_distance.normal * coll_distance.distance;
                    //Coll_RollB(int.Parse(coll2D[x].name.Substring(coll2D[x].name.Length - 1)));
                }
            }
        }
    }

    private void DistMaxCalcul(int curr_it)
    {

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++) {
            Rope_Point ParticleA = Points[PointIndex];

            ParticleA.new_pos += ParticleA.new_pos_p1 + ParticleA.new_pos_p2;

            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
        }
    }

    private void Coll_RollB(int Index_coll)
    {
        int new_indew = Index_coll;
        List<Rope_Point> lisr_p = new List<Rope_Point>();
        for (int PointIndex = 0; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            if (ParticleA.coll_state || ParticleA.enemie_coll)
            {
                lisr_p.Add(ParticleA);
            }
        }
        if (lisr_p.Count != 0)
        {
            Rope_Point p = lisr_p[lisr_p.Count / 2];
            new_indew = Points.IndexOf(p);
        }

        // --> To the Right
        for (int PointIndex = new_indew; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];

            Vector3 Delta = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint && !state_surround)
            {
                if (!ParticleB.enemie_coll)
                {
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                    //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
                }
                else
                {
                    ParticleB.transform.position += (Delta.normalized*0.2f) * ((CurrentDistance - MaxLenght_xPoint));
                }
            }
            else if (state_surround)
            {
                ////////// NOT USED //////////////
                if (!ParticleB.coll_state)
                {
                    //TODO: add new pos to the RiGbody2D 
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                    //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
                }
            }
        }

        // <-- To the left
        for (int PointIndex = new_indew; PointIndex > 0; PointIndex--)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];

            Vector3 Delta = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;


            if (CurrentDistance > MaxLenght_xPoint && !state_surround)
            {
                if (!ParticleB.enemie_coll)
                {
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                    //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
                }
                else
                {
                    ParticleB.transform.position += (Delta.normalized * 0.2f) * ((CurrentDistance - MaxLenght_xPoint));
                }
            } else if (state_surround)
            {
                if (!ParticleB.coll_state)
                {
                    //TODO: add new pos to the RiGbody2D 
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                    //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
                }
            }
        }

    }


    private void Extrems_Correction()
    {
        Vector3 Delta = (Points[1].transform.position + Points[1].new_pos) - (Points[0].transform.position + Points[0].new_pos);
        float CurrentDistance = Delta.magnitude;

        if (CurrentDistance > MaxLenght_xPoint)
        {
            Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
            //TODO: Find Rigidbody2D using property or fonction
            Points[0].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[0].transform.position +  (Vector2)P_B);
            //CableStart.position = Points[0].transform.position + P_B;
        }
        else
        {
            //CableStart.position = Points[0].transform.position + Points[0].new_pos;
        }

        Delta = (Points[NumPoints - 2].transform.position + Points[NumPoints - 2].new_pos)- (Points[NumPoints - 1].transform.position + Points[NumPoints - 1].new_pos);
        CurrentDistance = Delta.magnitude;

        if (CurrentDistance > MaxLenght_xPoint)
        {
            Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
            //TODO: Find Rigidbody2D using property or fonction
            Points[NumPoints - 1].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[NumPoints - 1].transform.position + (Vector2)P_B);
            //CableEnd.position = Points[NumPoints - 1].transform.position + P_B;
        }
        else
        {
            //CableEnd.position = Points[NumPoints - 1].transform.position + Points[NumPoints - 1].new_pos;
        }

        
        
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
            //if (SegmentIndex%2 != 0)
            //{
                Vector3 difference = Points[SegmentIndex+1].transform.position - Points[SegmentIndex].transform.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                Points[SegmentIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                //Points[SegmentIndex].transform.LookAt(Points[SegmentIndex + 1].transform);
            //}
        }

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            BoxCollider2D bc2d = Points[ParticleIndex].gameObject.GetComponent<BoxCollider2D>();

            if (ParticleIndex == NumPoints -1)
            {
                Rope_Point ParticleA = Points[ParticleIndex];
                Vector3 Delta = ParticleA.transform.position- player_Two.transform.position;
                float CurrentDistance = Delta.magnitude;

                bc2d.size = new Vector2(CurrentDistance/ParticleA.transform.localScale.x, 0.1f);
                bc2d.offset = new Vector2((CurrentDistance / ParticleA.transform.localScale.x) / 2, 0f);
            }
            else
            {
                Rope_Point ParticleA = Points[ParticleIndex];
                Rope_Point ParticleB = Points[ParticleIndex + 1];
                Vector3 Delta = ParticleA.transform.position - ParticleB.transform.position;
                float CurrentDistance = Delta.magnitude;

                bc2d.size = new Vector2(CurrentDistance / ParticleA.transform.localScale.x, 0.1f);
                bc2d.offset = new Vector2((CurrentDistance / ParticleA.transform.localScale.x) / 2, 0f);
            }
        }


    }


}
