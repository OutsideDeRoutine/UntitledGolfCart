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

        Debug.DrawRay(CoM.position, velocity, Color.red);
        Debug.DrawRay(CoM.position, CoM.forward);

        float v = Input.GetAxis("Vertical");
        float motoraceleration = maxMotorTorque * v * aceleration;
        float motorbreak = maxMotorTorque * Mathf.Abs(v) * brake;


        float vel = velocity.magnitude;
        float maxSteeringAtVel = Mathf.Clamp(maxSteeringAngle - (vel), 10, maxSteeringAngle);

        float h = Input.GetAxis("Horizontal") / 4;

        float fit = h == 0 ? (steering == 0 ? 0 : (steering > 0 ? -vel : vel) / 2) : h * 4;
        if (fit != h * 4)
            steering += steering > 0 ? (steering + fit < 0 ? -steering : fit) : (steering + fit > 0 ? -steering : fit);
        else steering += h * 4;

        steering = Mathf.Clamp(steering, -maxSteeringAtVel, maxSteeringAtVel);

        UpdateCarState(CoM.position, CoM.forward , velocity);

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }


            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motoraceleration;
                axleInfo.rightWheel.motorTorque = motoraceleration;
                if (axleInfo.rightWheel.motorTorque>0)
                switch (state)
                {
                    case (CarState.Stopped):

                        break;

                    case (CarState.Forwards):
                            axleInfo.leftWheel.motorTorque -= 100;
                            axleInfo.rightWheel.motorTorque -= 100;
                        break;

                    case (CarState.Backwards):
                            axleInfo.leftWheel.motorTorque += 150;
                            axleInfo.rightWheel.motorTorque += 150;
                        break;
                }
                
            }
            else
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;
                switch (state)
                {
                    case (CarState.Stopped):

                        break;

                    case (CarState.Forwards):
                        if (v < 0)
                        {
                            axleInfo.leftWheel.brakeTorque = motorbreak;
                            axleInfo.rightWheel.brakeTorque = motorbreak;
                        }
                        break;

                    case (CarState.Backwards):
                        if (v > 0)
                        {
                            axleInfo.leftWheel.brakeTorque = motorbreak;
                            axleInfo.rightWheel.brakeTorque = motorbreak;
                        }
                        break;
                }
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        ApplySteeringWheelRot(steering);
    }


    private void UpdateCarState(Vector3 position,Vector3 direction, Vector3 velocity)
    {
        
        if (Mathf.Abs( Vector3.Distance(Vector3.zero, velocity))< 0.01f)
            state = CarState.Stopped;
        else
        {
            float velAngle = Vector3.Angle(position, velocity);
            float dirAngle = Vector3.Angle(position, direction);
            Debug.Log(velAngle - dirAngle);
            if (Mathf.Abs(velAngle- dirAngle)> maxSteeringAngle) // <---- PROBLEMAS
            {
                state = CarState.Backwards;
            }
            else
            {
                state = CarState.Forwards;
            }
        }
    }
}
