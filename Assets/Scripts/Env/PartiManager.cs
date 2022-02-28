using System.Collections;
using UnityEngine;

public class PartiManager : MonoBehaviour
{
    public GameObject partiPrefab;
    public int nbParticules = 6;
    public void Exploser(GameObject caster)
    {
        StartCoroutine(Explosion(caster));
    }
    IEnumerator Explosion(GameObject caster)
    {
        Transform obj;
        for (int i = 0; i < nbParticules; i++)
        {
            obj = Instantiate(partiPrefab, this.transform).transform;
            float degree = Random.Range(0, 360);
            Vector2 dir = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
            obj.GetComponent<Particule>().caster = caster;
            obj.GetComponent<Particule>().FireDirection(dir * Random.Range(0.8f, 1.2f));
            yield return new WaitForSeconds(0.005f);
        }
    }
}
