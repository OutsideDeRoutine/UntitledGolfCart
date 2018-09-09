using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlller : MonoBehaviour {

    public float Mass;
    public float Speed;
    public float RotSpeed;
    private CharacterController _controller;
    private Animator _animator;

    [SerializeField]
    private Transform Armature;


    private float v;
    private float h;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();


        _srot = Armature.rotation;
    }

    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        float gravity = -9.81f * Time.deltaTime * Mass;
        if (_controller.isGrounded)
        {
            gravity = 0;
        }
        Vector3 move = this.transform.forward* v + this.transform.right *h;
        _controller.Move(move.normalized * Time.deltaTime * Speed + Vector3.up * gravity);

        _animator.SetInteger("walking", v>0? 1:v==0? h==0? 0: 1: -1);        
    }


    private float _timeCount = 0.0f;
    private float _angle = 0.0f;
    private Quaternion _srot;
    void LateUpdate()
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
    }

    public void rotate(float h)
    {
        this.transform.Rotate(this.transform.up, h * RotSpeed);
    }
}
