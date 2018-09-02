using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour {

    [SerializeField] float mouseSensitivity=1;
    [SerializeField] float velocity = 1;

    private bool inUse;

    void Update () {
        MouseMove();
        WasdMove();
    }

    private void MouseMove()
    {
        var h = Input.GetAxis("Mouse X");
        var v = Input.GetAxis("Mouse Y");
        transform.RotateAround(transform.position, transform.right, -v * mouseSensitivity);
        transform.RotateAround(transform.position, Vector3.up, h * mouseSensitivity);
    }

    private void WasdMove()
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * 0.1f * velocity, Space.Self);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * 0.1f * velocity, Space.Self);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * 0.1f * velocity, Space.Self);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * 0.1f * velocity, Space.Self);
    }
}
