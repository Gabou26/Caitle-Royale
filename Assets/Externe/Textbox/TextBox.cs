using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    [TextArea]
    public string message = "";
    TMP_Text textbox;
    bool ecrit = false;
    AudioSource son;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        son = GetComponent<AudioSource>();
        textbox = GetComponentInChildren<TMP_Text>();
        image = GetComponentInChildren<Image>();
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Activer()
    {
        StartCoroutine("Ecrire");
        ecrit = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !ecrit)
        {
            Activer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !ecrit)
        {
            Activer();
        }
    }

    IEnumerator Ecrire()
    {
        textbox.text = "";
        image.enabled = true;
        textbox.enabled = true;
        for (int i = 0; i <= message.Length; i++)
        {
            textbox.text = message.Substring(0, i);
            son.Play();
            yield return new WaitForSeconds(0.04f);
        }
    }
}
