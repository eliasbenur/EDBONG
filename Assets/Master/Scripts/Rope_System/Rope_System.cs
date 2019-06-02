using System.Collections.Generic;
using UnityEngine;

public class Rope_System : MonoBehaviour {

    #region Ini_Properties
    [HideInInspector] public List<Rope_Point> Points = new List<Rope_Point>();
    //Movement every Frame
    private Vector2 mov_P1;
    private Vector2 mov_P2;

    public Rope_Point PrefabPoint;
    public float MaxLenght_xPoint = 0.2f;
    public int NumPoints = 50;
    public Transform CableStart;
    public Transform CableEnd;

    // Every once is called the Chain Update
    private float TimeRemainder = 0f;
    private float SubstepTime = 0.02f;
    // How many times we interate in the same frame call
    public int Iterations;

    // Visual of the Chain
    public Sprite chainA;
    public Sprite chainB;

    // Use this for initialization
    void Start()
    {
        //The layers to ignore
        Physics2D.IgnoreLayerCollision(19, 9);
        Physics2D.IgnoreLayerCollision(20, 9);
        Physics2D.IgnoreLayerCollision(9, 9);
        Physics2D.IgnoreLayerCollision(9, 15);

        Points.Clear();

        mov_P1 = Vector3.zero;
        mov_P2 = Vector3.zero;

        // "Distance" between the 2 start points
        Vector3 Delta = CableEnd.position - CableStart.position;

        //Instantiate the points of the chain
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

            Points.Add(particle);

            if (ParticleIndex == 0){

                particle.gameObject.layer = 19;
            }else if (ParticleIndex == (NumPoints - 1))
            {
                particle.gameObject.layer = 20;
            }

            if (ParticleIndex % 2 == 0)
            {
                particle.GetComponent<SpriteRenderer>().sprite = chainA;
            }
            else
            {
                particle.GetComponent<SpriteRenderer>().sprite = chainB;
            }
        }
    }
    #endregion

    // Update is called once per frame
    void FixedUpdate () {
        TimeRemainder += Time.fixedDeltaTime;
        while (TimeRemainder > SubstepTime)
        {
            PreformSubstep(mov_P1 * Time.fixedDeltaTime, mov_P2 * Time.fixedDeltaTime);
            TimeRemainder -= SubstepTime;
        }
    }

    public void set_mov_P1(Vector2 movement)
    {
        mov_P1 = movement;
    }

    public void set_mov_P2(Vector2 movement)
    {
        mov_P2 = movement;
    }

    public void PreformSubstep(Vector3 P1_mov, Vector3 P2_mov)
    {
        Chain_PhysicsUpdate(P1_mov, P2_mov);
        VisualRotation_Points();
    }

    public List<Rope_Point> get_points()
    {
        return Points;
    }

    private void Chain_PhysicsUpdate(Vector3 P1_mov, Vector3 P2_mov)
    {
        // IF with the MovePositions some points are + far from MaxLenght (for collisions,etc..) we use transform position instead to correct their position
        CentredForce(NumPoints/2);

        Points[0].new_pos_p1 = P1_mov;

        Points[NumPoints - 1].new_pos_p2 = P2_mov;

        for (int curr_inte = 0; curr_inte <= Iterations; curr_inte++)
        {
            //We calcule the new position of the point depending of the movement of the extrems points
            DistMaxP1();
            DistMaxP2();

            DistMaxCalcul();
        }

        //Moving the points
        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++)
        {
            Rope_Point ParticleA = Points[PointIndex];

            ParticleA.GetComponent<Rigidbody2D>().MovePosition((Vector2)ParticleA.transform.position + (Vector2)ParticleA.new_pos);

            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
            ParticleA.new_pos = Vector3.zero;
        }

    }

    /* From the Positions of the Player1 we calculate the amount of movement to move every point to keep them togeher */
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

    /* From the Positions of the Player2 we calculate the amount of movement to move every point to keep them togeher */
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

    private void DistMaxCalcul()
    {

        for (int PointIndex = 0; PointIndex < NumPoints; PointIndex++) {
            Rope_Point ParticleA = Points[PointIndex];

            ParticleA.new_pos += ParticleA.new_pos_p1 + ParticleA.new_pos_p2;

            ParticleA.new_pos_p1 = Vector3.zero;
            ParticleA.new_pos_p2 = Vector3.zero;
        }
    }

    //We apply a force from the center of the chain ("to inside") to keep the points together
    //If the chain is colliding an enemie the center is changed to this one
    private void CentredForce(int Index_coll)
    {
        int new_indew = Index_coll;

        //We search if a point is colliding with an enemie, if yes, we change de index coll to use after
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

            if (CurrentDistance > MaxLenght_xPoint)
            {
                if (!ParticleB.enemie_coll)
                {
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                }
                else
                {
                    ParticleB.transform.position += (Delta.normalized*0.2f) * ((CurrentDistance - MaxLenght_xPoint));
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


            if (CurrentDistance > MaxLenght_xPoint)
            {
                if (!ParticleB.enemie_coll)
                {
                    ParticleB.transform.position += Delta.normalized * ((CurrentDistance - MaxLenght_xPoint));
                }
                else
                {
                    ParticleB.transform.position += (Delta.normalized * 0.2f) * ((CurrentDistance - MaxLenght_xPoint));
                }
            }
        }
    }

    private void VisualRotation_Points()
    {
        for (int SegmentIndex = 0; SegmentIndex < NumPoints - 1; SegmentIndex++)
        {
            Vector3 difference = Points[SegmentIndex+1].transform.position - Points[SegmentIndex].transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            Points[SegmentIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }
    }


}
