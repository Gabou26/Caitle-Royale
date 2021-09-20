using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerName : MonoBehaviour
{
    public GameObject player;

    Vector2 posCible, posDepart;
    float delaiMouv = 1;
    float vit = 5;

    private void Awake()
    {
        transform.localPosition = Vector2.zero;
    }

    private void Update()
    {
        if (delaiMouv < 1)
        {
            delaiMouv += Time.deltaTime * vit;
            transform.localPosition = Vector2.Lerp(posDepart, posCible, delaiMouv);
        }
    }

    public void UpdatePos(Vector2 pos)
    {
        if (posCible == pos)
            return;
        posDepart = transform.localPosition;
        posCible = pos;
        delaiMouv = 0;
    }
}
