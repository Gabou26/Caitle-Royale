using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealFloor : MonoBehaviour
{
    [SerializeField] protected LayerMask playerTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerTrigger) != 0)
        {
            GetComponentInParent<HealEtage>()?.HealPlayer(other.transform, transform.parent.parent);
        }
    }
}
