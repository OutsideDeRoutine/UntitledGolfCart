using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float velocity = 1;
    public Transform LookAtMe;

    public bool altUse;

    void Update()
    {
        MouseMove();
    }

    private void MouseMove()
    {

        var h = Input.GetAxis("Mouse X");

        var v = Input.GetAxis("Mouse Y");
        transform.RotateAround(LookAtMe.position, transform.right, -v * mouseSensitivity);
        
        if (!altUse)
        {
            this.GetComponentInParent<CharControlller>().rotate(h);
            if(transform.localRotation.y!=0)
                transform.RotateAround(LookAtMe.position, Vector3.up,  -transform.localRotation.y*10);
        }
        else transform.RotateAround(LookAtMe.position, LookAtMe.up, h * mouseSensitivity);
    }   


}
