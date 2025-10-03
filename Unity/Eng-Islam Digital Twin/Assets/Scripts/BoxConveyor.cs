using UnityEngine;
using realvirtual;   // realvirtual.io namespace

public class SensorConveyorController : MonoBehaviour
{
    [Header("Realvirtual.io Components")]
    public Sensor sensor;              // Assign your sensor in the Inspector
    public Drive conveyorDrive;        // Assign the conveyor's Drive script here
    public PLCInputBool PowerButtonRobot1;    // Optional start button
    public PLCInputBool PowerButtonRobot2;    // Optional start button
    public PLCInputBool PowerButtonRobot3;    // Optional start button
    public Animator robotAnimator; // Assign in Inspector


    [Header("Settings")]
    public int targetCount = 5;        // Trigger after N items
    public float moveDistance = 2f;    // Distance to move each time
    public float moveSpeed = 0.5f;     // Conveyor speed during move
    private int lastTriggerCount = 0;  // Internal tracker
    private bool isMoving = false;     // Track movement state

    void Update()
    {
        if (sensor == null || conveyorDrive == null) return;

        // Check if enough new objects passed and conveyor is not already moving
        if (!isMoving && sensor.Counter - lastTriggerCount >= targetCount)
        {
            lastTriggerCount = sensor.Counter;
            Debug.Log($"Sensor triggered {sensor.Counter} times. Moving conveyor.");

            // Start moving conveyor as a coroutine
            StartCoroutine(MoveConveyor());
        }
    }

    private System.Collections.IEnumerator MoveConveyor()
    {
        isMoving = true;

        // Set target motion
        float target = conveyorDrive.TargetPosition + moveDistance;
        conveyorDrive.TargetSpeed = moveSpeed;
        conveyorDrive.JogForward = true;

        // Wait until conveyor reaches target
        while (conveyorDrive.CurrentPosition < target)
        {
            yield return null; // wait one frame
        }

        // Stop conveyor
        conveyorDrive.JogForward = false;
        conveyorDrive.TargetPosition = target;

        TriggerRobotMotion();

        Debug.Log($"Conveyor reached {target} at speed {moveSpeed}");
        isMoving = false;
    }


    void TriggerRobotMotion()
    {
        if (robotAnimator != null)
        {
            robotAnimator.SetTrigger("BoxFullTrigger");
            Debug.Log("Robot animation triggered!");
        }
    }

}
