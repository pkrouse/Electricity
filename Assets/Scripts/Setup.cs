using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public enum AreaChoice { Lightning, Arc, Jacobs_Ladder }
   

    [TextArea(4, 4)]
    public string Instructions = string.Join(Environment.NewLine,
        "To play within Unity, set Location below for the effect you want to see.",
        "Then press play.  Use space bar to activate the effect.",
        "In a headset, just read the billboard for instructions to see all three effects.");
    public AreaChoice Location = AreaChoice.Lightning;

    public GameObject XR;
    public GameObject Placard;
    public Transform LightningStart;
    public Transform ArcStart;
    public Transform LadderStart;
    void Start()
    {
        switch (Location)
        {
            case AreaChoice.Lightning:
                XR.transform.position = LightningStart.position;
                XR.transform.rotation = LightningStart.rotation;
                break;

            case AreaChoice.Arc:
                XR.transform.position = ArcStart.position;
                XR.transform.rotation = ArcStart.rotation;
                break;

            case AreaChoice.Jacobs_Ladder:
                XR.transform.position = LadderStart.position;
                XR.transform.rotation = LadderStart.rotation;
                break;
        }
        Vector3 newPosition = new Vector3(XR.transform.position.x, Placard.transform.position.y, XR.transform.position.z);
        Placard.transform.position = newPosition;
        Placard.transform.rotation = XR.transform.rotation;
    }
}
