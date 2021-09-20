using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private bool autoStart = true, hideStart = true;
    float distCour = 1;
    Vector2 posDepart, posFin;
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        GetComponent<Image>().enabled = true;
        if (autoStart)
            rect.localPosition = new Vector2(0, 0);
        if (hideStart)
            GetComponent<Image>().enabled = false;
    }

    private void Start()
    {
        if (autoStart)
            StartCoroutine(LoadTrans());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (distCour < 1)
        {
            distCour += Time.deltaTime * 2f;
            if (distCour > 1)
                distCour = 1;
            rect.localPosition = Vector2.Lerp(posDepart, posFin, distCour);
        }
    }

    public void Transition(Vector2 posDepart, Vector2 posFin, bool fin)
    {
        StartCoroutine(Trans(posDepart, posFin, fin));
    }

    IEnumerator Trans(Vector2 posDepart, Vector2 posFin, bool fin)
    {
        yield return new WaitForEndOfFrame();
        distCour = 0;
        GetComponent<Image>().enabled = true;
        this.posDepart = posDepart;
        this.posFin = posFin;
        rect.localPosition = posDepart;
        if (fin)
        {
            yield return new WaitForSeconds(1);
            GetComponent<Image>().enabled = false;
        }
    }

    IEnumerator LoadTrans()
    {
        yield return new WaitForSeconds(0.1f);
        Transition(new Vector2(0, 0), new Vector2(2000, 0), true);
    }
}
