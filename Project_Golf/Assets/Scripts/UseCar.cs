using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCar : AbstractUsable{

    public Transform CharPosIn;

    public Transform CharPosOutR;
    public Transform CharPosOutL;


    //TODO -> arreglar giros del personaje dentro y fuera del coche.
    //TODO -> comprobar cual es el mejor lado para salir [ L | R ]
    void Update () {
        if (isUsing)
        {
            user.transform.position = CharPosIn.position;
            user.transform.rotation = CharPosIn.rotation;
            if ( Input.GetKeyDown(KeyCode.E))
            {
                user.transform.position = CharPosOutR.position;
                
                EndUsing();
            }
        }
    }

    public override void OnStart()
    {
        user.GetComponent<CharControlller>().EnterCar();
        user.GetComponent<CharControlller>().enabled = false;

        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<CarController>().enabled = true;
    }

    public override void OnEnd()
    {
        user.GetComponent<CharControlller>().enabled = true;
        user.GetComponent<CharControlller>().ExitCar();


        //TODO -> parar coche.
        this.GetComponent<CarController>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = true;
    }
}
