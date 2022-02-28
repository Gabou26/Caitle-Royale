using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TrailEffect : MonoBehaviour
{
    [SerializeField] private float timeFadeOut;
    [SerializeField] private Color transitionColor;

    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var counter = 0f;
        var originalColor = spriteRenderer.color;
        while (counter < timeFadeOut)
        {
            spriteRenderer.color = Color.Lerp(originalColor, transitionColor, counter / timeFadeOut);
            counter += Time.fixedDeltaTime;
            yield return new WaitForSeconds(timeFadeOut);
        }
        Destroy(gameObject);
    }
}