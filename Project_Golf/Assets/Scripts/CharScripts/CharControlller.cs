using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharControlller : MonoBehaviour {

    
    public float Mass;
    public float Speed;
    public float RotSpeed;
    private CharacterController _controller;
    private Animator _animator;

    [System.Serializable]
    public struct GolfStick
    {
        public GameObject stick;
        public enum StickState { Walk, Swing, Car };
        public StickState ss;
        public Transform walkingPos;
        public Transform swingingPos;
        public Transform caringPos;
    }

    public GolfStick stick;

    [SerializeField]
    private Transform Armature;

    private Camera mainCamera;
    private bool inUse;


    private float v;
    private float h;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        _srot = Armature.rotation;
    }

    void Update()
    {
        if (!inUse)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");
            float gravity = -9.81f * Time.deltaTime * Mass;
            if (_controller.isGrounded)
            {
                gravity = 0;
            }
            Vector3 move = this.transform.forward * v + this.transform.right * h;
            _controller.Move(move.normalized * Time.deltaTime * Speed + Vector3.up * gravity);

            _animator.SetInteger("walking", v > 0 ? 1 : v == 0 ? h == 0 ? 0 : 1 : -1);
        }
    }

    private float _timeCount = 0.0f;
    private float _angle = 0.0f;



    private Quaternion _srot;
    void LateUpdate()
    {
        if (!inUse)
        {

            Vector3 dir=_controller.velocity.normalized;
            Debug.DrawRay(_controller.transform.position, dir,Color.red);

            v = Mathf.Abs(v) == 0 ? 0 : v > 0 ? 1 : -1;
            h = Mathf.Abs(h) == 0 ? 0 : h > 0 ? 1 : -1;
            float angle = h == 0 ? 0 : v == 0 ? h * 90 : h * v * 45;
            if(angle != _angle)
            {
                _angle= angle;
                _timeCount = Time.deltaTime;
            }

            Armature.Rotate(Vector3.forward, angle);

            Quaternion grot = Armature.rotation;

            Armature.rotation = Quaternion.Lerp(_srot, grot, _timeCount);

            if(Quaternion.Angle(Armature.rotation, grot)!=0)
                _timeCount += Time.deltaTime*1f;
            _srot = Armature.rotation;



            if (Input.GetKeyDown(KeyCode.E) )
            {
                RaycastHit hit;

                if (Physics.Raycast(Armature.position, mainCamera.transform.forward, out hit))
                {
                    Debug.DrawRay(Armature.position, this.transform.forward);
                    IUsable iu = hit.collider.transform.GetComponent<IUsable>();
                    if (iu != null)
                    {
                        iu.StartUsing(this.gameObject);
                    }
                    else
                    {
                        iu = hit.transform.GetComponent<IUsable>();
                        if (iu != null)
                        {
                            iu.StartUsing(this.gameObject);
                        }
                    }

                }
            }

        }
        else
        {
            _srot= Armature.rotation;
        }
        if (stick.stick != null)
        {
            switch (stick.ss)
            {
                case (GolfStick.StickState.Car):
                    stick.stick.transform.position = stick.caringPos.position;
                    stick.stick.transform.rotation = stick.caringPos.rotation;
                    break;
                case (GolfStick.StickState.Swing):
                    stick.stick.transform.position = stick.swingingPos.position;
                    stick.stick.transform.rotation = stick.swingingPos.rotation;
                    break;
                case (GolfStick.StickState.Walk):
                    stick.stick.transform.position = stick.walkingPos.position;
                    stick.stick.transform.rotation = stick.walkingPos.rotation;
                    break;
            }
        }
    }

    public void rotate(float h)
    {
        if(!inUse)
            this.transform.Rotate(this.transform.up, h * RotSpeed);
    }

    internal float AnimationState(string name)
    {
        return (!_animator.GetCurrentAnimatorStateInfo(0).IsName(name))? 0 : _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    IEnumerator unUse()
    {
        yield return new WaitForFixedUpdate();

        inUse = false;
    }

    /*----------CAR-----------*/

    public void EnterCar(){

        stick.ss = GolfStick.StickState.Car;

        inUse = true;

        _animator.SetInteger("walking", 0);
        _animator.SetBool("Driving", true);

        mainCamera.GetComponent<CameraController>().CamUse = CameraController.CamState.AltUse;

        this.GetComponent<CharacterController>().enabled = false;
    }
    public void ExitCar(){
        stick.ss = GolfStick.StickState.Walk;

        _animator.SetBool("Driving", false);

        mainCamera.GetComponent<CameraController>().CamUse = CameraController.CamState.Normal;
        mainCamera.fieldOfView = 90;

        this.GetComponent<CharacterController>().enabled = true;

        StartCoroutine(unUse());

        this.transform.rotation = Quaternion.Euler( Vector3.up * this.transform.rotation.eulerAngles.y );
    }

    /*----------SWING-----------*/

    public void EnterSwing(Transform camPos)
    {
        stick.ss = GolfStick.StickState.Swing;
        inUse = true;

        _animator.SetInteger("walking", 0);
        _animator.SetBool("swinging", true);

        mainCamera.GetComponent<CameraController>().CamUse = CameraController.CamState.StaticAltUse;

        mainCamera.transform.position = camPos.position;
        mainCamera.transform.rotation = camPos.rotation;
    }
    public void ExitSwing(Vector3 lookAtMe)
    {
        _animator.SetBool("swinging", false);
        stick.ss = GolfStick.StickState.Walk;

        StartCoroutine(GoBackCam(lookAtMe));
    }

    private IEnumerator GoBackCam(Vector3 lookAtMe)
    {

        yield return new WaitForFixedUpdate();

        this.transform.LookAt(new Vector3(lookAtMe.x, this.transform.position.y, lookAtMe.z));

        mainCamera.GetComponent<CameraController>().Reset();

        yield return new WaitForFixedUpdate();

       

        mainCamera.GetComponent<CameraController>().CamUse = CameraController.CamState.Normal;

        StartCoroutine(unUse());
    }

    internal Vector3 GetSwingForce(Vector3 forward, Vector3 up)
    {
        return forward * 15 + up * 10; //TESTING
    }

    public void Swing()
    {
        _animator.SetTrigger("swing");
    }
}
