using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTester : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform point;
    public GameObject moveablePoint;
    public Vector3 vector;
    private Perturber perturber;
    void Start()
    {
        perturber = new Perturber();
        Debug.DrawLine(point.position, vector + point.position, Color.white, 30f);
        //for (int i = 0; i < 200; i++)
       // {
       //     Vector3 p = perturber.Perturb1debug(vector, 0.1f, point.position, moveablePoint);
       //     Debug.DrawLine(point.position, point.position + p, Color.red, 30f);
       // }
        //Vector3 p = perturber.Perturb1debug(vector, 0.1f, point.position, moveablePoint);
        //Debug.DrawLine(point.position, point.position + p, Color.red, 30f);
        for (int i=0; i<200; i++)
        {
           Vector3 p = perturber.PerturbVector(vector, 10f, 30f);
           Debug.DrawLine(point.position, point.position + p, Color.red, 30f);
       
        }
    }
}
