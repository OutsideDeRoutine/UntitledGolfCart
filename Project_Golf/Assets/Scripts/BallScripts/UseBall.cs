﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBall : AbstractUsable {

    public GameObject Ball;
    public Transform CharPos;
    public Transform CamPos;

    public GameObject UIThrow;

    public bool InUse;

    [System.Serializable]
    public class TW{
        [Range(10.0f, 100.0f)]
       public float fc= 100.0f;
        [Range(10.0f, 100.0f)]
        public float ht = 100.0f;
        [Range(-1.0f, 1.0f)]
        public float ac;
        [Range(-1.0f, 1.0f)]
        public float ef;
    }

    public TW tw;

    void Update() {
        if (isUsing && !InUse)
        {
            float h = Input.GetAxis("Horizontal");

            this.transform.Rotate(this.transform.up, h );

            user.transform.position = CharPos.position;
            user.transform.rotation = CharPos.rotation;

            if (Input.GetKeyDown(KeyCode.E))
            {
                EndUsing();
            }
        }
    }

    public void StartSwing()
    {
        UIThrow.GetComponent<ThrowController>().Sleep();
        cc.Swing();
        if (cc.stick.stick != null) StartCoroutine(Swing());
        else UIThrow.GetComponent<ThrowController>().WakeUp();
    }

    public IEnumerator Swing()
    {
        UIThrow.GetComponent<ThrowController>().enabled = false;
        InUse = true;

        //ESPERA A MOMENTO DEL GOLPE
        yield return new WaitUntil(() => cc.AnimationState("Swing") >0.55);

       

        //  1. CAMBIAR LA BOLA ESTATICA POR BOLA FISICA.
        Ball.GetComponent<SphereCollider>().enabled = true;
        Ball.GetComponent<Rigidbody>().isKinematic = false;

        //  2. HACER CALCULOS -> LANZAR BOLA 
        Vector3 force = cc.GetSwingForce(transform.forward * tw.fc / 100, transform.up * tw.ht / 100);

        Ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);

        yield return new WaitUntil(() => cc.AnimationState("Swing") >= 0.6);


        //  3. CAMARA SEGUIR BOLA HASTA QUE ATERRIZA Y SE PARA
        cc.text.text = "[Space] Speed Time";
        yield return new WaitUntil(() => WhileBallMoving());

        Ball.GetComponent<SphereCollider>().enabled = false;
        Ball.GetComponent<Rigidbody>().isKinematic = true;
 
        
        EndUsing();

        //  4. MOVER TODO A LA POSICION DE LA BOLA.
        Vector3 pos = Ball.transform.localPosition;

        this.transform.Translate(pos);

        Ball.transform.localPosition = Vector3.zero;

        InUse = false;
    }

    public float smoothCamPos = 20;
    public float smoothCamRot = 25;
    private Vector3 velocity = Vector3.zero;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    public bool WhileBallMoving()
    {
        Vector3 v = Ball.GetComponent<Rigidbody>().velocity;

        float vel = v.magnitude;


        if(tw.ac != 0)                                                                              // PRECISION
        {
            Vector3 f = transform.right * tw.ac * (vel / 200);

            Ball.GetComponent<Rigidbody>().AddForce(f * Time.deltaTime * 50, ForceMode.VelocityChange);  
        }
        

        if (tw.ef!=0  &&  isGrounded(Ball.transform.position, 0.05f))                               // EFECTO
        {
            if(tw.ef<0)
                Ball.GetComponent<Rigidbody>().AddForce(((v / 10) * tw.ef) * Time.deltaTime, ForceMode.Impulse);
            else
                Ball.GetComponent<Rigidbody>().AddForce(((v / 3) * tw.ef) * Time.deltaTime, ForceMode.Force);
        }


        cam.position = Vector3.SmoothDamp(Camera.main.transform.position , Ball.transform.position - this.transform.forward / 1.7f + Vector3.up/10, ref velocity, smoothCamPos * Time.deltaTime);
        Quaternion rot = cam.rotation;
        cam.LookAt(Ball.transform.position - v / 20);
        cam.rotation = Quaternion.RotateTowards(rot, cam.transform.rotation ,Time.deltaTime * smoothCamRot);

        bool stopped = vel < 0.02f;

        if(!stopped && Input.GetKey(KeyCode.Space)) Time.timeScale =  Mathf.Clamp( 2f + (0.5f/vel), 2f, 5f);
        else Time.timeScale = 1f;

        return stopped;
    }

    private bool isGrounded(Vector3 pos, float distToGround)
    {
        return Physics.Raycast(pos + Vector3.up * (distToGround/5), -Vector3.up,  distToGround);
    
    }

    public override void OnStart()
    {
        this.transform.LookAt(new Vector3( user.transform.position.x, this.transform.position.y, user.transform.position.z));
        this.transform.Rotate(Vector3.up,90);
        user.transform.position = CharPos.position;
        user.transform.rotation = CharPos.rotation;

        cc.EnterSwing(CamPos);

        GetComponent<BoxCollider>().enabled = false;

        UIThrow.GetComponent<ThrowController>().enabled = true;
        UIThrow.GetComponent<ThrowController>().WakeUp();
    }


    public override void OnEnd()
    {
        UIThrow.GetComponent<ThrowController>().Sleep();
        UIThrow.GetComponent<ThrowController>().enabled = false;

        cc.ExitSwing(Ball.transform.position);

        GetComponent<BoxCollider>().enabled = true;
    }

    public override string MessageToUse()
    {
        return "[E] Use Ball";
    }

    public override string MessageOnUse()
    {
        return "[A/D] Left/Right \n[Space]Throw \n[W/S] Effect \n[E] Exit";
    }
}
