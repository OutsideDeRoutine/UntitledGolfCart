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
        UpdatePosition(false);
    }

    private int roting;

    //ROTAR Y OPCION A SMOOTH
    void UpdatePosition(bool smooth)
    {
        if (roting==0)
        {
            Vector3 prev = Vector3.zero;
            Quaternion preq = Quaternion.identity;

            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i])
                {
                    if (smooth)
                    {
                        prev = stack[i].transform.position;
                        preq = stack[i].transform.rotation;
                    }

                    stack[i].transform.localPosition = Vector3.zero;

                    stack[i].transform.localRotation = Quaternion.Euler(Vector3.zero);

                    if (i == selected && isUsing)
                    {
                        stack[i].transform.localPosition += -Vector3.forward / 3;
                    }

                    float angle = ((i - (float)(selected)) / (float)(stack.Count)) * 360;

                    stack[i].transform.Rotate(-Vector3.forward, angle - 90);


                    stack[i].transform.Translate(-Vector3.up / 10);

                    if (smooth)
                    {

                        Vector3 prevt = stack[i].transform.position;
                        Quaternion preqt = stack[i].transform.rotation;

                        stack[i].transform.position = prev;

                        stack[i].transform.rotation = preq;

                        StartCoroutine(SmoothMove(stack[i], prevt, preqt));
                    }
                }
            }
        }
    }

    IEnumerator SmoothMove(GameObject go, Vector3 tov ,Quaternion toq)
    {
        roting++;
        while (Vector3.Distance(go.transform.position, tov) + Quaternion.Angle(go.transform.rotation, toq) >= 1f)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, tov, Time.deltaTime * 10);
            go.transform.rotation = Quaternion.Slerp(go.transform.rotation, toq, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }
        roting--;
    }

    void Update()
    {
        if (isUsing && roting == 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                EndUsing();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                    Next();
                    UpdatePosition(true);                    
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                    Prev();
                    UpdatePosition(true);
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
        if (stack[selected]) user.GetComponent<CharControlller>().ExitSelection(Instantiate(stack[selected].gameObject));
        else user.GetComponent<CharControlller>().ExitSelection(null);
        GetComponent<BoxCollider>().enabled = true;

        UpdatePosition(false);
    }

    public override void OnStart()
    {
        user.GetComponent<CharControlller>().EnterSelection(CamPos);

        GetComponent<BoxCollider>().enabled = false;

        isUsing = true;

        UpdatePosition(false);
    }

    public override string MessageToUse()
    {
        return "[E] Select Club";
    }

    public override string MessageOnUse()
    {
        return "[A/D] Prev/Next \n[E] Select";
    }
}
