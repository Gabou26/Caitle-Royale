using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseurRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
}
