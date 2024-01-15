using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerpTest : MonoBehaviour
{
    public Transform origin;
    public GameObject Up;
    public GameObject Down;
    public GameObject Front;
    public GameObject Back;
    public GameObject Left;
    public GameObject Right;
    public GameObject UpLeft;
    public GameObject UpRight;
    public GameObject DownFront;
    public GameObject LeftBack;



    void Start()
    {
        if (Up.activeSelf)
        {
            WorkWith(Up);
        }
        if (Down.activeSelf)
        {
            WorkWith(Down);
        }
        if (Front.activeSelf)
        {
            WorkWith(Front);
        }
        if (Back.activeSelf)
        {
            WorkWith(Back);
        }
        if (Left.activeSelf)
        {
            WorkWith(Left);
        }
        if (Right.activeSelf)
        {
            WorkWith(Right);
        }
        //
        if (UpLeft.activeSelf)
        {
            WorkWith(UpLeft);
        }
        if (UpRight.activeSelf)
        {
            WorkWith(UpRight);
        }
        if (DownFront.activeSelf)
        {
            WorkWith(DownFront);
        }
        if (LeftBack.activeSelf)
        {
            WorkWith(LeftBack);
        }
    }

   void WorkWith(GameObject g)
    {
        Vector3 testing = g.transform.position - origin.position;
        Debug.DrawLine(origin.position, testing, Color.red, 30f);
        Vector3 perp = Utils.GetUnitPerp(testing);
        Debug.DrawLine(origin.position, perp, Color.yellow, 30f);
    }
}
