using UnityEngine;
using System.Collections;
using UnityEditor;

public class testpoint : MonoBehaviour
{
    private Vector3 start;
    private Vector3 calcVector3;
    private Vector3 calcRotate;
    private Vector3 origin;

    void Start()
    {
        origin = Vector3.one;
        start = Vector3.right;
        calcVector3 = Vector3.zero;
    }



    void Update()
    {

        if (canShow)
        {
            Debug.DrawLine(origin, origin + start, Color.red);
            Debug.DrawLine(origin + start, origin + calcRotate, Color.blue);
            Debug.DrawLine(origin, origin + calcVector3, Color.black);
        }

    }


    private float angle = 30f;
    private bool canShow = false;
    //private Vector3 originPosition = Vector3.zero;


    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 70, 30), "calc"))
        {
            canShow = true;
            calcVector3 = Calc(start, 5, 30);
        }

        GUI.changed = false;
        angle = GUI.HorizontalSlider(new Rect(0, 30, 100, 17), angle, 0, 360);
        if (GUI.changed)
        {
            calcVector3 = Calc(start, 5, angle);
        }

    }

    public Vector3 Calc(Vector3 v, float distance, float angleTemp)
    {
        var dir = v.normalized*distance;
        calcRotate = Quaternion.Euler(0, 0, angleTemp) * dir;
        return calcRotate ;
    }

}
