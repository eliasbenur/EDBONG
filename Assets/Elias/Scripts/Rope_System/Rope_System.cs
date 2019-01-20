using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope_System : MonoBehaviour {

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

    bool rop_tens = false;

    public AnimationCurve animatedcurve;

    public bool state_surround;

    [System.Serializable] public struct coll_pos
    {
        public int point_nul;
        public float dist;
    }

    List<coll_pos> list_coll = new List<coll_pos>();

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
        _lineRenderer.positionCount = NumPoints;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;

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

                /*var collider = particle.gameObject.AddComponent<BoxCollider2D>();
                collider.offset = new Vector2(0,0);
                collider.size = new Vector2(1,2.2f);*/
                var collider = particle.gameObject.AddComponent<CircleCollider2D>();
                collider.radius = 0.5f;
                particle.p_free = false;
            }
            else{
                particle.p_free = true;
            }
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
        DistMaxConstraints(P1_mov, P2_mov);
        Update_LineRender();
    }

    private void DistMaxConstraints(Vector3 P1_mov, Vector3 P2_mov)
    {
        Coll_RollB(NumPoints/2);

        //CableStart.transform.position = Points[0].transform.position;
        //CableEnd.transform.position = Points[NumPoints - 1].transform.position;

        Points[0].new_pos_p1 = P1_mov;

        Points[NumPoints - 1].new_pos_p2 = P2_mov;

        for (int curr_inte = 0; curr_inte <= Iterations; curr_inte++)
        {
            DistMaxP1();
            DistMaxP2();

            DistMaxCalcul(0);

        }

        Extrems_Correction();

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            if (!state_surround)
            {
                ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + (Vector2)ParticleA.new_pos);
            }else
            {
                if (!ParticleA.coll_state)
                {
                    //TODO: add new pos to the RiGbody2D 
                    ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + (Vector2)ParticleA.new_pos);
                }
            }


            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
            ParticleA.new_pos = Vector3.zero;
        }
        
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
                Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
                ParticleB.new_pos_p1 = P_B;
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
                Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
                ParticleB.new_pos_p2 = P_B;
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
            if (ParticleA.coll_state)
            {
                lisr_p.Add(ParticleA);
            }
        }
        if (lisr_p.Count != 0)
        {
            Rope_Point p = lisr_p[lisr_p.Count / 2];
            new_indew = Points.IndexOf(p);
        }


        for (int PointIndex = new_indew; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];

            Vector3 Delta = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint && !state_surround)
            {
                ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
            }
            else if (state_surround)
            {
                if (!ParticleB.coll_state)
                {
                    //TODO: add new pos to the RiGbody2D 
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                    //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
                }
            }
        }

        for (int PointIndex = new_indew; PointIndex > 0; PointIndex--)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];

            Vector3 Delta = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;


            if (CurrentDistance > MaxLenght_xPoint && !state_surround)
            {
                ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                //ParticleA.transform.position -= Delta.normalized * ((CurrentDistance - MaxLenght_xPoint) / 2);
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
        // Update render position
        for (int SegmentIndex = 0; SegmentIndex < NumPoints; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Points[SegmentIndex].transform.position);
        }
    }


}
