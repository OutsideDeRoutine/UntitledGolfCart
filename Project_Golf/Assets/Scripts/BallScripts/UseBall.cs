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
                user.GetComponent<CharControlller>().Swing();
                StartCoroutine(Swing());
            }
        }
    }

    public IEnumerator Swing(){
        InUse = true;
        //ESPERA A MOMENTO DEL GOLPE
        yield return new WaitUntil(() => user.GetComponent<CharControlller>().AnimationState("Swing") >0.55);

        //  1. CAMBIAR LA BOLA ESTATICA POR BOLA FISICA.
        Ball.GetComponent<SphereCollider>().enabled = true;
        Ball.GetComponent<Rigidbody>().isKinematic = false;

        //  2. HACER CALCULOS -> LANZAR BOLA 
        Vector3 force = this.transform.forward * 2 + this.transform.up * 5; //TESTING
        
        Ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        Vector3 torque = this.transform.right * 10; //TESTING

        Ball.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.Impulse);

        yield return new WaitUntil(() => user.GetComponent<CharControlller>().AnimationState("Swing") >= 0.8);
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

    public float smoothCamPos = 0.2f;
    public float smoothCamRot = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }
    public bool WhileBallMoving()
    {
        cam.position = Vector3.SmoothDamp(Camera.main.transform.position , Ball.transform.position - this.transform.forward / 2 + Vector3.up/10, ref velocity, smoothCamPos);
        Quaternion rot = cam.rotation;
        cam.LookAt(Ball.transform.position);
        cam.rotation = Quaternion.RotateTowards(rot, cam.transform.rotation,Time.time * smoothCamRot);

        return Ball.GetComponent<Rigidbody>().velocity.magnitude < 0.02f;
    }

    public override void OnStart()
    {
        user.transform.position = CharPos.position;
        user.transform.rotation = CharPos.rotation;
        user.GetComponent<CharControlller>().EnterSwing(CamPos);
        GetComponent<BoxCollider>().enabled = false;
        
    }


    public override void OnEnd()
    {
        user.GetComponent<CharControlller>().ExitSwing();
        GetComponent<BoxCollider>().enabled = true;

    }
}
