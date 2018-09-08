using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlller : MonoBehaviour {

    public float Speed;
    private CharacterController _controller;
    private Animator _animator;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = this.transform.forward* Input.GetAxis("Vertical") + this.transform.right * Input.GetAxis("Horizontal");
        _controller.Move(move.normalized * Time.deltaTime * Speed);
        if (move.magnitude > 0 && !_animator.GetBool("walking"))
        {
            _animator.SetBool("walking", true);
        }
        else if(move.magnitude == 0 && _animator.GetBool("walking"))
        {
            _animator.SetBool("walking", false);
        }
    }
}
