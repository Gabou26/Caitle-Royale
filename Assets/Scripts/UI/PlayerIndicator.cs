using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    float mouvCour = 0;
    float dirY = 0.1f;
    Vector2 posBase, posDepart, posCible;

    // Start is called before the first frame update
    void Start()
    {
        posBase = transform.localPosition;
        posDepart = posBase;
        posCible = posBase + new Vector2(0, dirY);
    }

    public void SetPlayer(int id)
    {
        SpriteRenderer spriteR = GetComponent<SpriteRenderer>();
        switch (id)
        {
            case 1:
                spriteR.color = new Color(0.35f, 0.5f, 1f);
                break;
            case 2:
                spriteR.color = new Color(0.5f, 1, 0.35f);
                break;
            case 3:
                spriteR.color = new Color(1f, 0.35f, 0.35f);
                break;
            case 4:
                spriteR.color = new Color(1f, 0.85f, 0.35f);
                break;
            default:
                spriteR.color = new Color(0.35f, 0.95f, 1f);
                break;
        }
        spriteR.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        mouvCour += Time.deltaTime * 1.6f;

        transform.localPosition = Vector2.Lerp(posDepart, posCible, mouvCour);

        if (mouvCour >= 1)
        {
            dirY = -dirY;
            posDepart = transform.localPosition;
            posCible = posBase + new Vector2(0, dirY);
            mouvCour = 0;
        }

    }
}
