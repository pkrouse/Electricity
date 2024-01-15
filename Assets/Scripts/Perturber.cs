using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perturber
{
    //
    //  Used to perturb given vector 'direction' by changing it by a random angle. 
    //

    public Vector3 PerturbVector(Vector3 direction, float angleMin, float angleMax)
    {
        float angle = Random.Range(angleMin, angleMax);
        Vector3 cross = Vector3.Cross(Vector3.up, direction).normalized;
        if (cross == Vector3.zero)
        {
            // Oops - direction vector is collinear with world up vector.
            cross = Vector3.Cross(Vector3.right, direction).normalized;
        }
        Vector3 rotatedDirection = Quaternion.AngleAxis(angle, cross) * direction;
        if (rotatedDirection == direction)
        {
            // Probably a 0-length direction vector, and should just return error?
            rotatedDirection = Quaternion.AngleAxis(angle, Vector3.right) * direction;
        }
        float angle2 = Random.Range(0f, 359f);
        return Quaternion.AngleAxis(angle2, direction) * rotatedDirection;
    }

    public Vector3 PerturbPoint(Vector3 pt, float magnitude)
    {
        return pt + Random.insideUnitSphere * magnitude;
    }
}
