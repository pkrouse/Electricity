using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcGenerator : MonoBehaviour, IActivatable
{

    public Transform origin;
    public Transform[] destinations;
    Transform destination;
    private LineRenderer lightningLine;
    private Perturber perturber = new Perturber();
    float flashDuration = 0.75f;
    float flashTime = 0f;
    bool isStriking = false;
    float lightningMaxWidth = 0.1f;
    int segments = 10;
    List<Vector3> coords;
    float smoothingLengthMin = 0.1f;
    float smoothingLengthMax = 0.8f;
    int smoothingSections = 10;
    public GameObject linePrefab;
    AudioSource audioSrc;
    public GameObject debugSphere;

    void Awake()
    {
        lightningLine = GetComponent<LineRenderer>();
        lightningLine.startWidth = lightningMaxWidth;
        lightningLine.endWidth = lightningMaxWidth;
        destination = destinations[0];
        audioSrc = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        audioSrc.Play();
        ContinuousCurvedStrike();
    }

    void ContinuousCurvedStrike()
    {
        destination = destinations[(int)Random.Range(0, destinations.Length)];
        flashTime = 0f;
        isStriking = true;
        coords = GenerateCoords(origin, destination);
        RedrawArc();
    }

    void RedrawArc()
    {
        float smoothingLength = Random.Range(smoothingLengthMin, smoothingLengthMax);
        BezierCurve[] curves = InstantiateBezierCurves(smoothingLength, coords);
        SmoothPath(lightningLine, smoothingSections, curves, coords);
        lightningLine.enabled = true;
    }

    private void Update()
    {
        if (isStriking)
        {
            flashTime += Time.deltaTime;
            if (flashTime >= flashDuration)
            {
                isStriking = false;
                lightningLine.enabled = false;
                audioSrc.Stop();
            }
            else
            {
                RedrawArc();
            }
        }
        
    }
    private List<Vector3> GenerateCoords(Transform origin, Transform destination)
    {
        Vector3 currentPoint = origin.position;
        Vector3 endPoint = destination.position;
        float distance = Vector3.Distance(currentPoint, endPoint);
        float minDistance = distance / 8f;
        List<float> splits = new List<float>();
        for (int i = 0; i < segments; i++)
            splits.Add(Random.Range(minDistance, distance));
        splits.Sort();
        // turn the distances into differences
        for (int i = splits.Count - 1; i > 0; i--)
            splits[i] = splits[i] - splits[i - 1];
        List<Vector3> coords = new List<Vector3>();
        coords.Add(currentPoint); // the origin orb
        float angleMin = 8, angleMax = 20;
        foreach (float dist in splits)
        {
            Vector3 seg = endPoint - currentPoint;
            seg = Vector3.Normalize(seg) * dist;
            Vector3 bumped = perturber.PerturbVector(seg, angleMin, angleMax);
            currentPoint = currentPoint + bumped;
            coords.Add(currentPoint);
        }
        coords.Add(destination.position); // one of the receptor orbs
        return coords;
    }

    private void SmoothPath(LineRenderer line, int smoothingSections, BezierCurve[] curves, List<Vector3> coords)
    {
        line.positionCount = curves.Length * smoothingSections;
       
        int index = 0;
        for (int i = 0; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSections);
            for (int j = 0; j < segments.Length; j++)
            {
                line.SetPosition(index, segments[j]);
                index++;
            }
        }
    }

    BezierCurve[] InstantiateBezierCurves(float smoothingLength, List<Vector3> coords)
    {
        BezierCurve[] Curves = new BezierCurve[coords.Count - 1];
        
        for (int i = 0; i < Curves.Length; i++)
            Curves[i] = new BezierCurve();
        
        for (int i = 0; i < Curves.Length; i++)
        {
            // Vector3 position = line.GetPosition(i);
            Vector3 position = coords[i];
            Vector3 lastPosition = i == 0 ? coords[0] : coords[i-1];
            Vector3 nextPosition = coords[i + 1];

            Vector3 lastDirection = (position - lastPosition).normalized;
            Vector3 nextDirection = (nextPosition - position).normalized;

            Vector3 startTangent = (lastDirection + nextDirection) * smoothingLength;
            Vector3 endTangent = (nextDirection + lastDirection) * -1 * smoothingLength;

            Curves[i].Points[0] = position; // Start Position (P0)
            Curves[i].Points[1] = position + startTangent; // Start Tangent (P1)
            Curves[i].Points[2] = nextPosition + endTangent; // End Tangent (P2)
            Curves[i].Points[3] = nextPosition; // End Position (P3)
        }

        // Apply look-ahead for first curve and retroactively apply the end tangent
        {
            Vector3 nextDirection = (Curves[1].EndPosition - Curves[1].StartPosition).normalized;
            Vector3 lastDirection = (Curves[0].EndPosition - Curves[0].StartPosition).normalized;

            Curves[0].Points[2] = Curves[0].Points[3] +
                (nextDirection + lastDirection) * -1 * smoothingLength;

        }
        return Curves;
    }

}
