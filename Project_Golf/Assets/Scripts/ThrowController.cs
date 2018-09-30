﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowController : MonoBehaviour {

    public GameObject Ball;

    public GameObject Effect;
    public GameObject Button;
    public GameObject Accuracy;
    public GameObject Power;



    private Scrollbar eff;
    private Scrollbar acc;
    private Scrollbar pow;

    private UseBall ub;

    void OnEnable()
    {
        eff = Effect.GetComponent<Scrollbar>();
        acc = Accuracy.GetComponent<Scrollbar>();
        pow = Power.GetComponent<Scrollbar>();
        ub = Ball.GetComponent<UseBall>();

        Reset();
        num = 0;
    }

    private Coroutine routineEF;
    private float efmov;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickSpace();
        }

        float move = Input.GetAxis("Vertical");
        if (move != 0)
        {
            efmov = -0.1f * move;
            if (routineEF == null)  routineEF = StartCoroutine(MoveEF());
        }
        else
        {
            efmov = 0f;
            if (routineEF != null)
            {
                StopCoroutine(routineEF);
                routineEF = null;
            }
        }
    }

    public IEnumerator MoveEF()
    {
        while (efmov!=0)
        {
            eff.value += efmov;
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void ClickSpace()
    {
        if (!usin) { StopAllCoroutines(); }
        StartCoroutine(ClickSpaceCoroutine());
    }

    public int num;
    public bool usin;
    public IEnumerator ClickSpaceCoroutine()
    {

        num++;
        Button.GetComponent<Image>().fillCenter = true;

        if (!usin)
        {
            StartCoroutine(StartPower());
        }
           

        yield return new WaitForSeconds(0.2f);

        Button.GetComponent<Image>().fillCenter = false;
    }

    public IEnumerator StartPower()
    {
        Reset();
        usin = true;
        while (usin)
        {
            switch (num)
            {
                case 1:
                    if (pow.value == 1)
                    {
                        Reset();

                        yield return new WaitForFixedUpdate();

                        usin = false;
                        num = 0;
                    }
                    else
                    {
                        acc.value += 0.5f * Time.deltaTime;
                        pow.value += 0.5f * Time.deltaTime;
                    }
                   
                    break;

                case 2:
                    if (acc.value == 0)
                    {
                        Reset();

                        yield return new WaitForFixedUpdate();

                        usin = false;
                        num = 0;
                    }
                    else
                    {
                        acc.value -= 0.5f * Time.deltaTime;
                    }
                    
                    break;

                case 3:
                    
                    usin = false;
                    num = -1;
                    Shoot();
                    break;

            }
            
            yield return new WaitForFixedUpdate();
        }
       
    }

    public void Reset()
    {
        acc.value = 0f;
        pow.value = 0f;
    }
    public void Shoot()
    {
        float ef = (eff.value - 0.5f) * 2;

        ub.tw.ac = Mathf.Clamp( (-0.1f + acc.value)*(10 + (Mathf.Abs(ef) * 2)), -1, 1 );

        ub.tw.fc = (ef < 0 ? (100 - (50 * -ef)) : 100) * pow.value;
        ub.tw.ht = (ef > 0 ? (100 - (50 * ef)) : 100) * pow.value;

        ub.tw.ef = ef;
        ub.StartSwing();        
    }

    public void WakeUp()
    {
        this.GetComponent<CanvasGroup>().alpha = 1;       
    }

    public void Sleep()
    {
        this.GetComponent<CanvasGroup>().alpha = 0;
       
    }
}
