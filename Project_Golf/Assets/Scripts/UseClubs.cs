using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseClubs : AbstractUsable
{
    public List<GameObject> stack;
    int selected;
    public Transform CamPos;


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
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Prev();
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
    }

    public override void OnStart()
    {
        user.GetComponent<CharControlller>().EnterSelection(CamPos);

        GetComponent<BoxCollider>().enabled = false;
    }
}
