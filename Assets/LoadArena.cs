using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadArena : MonoBehaviour
{
    public TransitionUI transStart, transBleu;
    public LayerMask layerTrigger;
    public string sceneToLoad;

    private void Start()
    {
        if (transStart != null)
            StartCoroutine(LoadTrans());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layerTrigger) != 0)
        {
            StartCoroutine(Loading());
        }
    }

    public void Activer()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        transBleu.Transition(new Vector2(0, 1200), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator LoadTrans()
    {
        yield return new WaitForSeconds(0.1f);
        transStart.Transition(new Vector2(0, 0), new Vector2(0, -1200), true);
    }
}
