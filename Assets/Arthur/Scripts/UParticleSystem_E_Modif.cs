using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Using Verlet Integration to solve constrains and velocity
// This iteration is working much better then my first try
// at making a rope with physics. I've tried to do the same
// but with a different approach and it didn't work out.
// This code is all from UE4 CableComponent.
// Converted it in C# to test within Unity, due slow pc at home.
public class UParticleSystem_E_Modif : MonoBehaviour {
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

    //For Tweaking value with powerUP
    public Vector3 gravity = Physics.gravity;
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
		_lineRenderer.SetWidth(.5f, .5f);
		_lineRenderer.SetColors(Color.cyan, Color.blue);

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
        FParticle_E StartParticle = Particles[0];
		StartParticle.position = StartParticle.OldPosition = CableStart.position;
        FParticle_E EndParticle = Particles[NumSegments];
		EndParticle.position = EndParticle.OldPosition = CableEnd.position;

		Vector3 Gravity = gravity;
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
		VerletIntegrate(InSubstepTime, Gravity);
		SolveConstraints();
	}

    private void VerletIntegrate(float InSubstepTime, Vector3 Gravity) {
		int NumParticles = NumSegments + 1;
		float SubstepTimeSqr = InSubstepTime * InSubstepTime; // ???
		
		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
            FParticle_E particle = Particles[ParticleIndex];
			if (particle.bFree) {
				Vector3 Velocity = particle.position - particle.OldPosition;
				Vector3 NewPosition = particle.position + Velocity /*+ (SubstepTimeSqr * Gravity)*/; //  SubstepTimeSqr to reduce the gravity effect

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
                SolveDistanceConstraint(ParticleA, ParticleB, SegmentLength);

            }
            

        }

        // For each iteration
        for (int IterationIndex = 0; IterationIndex < SolverIterations_DMAX; IterationIndex++)
        {
            SolveDistanceMax_Old(SegmentMaxLenght);
            SolveCollitionConstrains();
        }

        // Update render position
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            _lineRenderer.SetPosition(SegmentIndex, Particles[SegmentIndex].position);

            var targetPos = Particles[SegmentIndex + 1].position;
            var thisPos = Particles[SegmentIndex].position;
            targetPos.x = targetPos.x - thisPos.x;
            targetPos.y = targetPos.y - thisPos.y;
            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            Particles[SegmentIndex].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //Particles[SegmentIndex].GetComponent<CapsuleCollider2D>().size = new Vector2(Vector2.Distance(targetPos, thisPos)/20, 0.1f);
        }
        _lineRenderer.SetPosition(NumSegments, Particles[NumSegments].position);
    }

    void SolveCollitionConstrains_Old()
    {
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            if (GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                //ParticleA.position = GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.ClosestPoint(ParticleA.position);
                Vector3 V_up = ParticleA.position;
                Vector3 V_down = ParticleA.position;
                Vector3 V_left = ParticleA.position;
                Vector3 V_right = ParticleA.position;
                Vector3 V_up_left = ParticleA.position;
                Vector3 V_up_right = ParticleA.position;
                Vector3 V_down_left = ParticleA.position;
                Vector3 V_down_right = ParticleA.position;
                bool point_found = false;
                Vector3 new_pos = new Vector3(0,0,0);
                while (!point_found)
                {
                    V_up += new Vector3(0,0.01f,0);
                    V_down += new Vector3(0, -0.01f, 0);
                    V_left += new Vector3(-0.01f, 0, 0);
                    V_right += new Vector3(0.01f, 0, 0);
                    V_up_left += new Vector3(-0.01f, 0.01f, 0);
                    V_up_right += new Vector3(0.01f, 0.01f, 0);
                    V_down_left += new Vector3(-0.01f, -0.01f, 0);
                    V_down_right += new Vector3(0.01f, -0.01f, 0);
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_up_left.x, V_up_left.y)))
                    {
                        point_found = true;
                        new_pos = V_up_left;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_up_right.x, V_up_right.y)))
                    {
                        point_found = true;
                        new_pos = V_up_right;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_down_left.x, V_down_left.y)))
                    {
                        point_found = true;
                        new_pos = V_down_left;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_down_right.x, V_down_right.y)))
                    {
                        point_found = true;
                        new_pos = V_down_right;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_up.x, V_up.y)))
                    {
                        point_found = true;
                        new_pos = V_up;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_down.x, V_down.y)))
                    {
                        point_found = true;
                        new_pos = V_down;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_left.x, V_left.y)))
                    {
                        point_found = true;
                        new_pos = V_left;
                    }
                    if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(V_right.x, V_right.y)))
                    {
                        point_found = true;
                        new_pos = V_right;
                    }
                }
                ParticleA.position = new_pos;
            }
        }
    }

    void SolveCollitionConstrains()
    {
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            if (GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = GameObject.Find("Target").transform.position - ParticleA.transform.position;
                float CurrentDistance = Delta.magnitude;
                Vector3 P_B = distance * Vector3.Normalize(Delta);
                ParticleA.transform.position -= P_B / 2;
                GameObject.Find("Target").transform.position += P_B / 2;
            }
            if (GameObject.Find("Target2").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleA.position.x, ParticleA.position.y)))
            {
                float distance = Mathf.Abs(ParticleA.GetComponent<CircleCollider2D>().Distance(GameObject.Find("Target2").GetComponent<CircleCollider2D>()).distance);

                Vector3 Delta = GameObject.Find("Target2").transform.position - ParticleA.transform.position;
                float CurrentDistance = Delta.magnitude;
                Vector3 P_B = distance * Vector3.Normalize(Delta);
                ParticleA.transform.position -= P_B / 2;
                GameObject.Find("Target2").transform.position += P_B / 2;
            }
        }
    }

    /*Not used*/
    void SolveDistanceMax_Old(float SegmentMaxLenght)
    {
        for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++)
        {
            FParticle_E ParticleA = Particles[SegmentIndex];
            FParticle_E ParticleB = Particles[SegmentIndex + 1];
            // Solve for this pair of particles
            SolveDistanceMax_Particles_Old(ParticleA, ParticleB, SegmentMaxLenght);
        }
        CableEnd.position = Particles[NumSegments].position;
        CableStart.position = Particles[0].position;
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
            //if (!GameObject.Find("Target").GetComponent<CircleCollider2D>().bounds.Contains(new Vector2(ParticleB.position.x + P_B.x, ParticleB.position.y + P_B.y)))
            //{
                if (ParticleA.bFree && ParticleB.bFree)
                {
                    //ParticleB.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(P_B.x, P_B.y);
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
                    //ParticleB.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(P_B.x, P_B.y);
                    ParticleB.position += P_B;
                    ParticleB.OldPosition = ParticleB.position;
                }
            //}

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
            if (ParticleA.bFree && ParticleB.bFree)
            {
                ParticleB.position += P_B/2;
                ParticleA.position -= P_B/2;
            }
            else if (ParticleA.bFree)
            {
                //ParticleA.position -= P_B;
                ParticleB.position += P_B / 2;
                ParticleA.position -= P_B / 2;

            }
            else if (ParticleB.bFree)
            {
                //ParticleB.position += P_B;
                ParticleB.position += P_B / 2;
                ParticleA.position -= P_B / 2;
            }

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
	#endregion
}