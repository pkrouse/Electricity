using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningGenerator : MonoBehaviour, IActivatable
{
    public Transform origin;
    public GameObject cloudLight;
    public Transform[] endpoints;
    public GameObject linePrefab;
    AudioSource audioSrc;
    public AudioClip[] thunderClaps;
    int thunderClapIndex = 0;
    private Perturber perturber = new Perturber();

    /////////////////////////////////////
    // Global lightning behavior settings 
    /////////////////////////////////////
    int segmentCount = 7; // less than 4 gets boring
    //private float duration = 1f;
    // Branching bolts go between these angles
    float minBoltAngle = 10;
    float maxBoltAngle = 35;
    float initialBoltWidth = 0.15f;
    float boltWidthReduction = 0.5f;
    float boltAttenuation = 0.15f;
    float initialPercentChanceOfBranch = 90;


    private bool striking = false;
    // Lightning on/off flash sequences, in seconds...
    // A sequence starts with ON time, then OFF time, repeats.
    // Sensible entries are odd in length, because all are turned off at end.
    static float[] sequence1 = { 0.25f, 0.25f, 0.1f,  0.1f, 0.1f };
    static float[] sequence2 = { 0.1f,  0.05f, 0.05f, 0.01f, 0.1f, 0.03f, 0.1f };
    static float[] sequence3 = { 0.25f, 0.2f, 0.1f,  0.08f, 0.05f };
    static float[] sequence4 = { 0.15f, 0.01f, 0.17f,  0.06f, 0.1f, 0.04f, 0.03f };
    static float[] sequence5 = { 0.25f, 0.05f, 0.13f,  0.1f, 0.14f };
    // This way allows sequences of differing lengths.
    List<float[]> flashSequences = new List<float[]> { sequence1, sequence2, sequence3, sequence4, sequence5 };
    float[] flashSequence = null; // chosen from list above
    float sequenceTime = 0f;      // Progress in current entry in current flash sequence.
    int sequenceIndex = 0;       // What's currently going on in chosen sequence.
    int sequenceListIndex = 0;   // Sequence currently being used.
    bool sequenceSetting = false;

    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        if (striking)
            return;
        sequenceIndex = 0;
        sequenceTime = 0f;
        sequenceListIndex = Random.Range(0, flashSequences.Count);
        flashSequence = flashSequences[sequenceListIndex];
        sequenceSetting = true;
        cloudLight.SetActive(true);
        GenerateBolts();
        thunderClapIndex = Random.Range(0, thunderClaps.Length);
        audioSrc.PlayOneShot(thunderClaps[thunderClapIndex]);
    }

    private void Update()
    {
        if (striking)
        {
            sequenceTime += Time.deltaTime;
            if (sequenceTime >= flashSequence[sequenceIndex])
            {
                sequenceIndex++;
                sequenceTime = 0f;
                sequenceSetting = !sequenceSetting;
                SetLineRenderers(sequenceSetting);
                if (sequenceIndex >= flashSequence.Length)
                {
                    striking = false;
                    sequenceSetting = false;
                    SetLineRenderers(false);
                }
            }
        }
    }

    

    private void GenerateBolts()
    {
        
        lineRenderers.Clear();
        striking = true;
        Transform endPoint = endpoints[Random.Range(0, endpoints.Length)]; // max is exclusive on int version
        GenerateBolt(origin.position, endPoint.position, segmentCount, initialPercentChanceOfBranch, initialBoltWidth);
    }

    // Calculate new values and generate another bolt.
    private void BranchAndGenerate(Vector3 origin, Vector3 endPoint, Vector3 newOrigin, int segmentCount, 
        float totalDistance, float percentChanceOfBranch, float boltWidth)
    {
        // Make a vector from parent positions and go off at an angle.
        Vector3 dir = endPoint - origin;
        Vector3 cross = Utils.GetUnitPerp(dir);
        float angle = Random.Range(minBoltAngle, maxBoltAngle);
        float flipChance = Random.Range(0f, 1f);
        if (flipChance > 0.5f)
            angle = -angle;
        Quaternion rotation = Quaternion.AngleAxis(angle, cross);
        dir = rotation * dir;
        int newSegmentCount = segmentCount - 1;
        float newPercentChanceOfBranch = percentChanceOfBranch * 0.5f;
        float newTotalDistance = totalDistance / 4f;
        dir = dir.normalized * newTotalDistance;
        Vector3 newEndPoint = newOrigin + dir;
        float newBoltWidth = boltWidth * boltWidthReduction;
        GenerateBolt(newOrigin, newEndPoint, newSegmentCount, newPercentChanceOfBranch, newBoltWidth);
    }

    private void GenerateBolt(Vector3 origin, Vector3 endPoint, int segmentCount, float percentChanceOfBranch, float boltWidth)
    {
        // If we have gone down the branches far enough, bug out.
        if (percentChanceOfBranch < 20 || segmentCount == 0)
            return;
        Vector3 trail = new Vector3(endPoint.x, endPoint.y, endPoint.z) - origin;
        GameObject boltLinePrefab = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer boltLine = boltLinePrefab.GetComponent<LineRenderer>();
        float totalDistance = trail.magnitude;
        float segmentDistance = totalDistance / segmentCount;
        float perturbation = segmentDistance / 3;
        trail = trail.normalized;
        List<Vector3> points = new List<Vector3>();
        points.Add(origin);
        // skip beginning and end points
        for (int i=1; i<segmentCount; i++)
        {
            Vector3 pt = origin + trail * segmentDistance * i;
            pt = perturber.PerturbPoint(pt, perturbation);
            points.Add(pt);
            float chanceOfBranch = Random.Range(0, 100);
            if (chanceOfBranch < percentChanceOfBranch)
                BranchAndGenerate(origin, endPoint, pt, segmentCount, totalDistance, percentChanceOfBranch, boltWidth);
        }
        points.Add(endPoint);
        boltLine.startWidth = boltWidth;
        boltLine.endWidth = boltWidth * boltAttenuation;
        //if (segmentCount == 1)
        //    boltLine.endWidth = boltWidth / 10;
        boltLine.positionCount = points.Count;
        boltLine.SetPositions(points.ToArray());
        boltLine.enabled = true;
        lineRenderers.Add(boltLine);
    }

    private void SetLineRenderers(bool enabled)
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.enabled = enabled;
        }
        cloudLight.SetActive(enabled);
    }
}
