using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verlet_Rope_System : MonoBehaviour {

    #region Ini_Properties
    private List<Verlet_Rope_Point> Points = new List<Verlet_Rope_Point>();
    private List<Verlet_Sticks> Sticks = new List<Verlet_Sticks>();
    public Transform PrefabPoint;

    public float MaxLenght_xPoint = 0.2f;
    public int NumPoints = 50;

    public int Iterations;

    public float bounce;
    public float friction;

    public LineRenderer _lineRenderer;

    private float TimeRemainder = 0f;
    private float SubstepTime = 0.02f;

    public Transform CableStart;
    public Transform CableEnd;

    public Vector2 mov_P1;
    public Vector2 mov_P2;

    public bool verlet;

    // Use this for initialization
    void Start()
    {
        Points.Clear();

        mov_P1 = Vector3.zero;
        mov_P2 = Vector3.zero;

        _lineRenderer = transform.GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        _lineRenderer.positionCount = NumPoints;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;

        // "Distance" between the 2 start points
        Vector3 Delta = CableEnd.position - CableStart.position;

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            Transform newTransform = Instantiate(PrefabPoint, Vector3.zero, Quaternion.identity) as Transform;

            float Alpha = (float)ParticleIndex / (float)(NumPoints - 1);
            Vector3 InitializePosition = CableStart.transform.position + (Alpha * Delta);
            InitializePosition.z = 0;

            //Ini variables of the Particle
            Verlet_Rope_Point particle = newTransform.GetComponent<Verlet_Rope_Point>();
            particle.transform.position = InitializePosition;
            particle.OldPosition = particle.transform.position - new Vector3(0.1f, 0.1f, 0);
            particle.transform.parent = this.transform;
            particle.name = "Point_" + ParticleIndex.ToString();

            particle.gameObject.layer = 9;

            Points.Add(particle);

            if (ParticleIndex == 0 || ParticleIndex == (NumPoints - 1))
            {
                particle.p_free = false;
            }
            else
            {
                particle.p_free = true;
            }
        }

        for (int ParticleIndex = 0; ParticleIndex < NumPoints - 1; ParticleIndex++)
        {
            Verlet_Rope_Point Point_A = Points[ParticleIndex];
            Verlet_Rope_Point Point_B = Points[ParticleIndex + 1];

            Verlet_Sticks Stick = new Verlet_Sticks();
            Stick.PointA = Point_A;
            Stick.PointB = Point_B;

            Sticks.Add(Stick);
        }
    }
    # endregion

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeRemainder += Time.fixedDeltaTime;
        while (TimeRemainder > SubstepTime)
        {
            PreformSubstep();
            TimeRemainder -= SubstepTime;
        }
    }

    private void PreformSubstep()
    {
        Players_Movement();

        if (verlet)
        {
            VerletIntegrate(SubstepTime);
        }

        //Update all stick (- cable start and end)
        for (int curr_int = 0; curr_int < Iterations; curr_int++)
        {
            UpdateSticks();

            //Collision_Constrainst();
        }

        // Update all stick (+ cable start and end)
        UpdateSticks_Full();
        

        //Collision
        /*Points[0].p_free = true;
        Points[NumPoints - 1].p_free = true;
        for (int curr_int = 0; curr_int < Iterations; curr_int++)
        {
            //Collision_Constrainst();

            UpdateSticks();
        }
        Points[0].p_free = false;
        Points[NumPoints - 1].p_free = false;*/

        //Walls
        //ConstrainsPoints();

        CableStart.transform.position = Points[0].transform.position;
        CableEnd.transform.position = Points[NumPoints - 1].transform.position;
        Update_LineRender();
    }

    private void Collision_Constrainst()
    {
        /*for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Verlet_Rope_Point ParticleA = Points[PointIndex];

            CircleCollider2D collid = GameObject.Find("Target2").GetComponent<CircleCollider2D>();
            Collider2D[] coll2D = new Collider2D[10];
            ContactFilter2D contf = new ContactFilter2D();
            contf.NoFilter();

            if (collid.OverlapPoint((Vector2)ParticleA.transform.position))
            //if (collid.OverlapCollider(contf, coll2D) > 0)
            //if (collid.IsTouching(ParticleA.GetComponent<CircleCollider2D>()))
            {
                Debug.Log("Hey");
                ColliderDistance2D coll_distance = ParticleA.gameObject.GetComponent<CircleCollider2D>().Distance(collid);
                ParticleA.transform.position += (Vector3)coll_distance.normal * coll_distance.distance;
                /////////////////////////////////////////////////////////////////////
                //CHANGE OLD POSITION!!!
                /////////////////////////////////////////////////////////////////////
            }
        }*/

        CircleCollider2D collid = GameObject.Find("Target2").GetComponent<CircleCollider2D>();
        Collider2D[] coll2D = new Collider2D[NumPoints];
        ContactFilter2D contf = new ContactFilter2D();
        contf.NoFilter();
        int num_coll = collid.OverlapCollider(contf, coll2D);

        for (int x = 0; x < num_coll; x++)
        {
            ColliderDistance2D coll_distance = coll2D[x].Distance(collid);
            coll2D[x].transform.position += (Vector3)coll_distance.normal * coll_distance.distance;
        }
    }

    private void UpdateSticks_Full()
    {
        Points[0].p_free = true;
        Points[NumPoints - 1].p_free = true;
        for (int curr_int = 0; curr_int < Iterations; curr_int++)
        {
            UpdateSticks();
        }
        Points[0].p_free = false;
        Points[NumPoints - 1].p_free = false;
    }

    private bool Check_MaxRope()
    {
        bool max = true;

        foreach (Verlet_Sticks Stick in Sticks)
        {
            if (Stick.PointB.name != ("Point_" + (NumPoints -1).ToString()))
            {
                Vector3 Delta = Stick.PointA.transform.position - Stick.PointB.transform.position;
                float dist = Delta.magnitude;
                if (dist < (MaxLenght_xPoint + 0.001f))
                {
                    max = false;
                }
            }
        }

        /*float dist_tot = 0;
        foreach (Verlet_Sticks Stick in Sticks)
        {
            Vector3 Delta = Stick.PointA.transform.position - Stick.PointB.transform.position;
            float dist = Delta.magnitude;
            dist_tot += dist;
        }
        if (dist_tot < MaxLenght_xPoint * NumPoints)
        {
            max = false;
        }*/


        if (max)
        {
            //Debug.Log("Rope MAXED");
        }

        return max;
    }

    private void Players_Movement()
    {
        Points[0].transform.position += (Vector3)mov_P1;
        Points[NumPoints - 1].transform.position += (Vector3)mov_P2;
        CableStart.transform.position = Points[0].transform.position;
        CableEnd.transform.position = Points[NumPoints - 1].transform.position;

        if (Check_MaxRope())
        {
            Points[NumPoints - 1].transform.position += (Points[NumPoints - 2].transform.position - Points[NumPoints - 1].transform.position).normalized * mov_P1.magnitude;
            Points[0].transform.position += (Points[1].transform.position - Points[0].transform.position).normalized * mov_P2.magnitude;
        }

    }

    private void ConstrainsPoints()
    {

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Verlet_Rope_Point particle = Points[PointIndex];
            if (particle.p_free)
            {
                Vector3 Velocity = (particle.transform.position - particle.OldPosition) * friction;

                // Walls Collision
                if (particle.transform.position.y < -5)
                {
                    particle.transform.position = new Vector3(particle.transform.position.x, -5, 0);
                    particle.OldPosition = new Vector3(particle.OldPosition.x, particle.transform.position.y + particle.NewPosition.y * bounce, 0);
                }
                else if (particle.transform.position.y > 5)
                {
                    particle.transform.position = new Vector3(particle.transform.position.x, 5, 0);
                    particle.OldPosition = new Vector3(particle.OldPosition.x, particle.transform.position.y + particle.NewPosition.y * bounce, 0);
                }
                if (particle.transform.position.x < -10)
                {
                    particle.transform.position = new Vector3(-10, particle.transform.position.y, 0);
                    particle.OldPosition = new Vector3(particle.transform.position.x + particle.NewPosition.x * bounce, particle.OldPosition.y, 0);
                }
                else if (particle.transform.position.x > 10)
                {
                    particle.transform.position = new Vector3(10, particle.transform.position.y, 0);
                    particle.OldPosition = new Vector3(particle.transform.position.x + particle.NewPosition.x * bounce, particle.OldPosition.y, 0);
                }
            }
        }
    }

    private void UpdateSticks_Test()
    {
        foreach (Verlet_Sticks Stick in Sticks)
        {
            Vector3 Delta = Stick.PointA.transform.position - Stick.PointB.transform.position;
            float dist = Delta.magnitude;
            if (dist > MaxLenght_xPoint)
            {
                float dif = MaxLenght_xPoint - dist;
                float percet = dif / dist / 2;


                if (Stick.PointA.p_free && Stick.PointB.p_free)
                {

                    CircleCollider2D collid = Stick.PointA.GetComponent<CircleCollider2D>();
                    Collider2D[] coll2D = new Collider2D[NumPoints];
                    ContactFilter2D contf = new ContactFilter2D();
                    contf.NoFilter();
                    int num_coll = collid.OverlapCollider(contf, coll2D);
                    
                    bool col = false;
                    /*
                    for (int x = 0; x < num_coll; x++)
                    {
                        if (coll2D[x].tag == "Objects")
                        {
                            //col = true;
                        }
                    }*/

                    if (col)
                    {
                        //Raycast - Collision Detection
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointA.transform.position, (-Delta).normalized, Delta.magnitude * percet, layer_mask);

                        if (hit.collider != null)
                        {
                            Stick.PointA.transform.position += (-Delta).normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointA.transform.position += (Delta * percet);
                        }
                    }
                    else
                    {
                        Stick.PointA.transform.position += (Delta * percet);
                    }


                    collid = Stick.PointB.GetComponent<CircleCollider2D>();
                    coll2D = new Collider2D[NumPoints];
                    contf = new ContactFilter2D();
                    contf.NoFilter();
                    int num_coll2 = collid.OverlapCollider(contf, coll2D);
                    bool col2 = false;
                    /*
                    for (int x = 0; x < num_coll2; x++)
                    {
                        if (coll2D[x].tag == "Objects")
                        {
                            //col2 = true;
                        }
                    }*/

                    if (col2)
                    {
                        //Raycast - Collision Detection
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit2 = Physics2D.Raycast(Stick.PointB.transform.position, Delta.normalized, Delta.magnitude * percet, layer_mask);
                        if (hit2.collider != null)
                        {
                            Stick.PointB.transform.position += Delta.normalized * hit2.distance;
                        }
                        else
                        {
                            Stick.PointB.transform.position -= (Delta * percet);
                        }
                    }
                    else
                    {
                        Stick.PointB.transform.position -= (Delta * percet);
                    }

                    


                    /*Stick.PointA.transform.position += (Delta * percet);
                    Stick.PointB.transform.position -= (Delta * percet);*/
                }
                else if (Stick.PointA.p_free)
                {

                    
                    CircleCollider2D collid = Stick.PointA.GetComponent<CircleCollider2D>();
                    Collider2D[] coll2D = new Collider2D[NumPoints];
                    ContactFilter2D contf = new ContactFilter2D();
                    contf.NoFilter();
                    int num_coll = collid.OverlapCollider(contf, coll2D);
                    bool col = false;
                    /*
                    for (int x = 0; x < num_coll; x++)
                    {
                        if (coll2D[x].tag == "Objects")
                        {
                            //col = true;
                        }
                    }*/

                    if (col)
                    {
                        //Raycast - Collision Detection
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointA.transform.position, (-Delta).normalized, Delta.magnitude * percet * 2, layer_mask);
                        if (hit.collider != null)
                        {
                            Stick.PointA.transform.position += (-Delta).normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointA.transform.position += ((Delta * percet) * 2);
                        }
                    }
                    else
                    {
                        Stick.PointA.transform.position += ((Delta * percet) * 2);
                    }

                    //Stick.PointA.transform.position += ((Delta * percet) * 2);
                }
                else if (Stick.PointB.p_free)
                {
                    CircleCollider2D collid = Stick.PointB.GetComponent<CircleCollider2D>();
                    Collider2D[] coll2D = new Collider2D[NumPoints];
                    ContactFilter2D contf = new ContactFilter2D();
                    contf.NoFilter();
                    int num_coll = collid.OverlapCollider(contf, coll2D);
                    bool col = false;
                    /*
                    for (int x = 0; x < num_coll; x++)
                    {
                        if (coll2D[x].tag == "Objects")
                        {
                            //col = true;
                        }
                    }*/

                    if (col)
                    {
                        //Raycast - Collision Detection
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit2 = Physics2D.Raycast(Stick.PointB.transform.position, Delta.normalized, Delta.magnitude * percet * 2, layer_mask);
                        if (hit2.collider != null)
                        {
                            Stick.PointB.transform.position += Delta.normalized * hit2.distance;
                        }
                        else
                        {
                            Stick.PointB.transform.position -= (Delta * percet) * 2;
                        }
                    }
                    else
                    {
                        Stick.PointB.transform.position -= (Delta * percet) * 2;
                    }



                    //Stick.PointB.transform.position -= ((Delta * percet) * 2);
                }
            }
        }
    }

    private void UpdateSticks()
    {
        foreach (Verlet_Sticks Stick in Sticks)
        {
            Vector3 Delta = Stick.PointA.transform.position - Stick.PointB.transform.position;
            float dist = Delta.magnitude;
            if (dist > MaxLenght_xPoint)
            {
                float dif = MaxLenght_xPoint - dist;
                float percet = dif / dist / 2;


                if (Stick.PointA.p_free && Stick.PointB.p_free)
                {
                    //Debug.Log(dist);
                    /*if (Delta.magnitude * percet > 0.0001f)
                    {
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointA.transform.position, (-Delta).normalized, Delta.magnitude * percet, layer_mask);

                        if (hit.collider != null)
                        {
                            Stick.PointA.transform.position += (-Delta).normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointA.transform.position += (Delta * percet);
                        }
                    }
                    else
                    {
                        Stick.PointA.transform.position += (Delta * percet);
                    }

                    if (Delta.magnitude * percet > 0.1f)
                    {
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointB.transform.position, Delta.normalized, Delta.magnitude * percet, layer_mask);

                        if (hit.collider != null)
                        {
                            Stick.PointB.transform.position += Delta.normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointB.transform.position -= (Delta * percet);
                        }
                    }
                    else
                    {
                        Stick.PointB.transform.position -= (Delta * percet);
                    }*/

                    
                    Stick.PointA.transform.position += (Delta * percet);
                    Stick.PointB.transform.position -= (Delta * percet);
                }
                else if (Stick.PointA.p_free)
                {
                    /*if (Delta.magnitude * percet * 2> 0.1f)
                    {
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointA.transform.position, (-Delta).normalized, Delta.magnitude * percet * 2, layer_mask);

                        if (hit.collider != null)
                        {
                            Stick.PointA.transform.position += (-Delta).normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointA.transform.position += ((Delta * percet) * 2);
                        }
                    }
                    else
                    {
                        Stick.PointA.transform.position += ((Delta * percet) * 2);
                    }*/

                    Stick.PointA.transform.position += ((Delta * percet) * 2);
                }
                else if (Stick.PointB.p_free)
                {

                    /*if (Delta.magnitude * percet * 2 > 0.1f)
                    {
                        int layer_mask = LayerMask.GetMask("Objects");
                        RaycastHit2D hit = Physics2D.Raycast(Stick.PointB.transform.position, Delta.normalized, Delta.magnitude * percet * 2, layer_mask);

                        if (hit.collider != null)
                        {
                            Stick.PointB.transform.position += Delta.normalized * hit.distance;
                        }
                        else
                        {
                            Stick.PointB.transform.position -= ((Delta * percet) * 2);
                        }
                    }
                    else
                    {
                        Stick.PointB.transform.position -= ((Delta * percet) * 2);
                    }*/

                    Stick.PointB.transform.position -= ((Delta * percet) * 2);
                }
            }
        }
    }

    private void VerletIntegrate(float InSubstepTime)
    {
        float SubstepTimeSqr = InSubstepTime * InSubstepTime;
        Vector3 Gravity = Physics.gravity;

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Verlet_Rope_Point particle = Points[PointIndex];
            if (particle.p_free)
            {
                Vector3 Velocity = (particle.transform.position - particle.OldPosition) * friction;
                particle.NewPosition =  Velocity + (SubstepTimeSqr * Gravity);

                particle.OldPosition = particle.transform.position;
                particle.transform.position += particle.NewPosition;
            }
            else
            {
                particle.NewPosition = particle.transform.position;
            }
        }
    }

    private void Update_LineRender()
    {
        // Update render position
        for (int SegmentIndex = 0; SegmentIndex < NumPoints; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Points[SegmentIndex].transform.position);
        }
    }
}
