using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBall : AbstractUsable {


    public Transform CharPosGolpeo;
	
	void Update () {
		
	}


    public override void OnStart()
    {
        user.transform.position = CharPosGolpeo.position;
        Debug.Log("Se ha colocado en la posicion de golpeo");

    }


    public override void OnEnd()
    {




    }
}
