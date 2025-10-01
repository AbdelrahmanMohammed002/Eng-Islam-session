using UnityEngine;

public class PickAndPlaceController : MonoBehaviour
{
	[Header("Assignments")]
	public Transform targetObject; // The object to pick (e.g., Can)
	public Transform placeTransform; // Where to place the object
	[Tooltip("IK target or direct gripper transform to move.")]
	public Transform effectorTransform;

	[Header("Animator IK (optional)")]
	public bool useAnimatorIK = false;
	public Animator animator; // Optional if using IK
	public AvatarIKGoal ikGoal = AvatarIKGoal.RightHand;
	[Range(0f, 1f)] public float ikWeight = 1f;

	[Header("Motion Settings")]
	[Tooltip("Meters per second for linear motion.")]
	public float moveSpeed = 1.0f;
	[Tooltip("Degrees per second for rotation.")]
	public float rotateSpeed = 180f;
	[Tooltip("Distance at which pick/place is considered reached.")]
	public float reachThreshold = 0.03f;

	[Header("Pick Settings")]
	[Tooltip("Parent the picked object under effector and disable physics while carried.")]
	public bool parentOnPick = true;
	public bool disableGravityOnPick = true;
	public bool setKinematicOnPick = true;

	private enum Phase { MoveToPick, Picking, MoveToPlace, Placing, Idle }
	private Phase currentPhase = Phase.Idle;

	private Rigidbody carriedBody;
	private Transform originalParent;
	private bool hasPicked;

	private void Start()
	{
		if (effectorTransform == null)
		{
			Debug.LogWarning("PickAndPlaceController: Effector/IK target is not assigned.");
		}
		if (targetObject != null)
		{
			currentPhase = Phase.MoveToPick;
		}
	}

	private void Update()
	{
		if (effectorTransform == null) return;

		switch (currentPhase)
		{
			case Phase.MoveToPick:
				MoveEffectorTowards(targetObject);
				if (IsAtTarget(targetObject))
				{
					currentPhase = Phase.Picking;
				}
				break;

			case Phase.Picking:
				TryPickTarget();
				if (hasPicked && placeTransform != null)
				{
					currentPhase = Phase.MoveToPlace;
				}
				break;

			case Phase.MoveToPlace:
				MoveEffectorTowards(placeTransform);
				if (IsAtTarget(placeTransform))
				{
					currentPhase = Phase.Placing;
				}
				break;

			case Phase.Placing:
				PlaceObject();
				currentPhase = Phase.Idle;
				break;

			case Phase.Idle:
				// Do nothing. Await external reset or assignments.
				break;
		}
	}

	private void MoveEffectorTowards(Transform destination)
	{
		if (destination == null) return;

		Vector3 targetPos = destination.position;
		Quaternion targetRot = destination.rotation;

		float step = moveSpeed * Time.deltaTime;
		effectorTransform.position = Vector3.MoveTowards(effectorTransform.position, targetPos, step);

		float rotStep = rotateSpeed * Time.deltaTime;
		effectorTransform.rotation = Quaternion.RotateTowards(effectorTransform.rotation, targetRot, rotStep);
	}

	private bool IsAtTarget(Transform destination)
	{
		if (destination == null) return false;
		return Vector3.Distance(effectorTransform.position, destination.position) <= reachThreshold;
	}

	private void TryPickTarget()
	{
		if (targetObject == null) return;

		// If the target has moved (e.g., end of conveyor), we still snap when within reachThreshold
		if (!IsAtTarget(targetObject))
		{
			// Keep chasing the moving target until in range
			MoveEffectorTowards(targetObject);
			return;
		}

		// Attach and disable physics while carried
		carriedBody = targetObject.GetComponent<Rigidbody>();
		if (carriedBody != null)
		{
			if (disableGravityOnPick) carriedBody.useGravity = false;
			if (setKinematicOnPick) carriedBody.isKinematic = true;
			carriedBody.velocity = Vector3.zero;
			carriedBody.angularVelocity = Vector3.zero;
		}

		originalParent = targetObject.parent;
		if (parentOnPick)
		{
			targetObject.SetParent(effectorTransform, worldPositionStays: true);
		}

		hasPicked = true;
	}

	private void PlaceObject()
	{
		if (!hasPicked || targetObject == null) return;

		// Release
		if (parentOnPick)
		{
			targetObject.SetParent(originalParent, worldPositionStays: true);
		}

		if (carriedBody != null)
		{
			if (disableGravityOnPick) carriedBody.useGravity = true;
			if (setKinematicOnPick) carriedBody.isKinematic = false;
			carriedBody = null;
		}

		hasPicked = false;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (!useAnimatorIK || animator == null || effectorTransform == null) return;

		animator.SetIKPositionWeight(ikGoal, ikWeight);
		animator.SetIKRotationWeight(ikGoal, ikWeight);
		animator.SetIKPosition(ikGoal, effectorTransform.position);
		animator.SetIKRotation(ikGoal, effectorTransform.rotation);
	}

	// Public controls
	public void StartSequence()
	{
		if (effectorTransform == null || targetObject == null || placeTransform == null)
		{
			Debug.LogWarning("PickAndPlaceController: Assign effector, targetObject, and placeTransform before starting.");
			return;
		}
		currentPhase = Phase.MoveToPick;
	}

	public void StopSequence()
	{
		currentPhase = Phase.Idle;
	}
}
