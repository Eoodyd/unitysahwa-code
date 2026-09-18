using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    public Vector3 A_Vector;
    public Vector3 B_Vector;
    public Vector3 cameraForwardVector1;
    public Vector3 rotationVector1;
    public Vector3 rotationVector2;
    public Quaternion rotationQuaternion;

    private void Update()
    {
        rotationQuaternion = Quaternion.Euler(B_Vector);
        
        Debug.Log("rotationQuaternion * A_Vector: " + rotationQuaternion * A_Vector);
        Debug.Log("����: " + Vector3.Dot(A_Vector, B_Vector));
        Debug.Log("����: " + Vector3.Cross(A_Vector, B_Vector));
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, A_Vector);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, B_Vector);
    }
#endif

}
