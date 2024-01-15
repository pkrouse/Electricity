using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    public GameObject test;

    private void OnTriggerEnter(Collider other)
    {
        test.SetActive(true);
    }

   
}
