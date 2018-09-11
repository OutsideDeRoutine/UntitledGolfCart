using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBall : AbstractUsable {

    public GameObject Ball;
    public Transform CharPos;
    public Transform CamPos;
    void Update() {
        if (isUsing)
        {

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
        //  1. CAMBIAR LA BOLA ESTATICA POR BOLA FISICA.
        Ball.GetComponent<SphereCollider>().enabled = true;
        Ball.GetComponent<Rigidbody>().isKinematic = false;
        //ESPERA A MOMENTO DEL GOLPE
        yield return new WaitUntil(() => user.GetComponent<CharControlller>().AnimationState("Swing") >0.6);

        //  2. HACER CALCULOS -> LANZAR BOLA 
        Vector3 force = Vector3.forward * 5 + Vector3.up * 3; //TESTING

        Ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        yield return new WaitForFixedUpdate();

        //  3. CAMARA SEGUIR BOLA HASTA QUE ATERRIZA Y SE PARA

        yield return new WaitUntil(() => Ball.GetComponent<Rigidbody>().velocity.magnitude < 0.1f);
        Ball.GetComponent<SphereCollider>().enabled = false;
        Ball.GetComponent<Rigidbody>().isKinematic = true;
 
        
        EndUsing();
        //  5. MOVER TODO A LA POSICION DE LA BOLA.
        Vector3 pos = Ball.transform.localPosition;
        this.transform.Translate(pos);

        Ball.transform.localPosition = Vector3.zero;

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
