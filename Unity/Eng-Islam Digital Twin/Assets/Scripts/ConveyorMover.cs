using UnityEngine;

public class ConveyorMover : MonoBehaviour
{
    [Tooltip("Units per second movement along this GameObject's right vector.")]
    public float speedMetersPerSecond = 1.0f;

    [Tooltip("Layers of objects to move. Leave empty to affect all rigidbodies.")]
    public LayerMask affectedLayers = ~0;

    [Tooltip("Use surface conveyor physics via Rigidbody.MovePosition in FixedUpdate.")]
    public bool usePhysicsMove = true;

    private void OnCollisionStay(Collision collision)
    {
        if (speedMetersPerSecond == 0f) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        if (((1 << rb.gameObject.layer) & affectedLayers) == 0) return;

        // Changed from transform.forward to transform.right for rightward movement
        Vector3 surfaceVelocity = transform.right * speedMetersPerSecond;

        // Project along the contact plane to reduce vertical injection
        ContactPoint contact = collision.GetContact(0);
        Vector3 tangentVelocity = Vector3.ProjectOnPlane(surfaceVelocity, contact.normal);

        if (usePhysicsMove)
        {
            rb.position += tangentVelocity * Time.fixedDeltaTime;
        }
        else
        {
            rb.velocity = new Vector3(tangentVelocity.x, rb.velocity.y, tangentVelocity.z);
        }
    }
}