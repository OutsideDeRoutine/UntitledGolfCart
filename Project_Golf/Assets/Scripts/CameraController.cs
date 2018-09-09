using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float velocity = 1;
    public Transform LookAtMe;

    private bool inUse;

    void Update()
    {
        MouseMove();
    }

    private void MouseMove()
    {
        var h = Input.GetAxis("Mouse X");

        var v = Input.GetAxis("Mouse Y");
        transform.RotateAround(LookAtMe.position, transform.right, -v * mouseSensitivity);
        
        this.GetComponentInParent<CharControlller>().rotate(h);
    }

}
