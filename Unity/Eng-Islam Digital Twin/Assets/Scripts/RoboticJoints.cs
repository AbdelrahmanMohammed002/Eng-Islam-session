using UnityEngine;

[System.Serializable]
public class RoboticJoint
{
    public string name; // Just for clarity in inspector
    public Transform joint; // The part of the robot that moves
    public Transform pivotPoint; // Empty GameObject at the rotation pivot
    public Vector3 axis = Vector3.up; // Axis of rotation (world axis)
    public float minAngle = -90f; // Lower limit
    public float maxAngle = 90f;  // Upper limit
    public float speed = 45f; // Degrees per second

    [HideInInspector] public float currentAngle = 0f; // Tracks rotation
}
