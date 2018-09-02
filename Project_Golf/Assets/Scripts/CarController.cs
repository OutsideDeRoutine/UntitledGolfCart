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

    public List<AxleInfo> axleInfos;
    public Transform steeringWheel;
    public float maxMotorTorque;
    public float maxSteeringAngle;


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
        float motor = maxMotorTorque * Input.GetAxis("Vertical");

        float vel = GetComponent<Rigidbody>().velocity.magnitude;

        float maxSteeringAtVel = Mathf.Clamp(maxSteeringAngle - (vel),10, maxSteeringAngle);

        float h = Input.GetAxis("Horizontal")/2;

        float fit = h ==0? (steering==0? 0:(steering>0? -vel : vel) / 20) : h * 4;
        if (fit != h * 4)
            steering += steering > 0 ? (steering + fit < 0 ? 0 : fit) : (steering + fit > 0 ? 0 : fit);
        else steering += h * 4;

        steering = Mathf.Clamp(steering, -maxSteeringAtVel, maxSteeringAtVel);

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        ApplySteeringWheelRot(steering);
    }
}
