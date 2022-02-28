using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyaleModeButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup aiButton;

    private void OnEnable()
    {
        aiButton.alpha = 0.4f;
        aiButton.blocksRaycasts = false;
        StartCoroutine(AttribuerValAI());
    }

    private void OnDisable()
    {
        aiButton.alpha = 1;
        aiButton.blocksRaycasts = true;
    }

    IEnumerator AttribuerValAI()
    {
        yield return new WaitForEndOfFrame();
        //aiButton.GetComponent<Selector>().SetIndex(0);
    }
}
