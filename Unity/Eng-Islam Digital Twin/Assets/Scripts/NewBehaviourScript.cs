using UnityEngine;

public class TriggerTransport : MonoBehaviour
{
    [Tooltip("Where the object will be moved to")]
    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        // Always get the top-most GameObject (the prefab root)
        Transform rootObject = other.transform.root;

        // Move root GameObject instead of just the child collider
        rootObject.position = destination.position;
        rootObject.rotation = destination.rotation;

        // Reset physics velocities if Rigidbody exists
        Rigidbody rb = rootObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"Transported {rootObject.name} to {destination.position}");
    }
}
