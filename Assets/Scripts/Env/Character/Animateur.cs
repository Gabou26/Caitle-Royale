using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animateur : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private bool startAwake = true;
    [SerializeField] private float interFrame = 1f;
    [SerializeField] private bool loop = true;

    SpriteRenderer spriteR;
    bool isPlaying = false;
    float delaiCour;
    int idCour;

    void Awake()
    {
        spriteR = GetComponent<SpriteRenderer>();
        interFrame = Random.Range(interFrame * 0.8f, interFrame * 1.2f);
        if (startAwake && !isPlaying)
            Play();
    }

    void Update()
    {
        if (!isPlaying)
            return;
        delaiCour += Time.deltaTime;

        while (delaiCour >= interFrame)
        {
            delaiCour -= interFrame;

            if (idCour < frames.Length - 1)
                idCour++;
            else
            {
                idCour = 0;
                if (!loop)
                {
                    isPlaying = false;
                    return;
                }
            }
            spriteR.sprite = frames[idCour];
        }
    }

    public void MouvAnim()
    {
        delaiCour += Time.deltaTime;
    }

    public void SetAnim(Sprite[] frames)
    {
        if (spriteR == null)
            spriteR = GetComponent<SpriteRenderer>();

        this.frames = frames;
        if (idCour >= frames.Length)
            idCour = 0;
        spriteR.sprite = frames[idCour];
    }

    public Sprite[] GetAnim()
    {
        return frames;
    }

    public void SetDelai(float delaiCour)
    {
        this.delaiCour = (delaiCour * interFrame);
    }

    public void Play()
    {
        idCour = 0;
        spriteR.sprite = frames[idCour];
        delaiCour = 0;
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }
}
