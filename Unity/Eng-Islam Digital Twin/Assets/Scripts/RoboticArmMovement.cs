using System.Drawing;
using UnityEngine;

[System.Serializable]
public class RoboticJointController : MonoBehaviour
{
    public RoboticJoint[] joints; // Array of joints to control

    private int selectedJoint = 0; // Which joint are we controlling right now?

    void Update()
    {
        if (joints.Length == 0) return;

        RoboticJoint joint = joints[selectedJoint];
        float step = joint.speed * Time.deltaTime;

        // Switch joint with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedJoint = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && joints.Length > 1) selectedJoint = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && joints.Length > 2) selectedJoint = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) && joints.Length > 3) selectedJoint = 3; 
        if (Input.GetKeyDown(KeyCode.Alpha5) && joints.Length > 4) selectedJoint = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6) && joints.Length > 5) selectedJoint = 5;
        if (Input.GetKeyDown(KeyCode.Alpha7) && joints.Length > 6) selectedJoint = 6;

        // Rotate left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float allowedStep = Mathf.Min(step, joint.currentAngle - joint.minAngle);
            if (allowedStep > 0f)
            {
                joint.pivotPoint.Rotate(joint.axis, -allowedStep, Space.Self);
                joint.currentAngle -= allowedStep;
            }
        }

        // Rotate right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float allowedStep = Mathf.Min(step, joint.maxAngle - joint.currentAngle);
            if (allowedStep > 0f)
            {
                joint.pivotPoint.Rotate(joint.axis, allowedStep, Space.Self);
                joint.currentAngle += allowedStep;
            }
        }

        // Clamp currentAngle to ensure it stays within limits
        joint.currentAngle = Mathf.Clamp(joint.currentAngle, joint.minAngle, joint.maxAngle);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Selected Joint: " + joints[selectedJoint].name);
        GUI.Label(new Rect(10, 30, 300, 20), "Angle: " + joints[selectedJoint].currentAngle.ToString("F1"));
    }
}