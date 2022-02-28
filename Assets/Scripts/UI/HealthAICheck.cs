using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAICheck : MonoBehaviour
{
    //Champs
    float delaiCour = 0;
    float interUI = 1f;

    Vector3 viewP;

    public void UpdateUI(Camera cam)
    {
        viewP = cam.WorldToViewportPoint(transform.position);
        if (viewP.x > 0f && viewP.x <= 1f && viewP.y > 0f && viewP.y <= 1f)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
