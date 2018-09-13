using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPointBall : MonoBehaviour {

    public GameObject ball;
    public bool show;
    public TMPro.TextMeshProUGUI text;
    private Camera cam;

    private Image img;
    private Color col;

    // Use this for initialization
    void Start () {
        show = true;
        cam = Camera.main;
        img = this.GetComponent<Image>();
        col = img.color;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        PositionArrow();
    }

    void PositionArrow()
    {
        text.text = ""+ Mathf.RoundToInt(Vector3.Distance(cam.transform.position, ball.transform.position));
        Vector3 v3Pos = Camera.main.WorldToViewportPoint(ball.transform.position);
        float fAngle;


        if (v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f) // Object center is visible
        {

            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0f);

            transform.position = cam.WorldToScreenPoint(ball.transform.position + (cam.transform.up/2));
            return;
        }
        else
        {
            int s= v3Pos.z < Camera.main.nearClipPlane? 0:180;
            v3Pos.x -= 0.5f;  // Translate to use center of viewport
            v3Pos.y -= 0.5f;
            v3Pos.z = 0;

            fAngle = Mathf.Atan2(v3Pos.x, v3Pos.y);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAngle * Mathf.Rad2Deg + s);

            transform.localPosition = Vector3.zero - this.transform.up * 100;
        }
    }
}
