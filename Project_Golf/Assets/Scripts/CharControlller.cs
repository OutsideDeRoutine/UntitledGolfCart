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

    void LateUpdate()
    {
        Vector3 dir=_controller.velocity.normalized;
        Debug.DrawRay(_controller.transform.position, dir,Color.red);
        float t = Mathf.Abs(v) <= 0.1 ? 0 : v > 0 ? 1 : -1;
        float angle = h == 0 ? 0 : t == 0 ? h * 90 : h * t * 45;
        Armature.Rotate(Vector3.forward * Time.deltaTime, angle);
    }

    public void rotate(float h)
    {
        this.transform.Rotate(this.transform.up, h * RotSpeed);
    }
}
