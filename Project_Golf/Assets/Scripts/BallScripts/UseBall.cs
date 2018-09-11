using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBall : AbstractUsable {


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

        //ESPERA A MOMENTO DEL GOLPE
        yield return new WaitUntil(() => user.GetComponent<CharControlller>().AnimationState("Swing") >0.6);

        //  2. HACER CALCULOS -> LANZAR BOLA 
        //  3. CAMARA SEGUIR BOLA HASTA QUE ATERRIZA Y SE PARA

        //ESPERA A MOMENTO TERMINAR LA ANIMACION
        yield return new WaitUntil(()=> user.GetComponent<CharControlller>().AnimationState("Swing") > 1); 
        
        EndUsing();
        //  5. CAMBIAR LA BOLA FISICA POR BOLA ESTATICA.
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
