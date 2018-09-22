using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBall : AbstractUsable {

    public GameObject Ball;
    public Transform CharPos;
    public Transform CamPos;

    public bool InUse;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                cc.Swing();
                if (cc.stick.stick != null) StartCoroutine(Swing());
            }
        }
    }

    public IEnumerator Swing(){

        InUse = true;

        //ESPERA A MOMENTO DEL GOLPE
        yield return new WaitUntil(() => cc.AnimationState("Swing") >0.55);

       

        //  1. CAMBIAR LA BOLA ESTATICA POR BOLA FISICA.
        Ball.GetComponent<SphereCollider>().enabled = true;
        Ball.GetComponent<Rigidbody>().isKinematic = false;

        //  2. HACER CALCULOS -> LANZAR BOLA 
        Vector3 force = cc.GetSwingForce(transform.forward,transform.up);

        Ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        Vector3 torque = GetSwingTorque(); 

        Ball.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.Impulse);

        yield return new WaitUntil(() => cc.AnimationState("Swing") >= 0.6);

        //  3. CAMARA SEGUIR BOLA HASTA QUE ATERRIZA Y SE PARA
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
        cam.position = Vector3.SmoothDamp(Camera.main.transform.position , Ball.transform.position - this.transform.forward / 1.7f + Vector3.up/10, ref velocity, smoothCamPos * Time.deltaTime);
        Quaternion rot = cam.rotation;
        cam.LookAt(Ball.transform.position - Ball.GetComponent<Rigidbody>().velocity/20);
        cam.rotation = Quaternion.RotateTowards(rot, cam.transform.rotation ,Time.deltaTime * smoothCamRot);

        return Ball.GetComponent<Rigidbody>().velocity.magnitude < 0.02f;
    }

    internal Vector3 GetSwingTorque()
    {
        return this.transform.right * 100;
    }

    public override void OnStart()
    {
        this.transform.LookAt(new Vector3( user.transform.position.x, this.transform.position.y, user.transform.position.z));
        this.transform.Rotate(Vector3.up,90);
        user.transform.position = CharPos.position;
        user.transform.rotation = CharPos.rotation;

        cc.EnterSwing(CamPos);

        GetComponent<BoxCollider>().enabled = false;
    }


    public override void OnEnd()
    {
        cc.ExitSwing(Ball.transform.position);

        GetComponent<BoxCollider>().enabled = true;
    }

    public override string MessageToUse()
    {
        return "[E] Use Ball";
    }

    public override string MessageOnUse()
    {
        return "[A/D] Left/Right \n[Space] Throw \n[E] Exit";
    }
}
