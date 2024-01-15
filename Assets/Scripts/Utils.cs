using UnityEngine;

public class Utils 
{
    public static Vector3 GetUnitPerp(Vector3 v)
    {
        Vector3 t = Vector3.forward;
        if (t == v.normalized || -t == v.normalized)
            t = Vector3.up;
        return Vector3.Cross(v, t).normalized;
    }
}
