using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float speed;
    
    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, speed * Time.fixedTime);
    }
}
