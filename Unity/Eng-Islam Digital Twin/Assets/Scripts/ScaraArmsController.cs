using realvirtual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaraArmsController : MonoBehaviour
{

    public Sensor sensor1;              // Assign your sensor in the Inspector
    public Sensor sensor2;              // Assign your sensor in the Inspector
    public Sensor sensor3;              // Assign your sensor in the Inspector
    public PLCInputBool PowerButtonRobot1;    // Optional start button
    public PLCInputBool PowerButtonRobot2;    // Optional start button
    public PLCInputBool PowerButtonRobot3;    // Optional start button

    // Update is called once per frame
    void Update()
    {
        if (sensor1.Occupied == true)
        {
            PowerButtonRobot1.Status.ValueOverride = true; // Simulate start button pressed
        }
        if (sensor2.Occupied == true)
        {
            PowerButtonRobot2.Status.ValueOverride = true; // Simulate start button pressed
        }
        if (sensor3.Occupied == true)
        {
            PowerButtonRobot3.Status.ValueOverride = true; // Simulate start button pressed
        }
    }
}
