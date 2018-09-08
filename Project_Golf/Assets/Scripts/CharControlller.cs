using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlller : MonoBehaviour {

    public float Mass;
    public float Speed;
    public float RotSpeed;
    private CharacterController _controller;
    private Animator _animator;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float gravity = -9.81f * Time.deltaTime * Mass;
        if (_controller.isGrounded)
        {
            gravity = 0;
        }
        Debug.Log(_controller.isGrounded);
        Vector3 move = this.transform.forward* v + this.transform.up * gravity;
        _controller.Move(move * Time.deltaTime * Speed);
        this.transform.Rotate(this.transform.up, h * RotSpeed);
        _animator.SetInteger("walking", v>0? 1:v==0? 0: -1);
        
    }
}
