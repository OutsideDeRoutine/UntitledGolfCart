using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BotomStart : MonoBehaviour {

    public void startGame(){
        SceneManager.LoadScene("DemoMap");
    }
}
