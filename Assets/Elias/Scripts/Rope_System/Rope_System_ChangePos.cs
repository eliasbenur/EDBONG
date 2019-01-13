using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope_System_ChangePos : MonoBehaviour
{

    #region Ini_Properties
    private List<Rope_Point> Points = new List<Rope_Point>();
    public List<GameObject> Objects_Coll = new List<GameObject>();
    public Transform PrefabPoint;
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

    [System.Serializable]
    public struct coll_pos
    {
        public int point_nul;
        public float dist;
    }

    List<coll_pos> list_coll = new List<coll_pos>();

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
        _lineRenderer.startWidth = 2f;
        _lineRenderer.endWidth = 2f;
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;

        // "Distance" between the 2 start points
        Vector3 Delta = CableEnd.position - CableStart.position;

        for (int ParticleIndex = 0; ParticleIndex < NumPoints; ParticleIndex++)
        {
            Transform newTransform = Instantiate(PrefabPoint, Vector3.zero, Quaternion.identity) as Transform;

            float Alpha = (float)ParticleIndex / (float)(NumPoints - 1);
            Vector3 InitializePosition = CableStart.transform.position + (Alpha * Delta);

            //Ini variables of the Particle
            Rope_Point particle = newTransform.GetComponent<Rope_Point>();
            particle.transform.position = InitializePosition;
            particle.transform.parent = this.transform;
            particle.transform.tag = "rope";
            particle.name = "Point_" + ParticleIndex.ToString();

            particle.gameObject.layer = 9;
            Physics2D.IgnoreLayerCollision(8, 9);
            Physics2D.IgnoreLayerCollision(9, 9);

            //RAYCAST TRIGER
            //Transform pref_tri = Instantiate(PrefabRay_Trig, newTransform.position, Quaternion.identity) as Transform;
            // pref_tri.gameObject.transform.parent = newTransform;

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
    }
    # endregion

    // Update is called once per frame
    void FixedUpdate()
    {
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


        //Points[0].transform.position += P1_mov;
        //Points[0].GetComponent<Rigidbody2D>().velocity = (Vector2)P1_mov;
        //Points[0].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[0].transform.position +  (Vector2)P1_mov);
        Points[0].new_pos_p1 = P1_mov;

        //Points[NumPoints - 1].transform.position += P2_mov;
        //Points[NumPoints - 1].GetComponent<Rigidbody2D>().velocity = (Vector2)P2_mov;
        //Points[NumPoints - 1].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[NumPoints - 1].transform.position + (Vector2)P2_mov);
        Points[NumPoints - 1].new_pos_p2 = P2_mov;

        for (int curr_inte = 0; curr_inte <= Iterations; curr_inte++)
        {
            //if (P1_mov != Vector3.zero)
            //{
            DistMaxP1();
            //}
            //if (P2_mov != Vector3.zero)
            //{
            DistMaxP2();
            //}
            //if (curr_inte == 0)
            //{
            //    rop_tens = Check_MaxRope();
            //}
            DistMaxCalcul(0);
            //CheckCollision();

        }

        Extrems_Correction();
    }

    private void DistMaxP1()
    {
        bool stop = false;

        for (int PointIndex = 0; PointIndex < NumPoints - 2 && !stop; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];

            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos_p1) - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint)
            //if(true)
            {
                Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
                ParticleB.new_pos_p1 = P_B;
            }
            else
            {
                ParticleB.new_pos_p1 = Vector3.zero;
            }
        }
    }

    private void DistMaxP2()
    {
        bool stop = false;

        for (int PointIndex = NumPoints - 1; PointIndex > 1 && !stop; PointIndex--)
        {

            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];

            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos_p2) - ParticleB.transform.position;
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint)
            //if(true)
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

    private void CheckCollision_Old()
    {
        /*for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++){
            Rope_Point ParticleA = Points[PointIndex];
            Vector3 sum_nex_pos = ParticleA.new_pos_p1 + ParticleA.new_pos_p2;
            ParticleA.new_pos = sum_nex_pos;
        }*/
        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            //Raycast - Collision Detection
            /*int layer_mask = LayerMask.GetMask("Objects");
            Vector3 sum_nex_pos = ParticleA.new_pos;
            RaycastHit2D hit = Physics2D.Raycast(ParticleA.transform.position, sum_nex_pos.normalized, sum_nex_pos.magnitude, layer_mask);
            if (hit.collider != null){
                ParticleA.new_pos = sum_nex_pos.normalized * hit.distance + (Vector3)hit.normal * 0.1f;
            }*/

            CircleCollider2D collid = GameObject.Find("Target2").GetComponent<CircleCollider2D>();

            if (collid.bounds.Contains((Vector2)ParticleA.transform.position))
            {
                ColliderDistance2D coll_distance = ParticleA.gameObject.GetComponent<CircleCollider2D>().Distance(collid);
                ParticleA.transform.position += (Vector3)coll_distance.normal * coll_distance.distance;
            }
        }

        Vector3 Delta = Points[NumPoints / 2].transform.position - Points[(NumPoints / 2) - 1].transform.position;
        float CurrentDistance = Delta.magnitude;
        if (CurrentDistance > MaxLenght_xPoint)
        {
            Vector3 P_B = ((CurrentDistance - MaxLenght_xPoint) / 2) * Delta.normalized;
            Points[(NumPoints / 2) - 1].transform.position += P_B;
            Points[NumPoints / 2].transform.position -= P_B;
        }
        int mid_right = NumPoints / 2;
        int mid_left = (NumPoints / 2) - 1;

        for (int SegmentIndex = mid_right; SegmentIndex < NumPoints - 1; SegmentIndex++)
        {
            Rope_Point ParticleA = Points[SegmentIndex];
            Rope_Point ParticleB = Points[SegmentIndex + 1];
            // Solve for this pair of particles
            Vector3 Delta2 = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance2 = Delta2.magnitude;
            if (CurrentDistance2 > MaxLenght_xPoint)
            {
                Vector3 P_B = (CurrentDistance2 - MaxLenght_xPoint) * Delta2.normalized;
                ParticleB.transform.position += P_B;
            }
        }
        for (int SegmentIndex2 = mid_left; SegmentIndex2 > 0; SegmentIndex2--)
        {
            Rope_Point ParticleA = Points[SegmentIndex2];
            Rope_Point ParticleB = Points[SegmentIndex2 - 1];
            // Solve for this pair of particles
            Vector3 Delta2 = ParticleA.transform.position - ParticleB.transform.position;
            float CurrentDistance2 = Delta2.magnitude;
            if (CurrentDistance2 > MaxLenght_xPoint)
            {
                Vector3 P_B = (CurrentDistance2 - MaxLenght_xPoint) * Delta2.normalized;
                ParticleB.transform.position += P_B;
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
        Vector3 obj_mov = Vector3.zero;
        GameObject obj_tomov = null;

        list_coll.Clear();


        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            /*int layer_mask = LayerMask.GetMask("Objects");
            RaycastHit2D hit = Physics2D.Raycast(ParticleA.transform.position, (ParticleA.new_pos_p1 + ParticleA.new_pos_p2).normalized, (ParticleA.new_pos_p1 + ParticleA.new_pos_p2).magnitude + 0.1f, layer_mask);

            if (hit.collider != null){

                ParticleA.new_pos = (ParticleA.new_pos_p1 + ParticleA.new_pos_p2).normalized * (hit.distance - 0.1f);

                if (hit.transform.tag == "obj_mov" || hit.transform.tag == "enemy_mov" || hit.transform.tag == "Objects"){
                    coll_pos cp = new coll_pos();
                    cp.point_nul = PointIndex;
                    cp.dist = ((ParticleA.new_pos_p1 + ParticleA.new_pos_p2).magnitude + 0.1f) - (hit.distance - 0.1f);
                    if (list_coll.Count > 0){
                        bool found = false;
                        for (int x = 0; x < list_coll.Count; x++){
                            if (list_coll[x].dist < cp.dist && !found){
                                list_coll.Insert(x, cp);
                                found = true;
                            }
                        }
                        if (!found){
                            list_coll.Add(cp);
                        }
                    }
                    else{
                        list_coll.Add(cp);
                    }
                }
            }
            else{*/
            ParticleA.new_pos = ParticleA.new_pos_p1 + ParticleA.new_pos_p2;
            //}

        }
        for (int curr_ite = 0; curr_ite < Iterations; curr_ite++)
        {
            for (int x = 0; x < list_coll.Count; x++)
            {
                //Coll_RollB(list_coll[x].point_nul);
            }
        }


        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            //ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + (Vector2)ParticleA.new_pos);
            ParticleA.transform.position += ParticleA.new_pos;

            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
            ParticleA.new_pos = Vector3.zero;
        }

        //Vector2 lal = (ParticleA.new_pos_p1 + ParticleA.new_pos_p2);
        //ParticleA.transform.position += ParticleA.new_pos_p1 + ParticleA.new_pos_p2;
        //ParticleA.GetComponent<Rigidbody2D>().velocity = (Vector2)(ParticleA.new_pos_p1 + ParticleA.new_pos_p2);
        //ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + lal);

        //ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + new Vector2(0,0.2f));

        //Extrems_Correction();

        //ParticleA.transform.position += ParticleA.new_pos_p1 + ParticleA.new_pos_p2;



        //Debug.Log(obj_mov);
        //Debug.DrawLine(GameObject.Find("Target2").transform.position, GameObject.Find("Target2").transform.position + obj_mov, Color.white, 2.5f);

    }

    private void Coll_RollB(int Index_coll)
    {
        for (int PointIndex = Index_coll; PointIndex < NumPoints - 1; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex + 1];

            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance > MaxLenght_xPoint)
            {
                /*if (new Pose IS INSIDE COLLIDER, DISTANCE... PARA SACARLO FUERA)
                 {

                 }*/
                //ParticleB.transform.position += Delta.normalized * (CurrentDistance - MaxLenght_xPoint);
                ParticleB.new_pos += Delta.normalized * (CurrentDistance - MaxLenght_xPoint);
                //ParticleB.transform.position += ParticleB.new_pos;
                //ParticleB.new_pos = Vector3.zero;
            }
            else
            {
                //PointIndex = NumPoints;
            }
        }

        for (int PointIndex = Index_coll; PointIndex > 0; PointIndex--)
        {
            Rope_Point ParticleA = Points[PointIndex];
            Rope_Point ParticleB = Points[PointIndex - 1];

            Vector3 Delta = (ParticleA.transform.position + ParticleA.new_pos) - (ParticleB.transform.position + ParticleB.new_pos);
            float CurrentDistance = Delta.magnitude;


            if (CurrentDistance > MaxLenght_xPoint)
            {
                /* if (new Pose IS INSIDE COLLIDER, DISTANCE... PARA SACARLO FUERA)
                 {

                 }*/
                //ParticleB.transform.position += Delta.normalized * (CurrentDistance - MaxLenght_xPoint);
                ParticleB.new_pos += Delta.normalized * (CurrentDistance - MaxLenght_xPoint);
                //ParticleB.transform.position += ParticleB.new_pos;
                //ParticleB.new_pos = Vector3.zero;

            }
            else
            {
                //PointIndex = 0;
            }
        }

        //CheckCollision();
    }


    private void Extrems_Correction()
    {
        Vector3 Delta = (Points[1].transform.position + Points[1].new_pos_p1 + Points[1].new_pos_p2) - (Points[0].transform.position + Points[0].new_pos_p1 + Points[0].new_pos_p2);
        float CurrentDistance = Delta.magnitude;

        if (CurrentDistance > MaxLenght_xPoint)
        {
            Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
            Points[0].transform.position += P_B;
            //Points[0].GetComponent<Rigidbody2D>().velocity = (Vector2)P_B;
            //Points[0].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[0].transform.position +  (Vector2)P_B);
        }

        Delta = (Points[NumPoints - 2].transform.position + Points[NumPoints - 2].new_pos_p1 + Points[NumPoints - 2].new_pos_p2) - (Points[NumPoints - 1].transform.position + Points[NumPoints - 1].new_pos_p1 + Points[NumPoints - 1].new_pos_p2);
        CurrentDistance = Delta.magnitude;

        if (CurrentDistance > MaxLenght_xPoint)
        {
            Vector3 P_B = (CurrentDistance - MaxLenght_xPoint) * Delta.normalized;
            Points[NumPoints - 1].transform.position += P_B;
            //Points[NumPoints - 1].GetComponent<Rigidbody2D>().velocity = (Vector2)P_B;
            //Points[NumPoints - 1].GetComponent<Rigidbody2D>().MovePosition((Vector2)Points[NumPoints - 1].transform.position + (Vector2)P_B);
        }

        CableStart.position = Points[0].transform.position;
        CableEnd.position = Points[NumPoints - 1].transform.position;
    }

    private void Update_LineRender()
    {
        // Update render position
        for (int SegmentIndex = 0; SegmentIndex < NumPoints; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Points[SegmentIndex].transform.position);
        }
    }

    private bool Check_MaxRope()
    {

        if (Points[NumPoints - 2].new_pos_p1 != Vector3.zero && (Points[1].new_pos_p2) != Vector3.zero)
        {
            //Debug.Log(Vector3.Dot((Vector3)mov_P2.normalized, Points[NumPoints - 2].new_pos_p1.normalized));
            return true;
        }
        else
        {
            return false;
        }
    }

}
