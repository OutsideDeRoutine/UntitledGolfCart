using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] float mouseSensitivity = 1;
    public Transform LookAtMe;

    public enum CamState { Normal, AltUse, StaticAltUse };
    public CamState CamUse;

    private Quaternion StartRot;
    private Vector3 StartPos;

    private void Start()
    {
        StartRot = transform.localRotation;
        StartPos = transform.localPosition;
    }

    void Update()
    {
        MouseMove();
    }

    private void MouseMove()
    {

        var h = Input.GetAxis("Mouse X");

        var v = Input.GetAxis("Mouse Y");

        switch (CamUse)
        {
            case (CamState.Normal):
                transform.RotateAround(LookAtMe.position, transform.right, -v * mouseSensitivity);

                this.GetComponentInParent<CharControlller>().rotate(h);
                if (transform.localRotation.y != 0)
                    transform.RotateAround(LookAtMe.position, Vector3.up, -transform.localRotation.y * 10);

                break;
            case (CamState.AltUse):
                transform.RotateAround(LookAtMe.position, transform.right, -v * mouseSensitivity);

                transform.RotateAround(LookAtMe.position, LookAtMe.up, h * mouseSensitivity);

                break;
            case (CamState.StaticAltUse):

                break;
        }
    }

    public void Reset()
    {
        transform.localRotation = StartRot;
        transform.localPosition= StartPos;
    }
}
