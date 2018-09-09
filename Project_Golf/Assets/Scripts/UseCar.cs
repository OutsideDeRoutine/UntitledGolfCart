using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCar : MonoBehaviour {

    public Transform CharPos;

    private GameObject user;

    private bool isUsing;

	public void StartUsing(GameObject user)
    {
        this.user = user;
        isUsing = true;
        user.GetComponent<CharacterController>().enabled = false;
        user.GetComponent<CharControlller>().enabled = false;
        this.GetComponent<CarController>().enabled = true;
        this.GetComponent<BoxCollider>().enabled = false;
        Camera.main.GetComponent<CameraController>().altUse = true;
    }

    public void EndUsing()
    {
        this.user = null;
        isUsing = false;
        user.GetComponent<CharacterController>().enabled = true;
    }
	
	void Update () {
        if (isUsing)
        {
            user.transform.position = CharPos.position;
            user.transform.rotation = CharPos.rotation;
        }
        

    }
}
