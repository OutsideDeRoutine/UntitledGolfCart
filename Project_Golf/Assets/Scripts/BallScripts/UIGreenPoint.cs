using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGreenPoint : MonoBehaviour {

    public Transform player;
    public TMPro.TextMeshProUGUI text;
    private Camera cam;
    public List<Transform> greens;
    public bool on;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PositionArrow();
    }

    void PositionArrow()
    {
        bool useIT=false;
        Vector3 green = Vector3.positiveInfinity;
        Vector3 v3Fin = Vector3.zero;
        foreach (Transform g in greens)
        {
                Vector3 v3Pos = cam.WorldToViewportPoint(g.position);

                if (!(v3Pos.z < Camera.main.nearClipPlane) && v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f) // Object center is visible
                {
                    if( Vector3.Distance(player.position, green) > Vector3.Distance(player.position, g.position))
                    {
                        green = g.position;
                        v3Fin = v3Pos;
                        useIT = true;
                    }
                }
        }
        if (on && useIT && Vector3.Distance(player.position, green)>10)
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 0;
            return;
        }
        text.text = "" + Mathf.RoundToInt(Vector3.Distance(player.position, green));


        transform.position = cam.WorldToScreenPoint(green + (cam.transform.up));
    }
}
