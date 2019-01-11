using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Using Verlet Integration to solve constrains and velocity
// This iteration is working much better then my first try
// at making a rope with physics. I've tried to do the same
// but with a different approach and it didn't work out.
// This code is all from UE4 CableComponent.
// Converted it in C# to test within Unity, due slow pc at home.
public class UParticleSystem_E : MonoBehaviour {
	#region public Properties
	public Transform PrefabParticle;
	public Transform CableStart;
	public Transform CableEnd;
	public int CableLengthDesired = 10;
    public int CableMaxLenght = 40;
	public int NumSegments = 50;
	public int Elasticity_Strenght = 1; /* Num of Iteration of the Contraint */
    public int SolverIterations_DMAX = 1;
    #endregion

    #region private Properties
    private List<FParticle_E> Particles = new List<FParticle_E>();
	public LineRenderer _lineRenderer;
	private float TimeRemainder = 0f;
	private float SubstepTime = 0.02f;
    #endregion

    #region Proto's Variables Test
    public float somme;
    private Transform objectTransfom;
    private float noMovementThreshold = 0.0001f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;
    public bool IsMoving
    {
        get { return isMoving; }
    }
    #endregion

    #region Unity Methods

    private void Awake()
    {
        // ????
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero; 
        }
    }

    void Start() {
		int NumParticles = NumSegments + 1;   // ????
		Particles.Clear();
		
		// Use linerenderer as visual cable representation
		_lineRenderer = transform.GetComponent<LineRenderer>();
		if (_lineRenderer == null) {
			_lineRenderer = gameObject.AddComponent<LineRenderer>();
		}
		_lineRenderer.SetVertexCount(NumSegments + 1);
		_lineRenderer.SetWidth(.2f, .2f);
		_lineRenderer.SetColors(Color.black, Color.black);

        // "Distance" between the 2 start points
		Vector3 Delta = CableEnd.position - CableStart.position;

		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
            // Instantiation of the Particle
			Transform newTransform = Instantiate(PrefabParticle, Vector3.zero, Quaternion.identity) as Transform;

            //Pos Initial of the Particle
            float Alpha = (float)ParticleIndex / (float)NumParticles;
			Vector3 InitializePosition = CableStart.transform.position + (Alpha * Delta);

            //Ini variables of the Particle
            FParticle_E particle = newTransform.GetComponent<FParticle_E>();
			particle.position = InitializePosition;
			particle.OldPosition = InitializePosition;
			particle.transform.parent = this.transform;
			particle.name = "Particle_0" + ParticleIndex.ToString();
            // Layer to "Rope" To ignore the collision betwen the Player and the Rope
            particle.gameObject.layer = 9;
            Physics2D.IgnoreLayerCollision(8, 9);

            //List of Particles
            Particles.Add(particle);



            if (ParticleIndex == 0 || ParticleIndex == (NumParticles - 1)) {
				particle.bFree = false;
			} else {
				particle.bFree = true;
			}
		}
        //Debug.Log(Particles.Count);
    }

    void FixedUpdate() {
        // Update start+end positions first
        /*FParticle_E StartParticle = Particles[0];
		StartParticle.position = StartParticle.OldPosition = CableStart.position;
        FParticle_E EndParticle = Particles[NumSegments];
		EndParticle.position = EndParticle.OldPosition = CableEnd.position;*/

		Vector3 Gravity = Physics.gravity;
		float UseSubstep = Mathf.Max(SubstepTime, 0.005f);

        // Every 0.02s -->PerformSubstep();
		TimeRemainder += Time.fixedDeltaTime;
		while (TimeRemainder > UseSubstep) {
			PreformSubstep(UseSubstep, Gravity);
			TimeRemainder -= UseSubstep;
		}   
	}
    #endregion

	#region private Methods
	private void PreformSubstep(float InSubstepTime, Vector3 Gravity) {
		//VerletIntegrate(InSubstepTime, Gravity);
		//SolveConstraints();
	}

    private void VerletIntegrate(float InSubstepTime, Vector3 Gravity) {
		int NumParticles = NumSegments + 1;
		float SubstepTimeSqr = InSubstepTime * InSubstepTime; // ???
		
		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
            FParticle_E particle = Particles[ParticleIndex];
			if (particle.bFree) {
				Vector3 Velocity = particle.position - particle.OldPosition;
				Vector3 NewPosition = particle.position + Velocity + (SubstepTimeSqr * Gravity); //  SubstepTimeSqr to reduce the gravity effect

                particle.OldPosition = particle.position;
				particle.position = NewPosition;
			}
		}
	}

    private void SolveConstraints()
    {
        int NumParticles = NumSegments + 1;
        float SegmentLength = CableLengthDesired / (float)NumSegments;
        float SegmentMaxLenght = CableMaxLenght / (float)NumParticles;

        // For each iteration
        for (int IterationIndex = 0; IterationIndex < Elasticity_Strenght; IterationIndex++)
        {
            // For each segment
            for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
            {
                FParticle_E ParticleA = Particles[SegmentIndex];
                FParticle_E ParticleB = Particles[SegmentIndex + 1];
                // Solve for this pair of particles
                //SolveDistanceConstraint(ParticleA, ParticleB, SegmentLength);
            }
        }

        // For each iteration
        for (int IterationIndex = 0; IterationIndex < SolverIterations_DMAX; IterationIndex++)
        {
            SolveDistanceMax_Old(SegmentMaxLenght);

            //SolveCollitionConstrains();
        }

        Vector3 Delta2 = Particles[NumSegments - 1].position - Particles[NumSegments].position;
        float CurrentDistance2 = Delta2.magnitude;
        Vector3 Delta3 = Particles[1].position - Particles[0].position;
        float CurrentDistance3 = Delta3.magnitude;


        if (CurrentDistance2 > SegmentMaxLenght && CurrentDistance3 > SegmentMaxLenght)
        {
            Vector3 P_B2 = (CurrentDistance3 - SegmentMaxLenght) * Vector3.Normalize(Delta3);
            Particles[0].position += P_B2 / 2;
            Particles[1].position -= P_B2 / 2;
            CableStart.position = Particles[0].position;

            Vector3 P_B = (CurrentDistance2 - SegmentMaxLenght) * Vector3.Normalize(Delta2);
            Particles[NumSegments].position += P_B / 2;
            Particles[NumSegments - 1].position -= P_B / 2;
            CableEnd.position = Particles[NumSegments].position;
        }
        else if (CurrentDistance2 > SegmentMaxLenght)
        {
            Vector3 P_B = (CurrentDistance2 - SegmentMaxLenght) * Vector3.Normalize(Delta2);
            Particles[NumSegments].position += P_B / 2;
            Particles[NumSegments - 1].position -= P_B / 2;
            CableEnd.position = Particles[NumSegments].position;

        }
        else if (CurrentDistance3 > SegmentMaxLenght)
        {
            Vector3 P_B = (CurrentDistance3 - SegmentMaxLenght) * Vector3.Normalize(Delta3);
            Particles[0].position += P_B / 2;
            Particles[1].position -= P_B / 2;
            CableStart.position = Particles[0].position;
        }

        update_lineRenderer();
    }

    void update_lineRenderer()
    {
        // Update render position
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Particles[SegmentIndex].position);
        }
        _lineRenderer.SetPosition(NumSegments, Particles[NumSegments].position);
    }

    void SolveDistanceMax_Old(float SegmentMaxLenght)
    {

        int mid_left = NumSegments / 2;
        int mid_right = (NumSegments + 1) / 2;

        for (int SegmentIndex = 0; SegmentIndex < mid_left; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            // Solve for this pair of particles
            SolveDistanceMax_Particles_Old(ParticleA, ParticleB, SegmentMaxLenght);
        }
        for (int SegmentIndex2 = NumSegments; SegmentIndex2 > mid_right; SegmentIndex2--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex2];
            FParticle_E ParticleB = Particles[SegmentIndex2 - 1];
            // Solve for this pair of particles of the other side
            SolveDistanceMax_Particles_Old(ParticleA, ParticleB, SegmentMaxLenght);
        }

        Vector3 Delta = Particles[mid_left].position - Particles[mid_right].position;
        float CurrentDistance = Delta.magnitude;
        if (CurrentDistance > SegmentMaxLenght)
        {
            Vector3 P_B = ((CurrentDistance - SegmentMaxLenght) / 2) * Vector3.Normalize(Delta);
            Particles[mid_right].position += P_B;
            Particles[mid_left].position -= P_B;
        }

        for (int SegmentIndex = mid_right; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            // Solve for this pair of particles
            SolveDistanceMax_Particles_Old(ParticleA, ParticleB, SegmentMaxLenght);
        }
        for (int SegmentIndex2 = mid_left; SegmentIndex2 > 0; SegmentIndex2--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex2];
            FParticle_E ParticleB = Particles[SegmentIndex2 - 1];
            // Solve for this pair of particles of the other side
            SolveDistanceMax_Particles_Old(ParticleA, ParticleB, SegmentMaxLenght);
        }

        

    }
    /*
     * 1. Force aplied from the left and the right, 50% of the rope each one
     * 2. The mid point correction
     * 3. = 1, but backwards
     * 4. Player correction
     * */
    void SolveDistanceMax(float SegmentMaxLenght)
    {
        int mid_left = NumSegments / 2;
        int mid_right = ( NumSegments + 1 ) / 2;

        /*1*/
        for (int SegmentIndex = 0; SegmentIndex < mid_left; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            // Solve for this pair of particles
            SolveDistanceMax_Particles(ParticleA, ParticleB, SegmentMaxLenght);
        }
        for (int SegmentIndex2 = NumSegments; SegmentIndex2 > mid_right; SegmentIndex2--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex2];
            FParticle_E ParticleB = Particles[SegmentIndex2 - 1];
            // Solve for this pair of particles of the other side
            SolveDistanceMax_Particles(ParticleA, ParticleB, SegmentMaxLenght);
        }

        /*2*/
        Vector3 Delta = Particles[mid_left].position - Particles[mid_right].position;
        float CurrentDistance = Delta.magnitude;
        if (CurrentDistance > SegmentMaxLenght)
        {
            Vector3 P_B = ((CurrentDistance - SegmentMaxLenght) / 2) * Vector3.Normalize(Delta);
            Particles[mid_right].position += P_B;
            Particles[mid_right].OldPosition = Particles[mid_right].position;
            Particles[mid_left].position -= P_B;
            Particles[mid_left].OldPosition = Particles[mid_left].position;
        }/*else if ()
        {

        }else if ()
        {

        }*/

        /*3*/
        for (int SegmentIndex = mid_right; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            // Solve for this pair of particles
            SolveDistanceMax_Particles(ParticleA, ParticleB, SegmentMaxLenght);
        }
        for (int SegmentIndex2 = mid_left; SegmentIndex2 > 0; SegmentIndex2--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex2];
            FParticle_E ParticleB = Particles[SegmentIndex2 - 1];
            // Solve for this pair of particles of the other side
            SolveDistanceMax_Particles(ParticleA, ParticleB, SegmentMaxLenght);
        }

        /*4*/

        Vector3 Delta2 = Particles[NumSegments - 1].position - Particles[NumSegments].position; 
        float CurrentDistance2 = Delta2.magnitude;
        Vector3 Delta3 = Particles[1].position - Particles[0].position;  
        float CurrentDistance3 = Delta3.magnitude;


        if (CurrentDistance2 > SegmentMaxLenght && CurrentDistance3 > SegmentMaxLenght)
        {
            Vector3 P_B2 = (CurrentDistance3 - SegmentMaxLenght) * Vector3.Normalize(Delta3);
            Particles[0].position += P_B2;
            Particles[0].OldPosition = Particles[0].position;
            CableStart.position = Particles[0].position;
            
            Vector3 P_B = (CurrentDistance2 - SegmentMaxLenght) * Vector3.Normalize(Delta2);
            Particles[NumSegments].position += P_B;
            Particles[NumSegments].OldPosition = Particles[NumSegments].position;
            CableEnd.position = Particles[NumSegments].position;
        }
        else if (CurrentDistance2 > SegmentMaxLenght)
        {
            Vector3 P_B = (CurrentDistance2 - SegmentMaxLenght) * Vector3.Normalize(Delta2);
            Particles[NumSegments].position += P_B;
            Particles[NumSegments].OldPosition = Particles[NumSegments].position;
            CableEnd.position = Particles[NumSegments].position;
            
        }
        else if (CurrentDistance3 > SegmentMaxLenght)
        {
            Vector3 P_B = (CurrentDistance3 - SegmentMaxLenght) * Vector3.Normalize(Delta3);
            Particles[0].position += P_B;
            Particles[0].OldPosition = Particles[0].position;
            CableStart.position = Particles[0].position;
        }
    }

    /* Correction betwen 2 points, with a max distance. The point B is he moves to point A allways */
    void SolveDistanceMax_Particles(FParticle_E ParticleA, FParticle_E ParticleB, float SegmentMaxLenght)
    {
        Vector3 Delta = ParticleA.position - ParticleB.position;
        float CurrentDistance = Delta.magnitude;

        if (CurrentDistance > SegmentMaxLenght)
        {
            Vector3 P = SegmentMaxLenght * Vector3.Normalize(Delta);
            Vector3 P_B = (CurrentDistance - SegmentMaxLenght) * Vector3.Normalize(Delta);
            if (ParticleA.bFree && ParticleB.bFree)
            {
                ParticleB.position += P_B;
                ParticleB.OldPosition = ParticleB.position;
            }
            else if (ParticleA.bFree)
            {
                //ParticleB.position += P_B;
                //ParticleB.OldPosition = ParticleB.position;
                //CableEnd.position = ParticleB.position;

            }
            else if (ParticleB.bFree)
            {
                ParticleB.position += P_B;
                ParticleB.OldPosition = ParticleB.position;
            }

        }
    }

    /* Correction betwen 2 points, with a max distance. The point B is he moves to point A allways */
    void SolveDistanceMax_Particles_Old(FParticle_E ParticleA, FParticle_E ParticleB, float SegmentMaxLenght)
    {
        Vector3 Delta = ParticleA.position - ParticleB.position;
        float CurrentDistance = Delta.magnitude;

        if (CurrentDistance > SegmentMaxLenght)
        {

            Vector3 P = SegmentMaxLenght * Vector3.Normalize(Delta);
            Vector3 P_B = (CurrentDistance - SegmentMaxLenght) * Vector3.Normalize(Delta);
            int layer_mask = LayerMask.GetMask("Objects");

            //RaycastHit2D hit = Physics2D.Raycast(ParticleB.position, P_B.normalized, P_B.magnitude, layer_mask);
            //if (hit.collider != null)
            //{
                //hit.normal // perpendicular
           //     Vector3 inertia = Vector3.Normalize(Delta) * (CurrentDistance - (hit.distance + SegmentMaxLenght));
           //     inertia = Vector3.Project(inertia, new Vector3(hit.normal.y, hit.normal.x, 0));
            //    ParticleB.position += (hit.distance) * Vector3.Normalize(Delta) /*+ inertia*/; 
            //}
            //else
            //{
                if (ParticleA.bFree && ParticleB.bFree)
                {
                    ParticleB.position += P_B;
                    //ParticleB.position += P_B/2;
                    //ParticleA.position -= P_B/2;
                }
                else if (ParticleA.bFree)
                {
                    //ParticleA.position -= P_B;
                    //ParticleB.position += P_B / 2;
                    //ParticleA.position -= P_B / 2;

                }
                else if (ParticleB.bFree)
                {
                    ParticleB.position += P_B;
                    //ParticleB.position += P_B / 2;
                    //ParticleA.position -= P_B / 2;
                }

                /*
                if (GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleB.position.x, ParticleB.position.y)))
                {
                    float distance = Mathf.Abs(ParticleB.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target").GetComponent<CircleCollider2D>()).distance);

                    Vector3 Delta2 = GameObject.Find("Target").transform.position - ParticleB.transform.position;
                    Vector3 P_B2 = distance * Vector3.Normalize(Delta2);
                    ParticleB.transform.position -= P_B2 / 2;
                    GameObject.Find("Target").transform.position += P_B2 / 2;
                }*/
            //}


        }
    }

    void SolveDistanceConstraint(FParticle_E ParticleA, FParticle_E ParticleB, float DesiredDistance) {
		// Find current difference between particles
		Vector3 Delta = ParticleB.position - ParticleA.position;
		float CurrentDistance = Delta.magnitude;
		float ErrorFactor = (CurrentDistance - DesiredDistance) / CurrentDistance;

        // Only move free particles to satisfy constraints
        if (ParticleA.bFree && ParticleB.bFree)
        {
            ParticleA.position += ErrorFactor * 0.5f * Delta;
            ParticleB.position -= ErrorFactor * 0.5f * Delta;
        }
        else if (ParticleA.bFree)
        {
            //ParticleA.position += ErrorFactor * Delta;
            ParticleA.position += ErrorFactor * 0.5f * Delta;
            ParticleB.position -= ErrorFactor * 0.5f * Delta;
        }
        else if (ParticleB.bFree)
        {
            //ParticleB.position -= ErrorFactor * Delta;
            ParticleA.position += ErrorFactor * 0.5f * Delta;
            ParticleB.position -= ErrorFactor * 0.5f * Delta;
        }

    }

    Vector3 SolveCollitionConstrains()
    {
        Vector3 mov_tarjet1 = new Vector3(0, 0, 0);
        Vector3 mov_part1 = new Vector3(0,0,0);
        Vector3 mov_tarjet2 = new Vector3(0, 0, 0);
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            if (GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = ParticleA.transform.position - ParticleA.OldPosition;
                Vector3 P_B = distance * Vector3.Normalize(Delta);

                mov_part1 -= P_B / 2;
                mov_tarjet1 += P_B / 2;
            }
            if (GameObject.Find("Target2").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target2").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = GameObject.Find("Target2").transform.position - ParticleA.transform.position;
                Vector3 P_B = distance * Vector3.Normalize(Delta);
                ParticleA.transform.position -= P_B / 2;
                mov_tarjet2 += P_B / 2;
            }
            if (GameObject.Find("Target3").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target3").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = GameObject.Find("Target3").transform.position - ParticleA.transform.position;
                Vector3 P_B = distance * Vector3.Normalize(Delta);
                ParticleA.transform.position -= P_B;
            }
            if (GameObject.Find("Target4").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target4").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = GameObject.Find("Target4").transform.position - ParticleA.transform.position;
                Vector3 P_B = distance * Vector3.Normalize(Delta);
                ParticleA.transform.position -= P_B;
            }
        }
        GameObject.Find("Target").transform.position += mov_tarjet1;
        GameObject.Find("Target2").transform.position += mov_tarjet2;
        return mov_part1;
    }

    public void Apply_Force_Ply1(float Ini_Force)
    {
        float SegmentMaxLenght = CableMaxLenght / (float)NumSegments + 1;


        for (int SegmentIndex = 0; SegmentIndex < NumSegments && Ini_Force > 0; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            ParticleB.new_force = 0;

            Vector3 Delta = ParticleA.position - ParticleB.position;
            float CurrentDistance = Delta.magnitude;

            if (CurrentDistance < SegmentMaxLenght)
            {
                if ((CurrentDistance+Ini_Force) > SegmentMaxLenght)
                {
                    ParticleB.new_force += Ini_Force - CurrentDistance;
                    Ini_Force -= Ini_Force - CurrentDistance;
                }
                else
                {
                    ParticleB.new_force = Ini_Force;
                    Ini_Force = 0;

                }
            }
            else
            {
                //Nothing
            }
        }

        for (int SegmentIndex = NumSegments; SegmentIndex >= 0 ; SegmentIndex--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            if (ParticleA.new_force > 0)
            {
                for (int SegmentIndex2 = SegmentIndex; SegmentIndex2 >= 0; SegmentIndex2--)
                {
                    FParticle_E ParticleB = Particles[SegmentIndex2];
                    ParticleB.new_force += ParticleA.new_force;
                }
            }
        }

        for (int SegmentIndex = NumSegments; SegmentIndex > 0; SegmentIndex--)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex - 1];
            if (ParticleA.new_force > 0)
            {
                Vector3 Delta = ParticleB.position - ParticleA.position;
                ParticleB.position += Delta.normalized * ParticleA.new_force;
            }
            
        }

        update_lineRenderer();

    }
    #endregion
}