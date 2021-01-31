using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform Ref;
    void LateUpdate()
    {
        transform.position = Ref.position;
    }
}
