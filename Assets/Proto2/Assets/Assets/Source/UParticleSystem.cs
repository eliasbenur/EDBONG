using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Using Verlet Integration to solve constrains and velocity
// This iteration is working much better then my first try
// at making a rope with physics. I've tried to do the same
// but with a different approach and it didn't work out.
// This code is all from UE4 CableComponent.
// Converted it in C# to test within Unity, due slow pc at home.
public class UParticleSystem : MonoBehaviour {
	#region public Properties
	public Transform PrefabParticle;
	public Transform CableStart;
	public Transform CableEnd;
	public int CableLength = 10;
	public int NumSegments = 50;
	public int SolverIterations = 1;
	#endregion

	#region private Properties
	private List<FParticle> Particles = new List<FParticle>();
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
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    void Start() {
		int NumParticles = NumSegments + 1;
		Particles.Clear();
		
		// Use linerenderer as visual cable representation
		_lineRenderer = transform.GetComponent<LineRenderer>();
		if (_lineRenderer == null) {
			_lineRenderer = gameObject.AddComponent<LineRenderer>();
		}
		_lineRenderer.SetVertexCount(NumSegments);
		_lineRenderer.SetWidth(.2f, .2f);
		_lineRenderer.SetColors(Color.cyan, Color.blue);

		Vector3 Delta = CableEnd.position - CableStart.position;

		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
			Transform newTransform = Instantiate(PrefabParticle, Vector3.zero, Quaternion.identity) as Transform;

			float Alpha = (float)ParticleIndex / (float)NumParticles;
			Vector3 InitializePosition = CableStart.transform.position + (Alpha * Delta);

			FParticle particle = newTransform.GetComponent<FParticle>();
			particle.position = InitializePosition;
			particle.OldPosition = InitializePosition;
			particle.transform.parent = this.transform;
			particle.name = "Particle_0" + ParticleIndex.ToString();

            Particles.Add(particle);


            if (ParticleIndex == 0 || ParticleIndex == (NumParticles - 1)) {
				particle.bFree = false;
			} else {
				particle.bFree = true;
			}
		}
        //Debug.Log(Particles.Count);
    }

    void Update() {
		// Update start+end positions first

		FParticle StartParticle = Particles[0];
		StartParticle.position = StartParticle.OldPosition = CableStart.position;
		
		FParticle EndParticle = Particles[NumSegments];
		EndParticle.position = EndParticle.OldPosition = CableEnd.position;

		Vector3 Gravity = Physics.gravity;
		float UseSubstep = Mathf.Max(SubstepTime, 0.005f);

		TimeRemainder += Time.deltaTime;
		while (TimeRemainder > UseSubstep) {
			PreformSubstep(UseSubstep, Gravity);
			TimeRemainder -= UseSubstep;
		}
        Vector3 distance = (CableEnd.position - CableStart.position);
        float distanceMeter = (distance.x * distance.x) + (distance.y*distance.y);
        //Debug.Log(distanceMeter);
        GetLengthRope();        
	}
    #endregion

    void GetLengthRope()
    {
        for (int i = NumSegments; i > 0 + 1; i--)
        {
            var distanceCollider = Particles[i].position - Particles[i - 1].position;
            var distance = (distanceCollider.x * distanceCollider.x) + (distanceCollider.y * distanceCollider.y);
            somme += distance;
        }
        //Debug.Log(somme);
        somme = 0;
    }

	#region private Methods
	private void PreformSubstep(float InSubstepTime, Vector3 Gravity) {
		VerletIntegrate(InSubstepTime, Gravity);
		SolveConstraints();
	}

	private void VerletIntegrate(float InSubstepTime, Vector3 Gravity) {
		int NumParticles = NumSegments + 1;
		float SubstepTimeSqr = InSubstepTime * InSubstepTime;
		
		for (int ParticleIndex = 0; ParticleIndex < NumParticles; ParticleIndex++) {
			FParticle particle = Particles[ParticleIndex];
			if (particle.bFree) {
				Vector3 Velocity = particle.position - particle.OldPosition;
				Vector3 NewPosition = particle.position + Velocity + (SubstepTimeSqr * Gravity);
				
				particle.OldPosition = particle.position;
				particle.position = NewPosition;
			}
		}
	}

	private void SolveConstraints() {
		float SegmentLength = CableLength/(float)NumSegments;
		
		// For each iteration
		for (int IterationIndex = 0; IterationIndex < SolverIterations; IterationIndex++) {
			// For each segment
			for (int SegmentIndex = 0; SegmentIndex < NumSegments; SegmentIndex++) {
				FParticle ParticleA = Particles[SegmentIndex];
				FParticle ParticleB = Particles[SegmentIndex + 1];
				// Solve for this pair of particles
				SolveDistanceConstraint(ParticleA, ParticleB, SegmentLength);

				// Update render position
				_lineRenderer.SetPosition(SegmentIndex, ParticleA.position);
			}
		}
	}

	void SolveDistanceConstraint(FParticle ParticleA, FParticle ParticleB, float DesiredDistance) {
		// Find current difference between particles
		Vector3 Delta = ParticleB.position - ParticleA.position;
		float CurrentDistance = Delta.magnitude;
		float ErrorFactor = (CurrentDistance - DesiredDistance) / CurrentDistance;
		
		// Only move free particles to satisfy constraints
		if (ParticleA.bFree && ParticleB.bFree) {
			ParticleA.position += ErrorFactor * 0.5f * Delta;
			ParticleB.position -= ErrorFactor * 0.5f * Delta;
		} else if (ParticleA.bFree) {
			ParticleA.position += ErrorFactor * Delta;
		} else if (ParticleB.bFree) {
			ParticleB.position -= ErrorFactor * Delta;
		}
	}
	#endregion
}