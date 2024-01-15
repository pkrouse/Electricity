using System.Collections.Generic;
using UnityEngine;

public class JacobsLadderGenerator : MonoBehaviour, IActivatable
{
    public Transform pivotPoint;
    public Transform sparkOriginLeft;
    public Transform sparkOriginRight;
    public Transform sparkEndLeft;
    public Transform sparkEndRight;
    public GameObject arcLight;
    private Vector3 originLeft;
    private Vector3 originRight;
    private Vector3 directionVectorLeft;
    private Vector3 directionVectorRight;
    private float speed = 1.8f;
    private bool arcing = false;
    private LineRenderer arcLine;
    private Perturber perturber = new Perturber();
    float arcWidth = 0.03f;
    AudioSource audioSrc;

    void Awake()
    {
        directionVectorLeft = sparkEndLeft.position - sparkOriginLeft.position;
        directionVectorLeft = directionVectorLeft.normalized;
        directionVectorRight = sparkEndRight.position - sparkOriginRight.position;
        directionVectorRight = directionVectorRight.normalized;
        arcLine = GetComponent<LineRenderer>();
        audioSrc = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        if (arcing == true)
            return;
        GenerateArc();
    }

    private void GenerateArc()
    {
        originLeft = sparkOriginLeft.position;
        originRight = sparkOriginRight.position;
        audioSrc.Play();
        arcLight.SetActive(true);
        arcing = true;
    }

    private void Update()
    {
        if (arcing)
        {
            originLeft += directionVectorLeft * speed * Time.deltaTime;
            originRight += directionVectorRight * speed * Time.deltaTime;
            if (originLeft.y >= sparkEndLeft.position.y)
            {
                audioSrc.Stop();
                arcing = false;
                arcLine.enabled = false;
                arcLight.SetActive (false);
                return;
            }
            List<Vector3> points = GeneratePoints(originLeft, originRight);
            arcLine.startWidth = arcWidth;
            arcLine.endWidth = arcWidth;
            arcLine.positionCount = points.Count;
            arcLine.SetPositions(points.ToArray());
            arcLine.enabled = true;
        }
    }

    // Generate the points between left and right that simulate an arc.
    List<Vector3> GeneratePoints(Vector3 left, Vector3 right)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(left);
        // Get a vector from the pivot point headed to its right
        Vector3 pt1 = pivotPoint.transform.position;
        Vector3 pt2 = pt1 + pivotPoint.transform.right;
        Vector3 axis = pt2 - pt1;
        Vector3 rotator = left - pivotPoint.transform.position;
        

        for (float angle=-5; angle > -30; angle = angle - 5)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 rotated = rotation * rotator;
            Vector3 newPoint = new Vector3(pivotPoint.transform.position.x, pivotPoint.transform.position.y, pivotPoint.transform.position.z);
            newPoint += rotated;
            newPoint = perturber.PerturbPoint(newPoint, 0.05f);
            points.Add(newPoint);
            // move light to center of arc
            if (angle == -15)
                arcLight.transform.position = newPoint + new Vector3(0.1f, 0, 0);
        } 
        points.Add(right);
        return points;
    }
}
