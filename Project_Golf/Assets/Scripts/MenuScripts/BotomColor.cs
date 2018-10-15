using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotomColor : MonoBehaviour {

    public Color color;
    public Material material;


    public void pickColor(){


        material.color = color;

    }
}
