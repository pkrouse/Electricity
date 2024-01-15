using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTester : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject p0;
    public GameObject p1;
    public GameObject p2;
    private LineRenderer lr;
    float lightningMaxWidth = 0.1f;

    void Start()
    {
        GameObject go = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lr = go.GetComponent<LineRenderer>();   
    }

    void Update()
    {
        lr.enabled = false;
        Vector3 v0 = p0.transform.position;
        Vector3 v1 = p1.transform.position;
        Vector3 v2 = p2.transform.position;
        Bezier(lr, v0, v1, v2);
        lr.enabled = true;
    }

    void Bezier(LineRenderer lr, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        lr.positionCount = 7;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < lr.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
            lr.SetPosition(i, B);
            t += (1 / (float)(lr.positionCount-1));
        }
        lr.startWidth = lightningMaxWidth;
        lr.endWidth = lightningMaxWidth;
    }
}
