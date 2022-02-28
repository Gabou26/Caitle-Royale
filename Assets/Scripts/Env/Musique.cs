using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musique : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().Play();
    }
}
