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
        Debug.Log("내적: " + Vector3.Dot(A_Vector, B_Vector));
        Debug.Log("내적: " + Vector3.Cross(A_Vector, B_Vector));
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, A_Vector);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, B_Vector);



        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(humanMaskCharacter.transform.position + humanMaskCharacter.transform.up, new Vector3(2, 2, 2));
        //Gizmos.DrawWireSphere(humanMaskCharacter.transform.position + humanMaskCharacter.transform.up, 1.5f);
        //Vector3 characterPosition = humanMaskCharacter.transform.position + humanMaskCharacter.transform.up;
        //Gizmos.DrawLine(characterPosition, characterPosition + humanMaskCharacter.transform.forward * 2);
        //Gizmos.DrawLine(characterPosition + humanMaskCharacter.transform.right, characterPosition + humanMaskCharacter.transform.right + humanMaskCharacter.transform.forward * 2);
        //Gizmos.DrawLine(characterPosition - humanMaskCharacter.transform.right, characterPosition - humanMaskCharacter.transform.right + humanMaskCharacter.transform.forward * 2);

        //Vector3 characterPositionUp = humanMaskCharacter.transform.position + humanMaskCharacter.transform.up * 2;
        //Gizmos.DrawLine(characterPositionUp, characterPositionUp + humanMaskCharacter.transform.forward * 2);
        //Gizmos.DrawLine(characterPositionUp + humanMaskCharacter.transform.right, characterPositionUp + humanMaskCharacter.transform.right + humanMaskCharacter.transform.forward * 2);
        //Gizmos.DrawLine(characterPositionUp - humanMaskCharacter.transform.right, characterPositionUp - humanMaskCharacter.transform.right + humanMaskCharacter.transform.forward * 2);
    }
#endif

}
