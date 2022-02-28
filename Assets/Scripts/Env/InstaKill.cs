using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col)
            col.GetComponent<HealSystem>()?.Death(col.gameObject);
    }
}
