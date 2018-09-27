using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour {

    public enum CarState { Stopped, Forwards, Backwards };
    public Transform CoM;
    public List<AxleInfo> axleInfos;
    public Transform steeringWheel;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float aceleration;
    public float brake;

    public CarState state;

    public bool HandBrake;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CoM.localPosition;
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private float actSWRot;
    public void ApplySteeringWheelRot(float rotation)
    {
        float rotateNow= rotation - actSWRot;

        steeringWheel.transform.RotateAround(steeringWheel.position, steeringWheel.forward, rotateNow*3);

        actSWRot = rotation;
    }

    private float steering;


    public void FixedUpdate()
    {

        
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        //Debug.Log(velocity.x + " - " + velocity.y + " - " + velocity.z);

        Debug.DrawRay(CoM.position, velocity, Color.red);
        Debug.DrawRay(CoM.position, CoM.forward);

        float v = Input.GetAxis("Vertical");
        float motoraceleration = maxMotorTorque * v * aceleration;
        float motorbreak = maxMotorTorque * Mathf.Abs(v) * brake;


        float vel = velocity.magnitude;
        Debug.Log("Magnitud de la velocidad: "+vel);
        float maxSteeringAtVel = Mathf.Clamp(maxSteeringAngle - (vel), 10, maxSteeringAngle);

        if(!HandBrake) Camera.main.fieldOfView = 90 + vel;

        float h = Input.GetAxis("Horizontal") / 6;
        steering = h* maxSteeringAngle;

        steering = Mathf.Clamp(steering, -maxSteeringAtVel, maxSteeringAtVel);

        UpdateCarState(CoM.localPosition, CoM.forward , velocity);

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (!HandBrake)
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;

                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }

                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    switch (state)
                    {
                        case (CarState.Stopped):
                            axleInfo.leftWheel.motorTorque = motoraceleration;
                            axleInfo.rightWheel.motorTorque = motoraceleration;
                            break;

                        case (CarState.Forwards):
                            if (v >= 0)
                            {
                                axleInfo.leftWheel.motorTorque = motoraceleration;
                                axleInfo.rightWheel.motorTorque = motoraceleration;
                            }
                            else
                            {
                                axleInfo.leftWheel.brakeTorque = motorbreak;
                                axleInfo.rightWheel.brakeTorque = motorbreak;
                            }
                            break;

                        case (CarState.Backwards):
                            if (v <= 0)
                            {
                                axleInfo.leftWheel.motorTorque = motoraceleration;
                                axleInfo.rightWheel.motorTorque = motoraceleration;
                            }
                            else
                            {
                                axleInfo.leftWheel.brakeTorque = motorbreak;
                                axleInfo.rightWheel.brakeTorque = motorbreak;
                            }
                            break;
                    }
                }
                else
                {
                    if (Mathf.Abs(vel) > 0)
                        switch (state)
                        {
                            case (CarState.Stopped):
                                axleInfo.leftWheel.motorTorque = 0;
                                axleInfo.rightWheel.motorTorque = 0;
                                break;

                            case (CarState.Forwards):
                                axleInfo.leftWheel.motorTorque = -100;
                                axleInfo.rightWheel.motorTorque = -100;
                                if (v < 0)
                                {
                                    axleInfo.leftWheel.motorTorque = -motorbreak;
                                    axleInfo.rightWheel.motorTorque = -motorbreak;
                                }
                                break;

                            case (CarState.Backwards):

                                axleInfo.leftWheel.motorTorque = 150;
                                axleInfo.rightWheel.motorTorque = 150;
                                if (v > 0)
                                {
                                    axleInfo.leftWheel.motorTorque = motorbreak;
                                    axleInfo.rightWheel.motorTorque = motorbreak;
                                }
                                break;
                        }
                    else
                    {
                        axleInfo.leftWheel.motorTorque = 0;
                        axleInfo.rightWheel.motorTorque = 0;
                    }
                }
            }
            else
            {
                axleInfo.leftWheel.brakeTorque = 1000;
                axleInfo.rightWheel.brakeTorque = 1000;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
        if (!HandBrake)  ApplySteeringWheelRot(steering);

        if (vel > 10)
        {
            GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(velocity, 10);
        }
    }


    private void UpdateCarState(Vector3 position,Vector3 direction, Vector3 velocity)
    {
        
        if (Mathf.Abs( Vector3.Distance(Vector3.zero, velocity))< 0.5f)
            state = CarState.Stopped;
        else
        {
            float a = angle(position, direction, velocity);
            if (Mathf.Abs(a)> 90)
            {
                state = CarState.Backwards;
            }
            else
            {
                state = CarState.Forwards;
            }
        }
    }
    private float angle(Vector3 a, Vector3 b, Vector3 c)
    {
        float C = Vector3.Distance(a, b);
        float B = Vector3.Distance(a, c);
        float A = Vector3.Distance(b, c);
        return Mathf.Acos(  (Mathf.Pow(B, 2)    + Mathf.Pow(C, 2)   - Mathf.Pow(A, 2))   /   (2* B * C))*100;
    }
}
