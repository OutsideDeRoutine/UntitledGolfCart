using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseClubs : AbstractUsable
{
    public List<GameObject> stack;
    int selected;
    public Transform CamPos;

    void Start()
    {
        UpdatePosition();
    }

    //ROTAR Y OPCION A SMOOTH
    void UpdatePosition()
    {
        for(int i= 0; i< stack.Count; i++)
        {
            
            stack[i].transform.localPosition = Vector3.zero;

            stack[i].transform.localRotation=Quaternion.Euler(Vector3.zero);

            if (i == selected &&  isUsing)
            {
                stack[i].transform.localPosition += this.transform.forward / 5;
            }

            float angle = ((i-(float)(selected)) / (float)(stack.Count)) * 360;

            stack[i].transform.Rotate(this.transform.forward, angle - 90);

            stack[i].transform.Translate(-this.transform.up / 10);
        }
    }

    void Update()
    {

        
        if (isUsing )
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                EndUsing();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Next();
                UpdatePosition();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Prev();
                UpdatePosition();
            }
        }
    }

    private void Prev()
    {
        selected--;
        if(selected < 0)
        {
            selected = stack.Count-1;
        }
    }

    private void Next()
    {
        selected++;
        if (selected >= stack.Count)
        {
            selected = 0;
        }
    }

    public override void OnEnd()
    {
        user.GetComponent<CharControlller>().ExitSelection(Instantiate(stack[selected].gameObject));

        GetComponent<BoxCollider>().enabled = true;

        UpdatePosition();
    }

    public override void OnStart()
    {
        user.GetComponent<CharControlller>().EnterSelection(CamPos);

        GetComponent<BoxCollider>().enabled = false;

        isUsing = true;

        UpdatePosition();
    }
}
