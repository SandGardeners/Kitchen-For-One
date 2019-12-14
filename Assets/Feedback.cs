using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class Feedback : MonoBehaviour
{
    TMP_Text text;
    public Color wrongColor;
    Color startColor;
    public List<AudioClip> drumKits;
    AudioSource source;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        text = GetComponent<TMP_Text>();
        startColor = text.color;
    }

    public void KeyFeedback(string text)
    {
        //source.PlayOneShot(drumKits[UnityEngine.Random.Range(0, drumKits.Count)]);
        if(!string.IsNullOrEmpty(text) && text.Length > 0)
        {

            char key =text.ToLower()[text.Length - 1];
                
                if(key >= 'a' && key <= 'z')
                {
                    source.PlayOneShot(drumKits[(int)key - (int)'a']);
                }
                else if(key == ' ')
            {
                source.PlayOneShot(drumKits[drumKits.Count - 1]);
            }
        }
    }

    [ContextMenu("Good")]
    public void GoodFeedback(Action callback)
    {
        text.DOFade(1f, 0.25f).OnComplete(
            () =>
            {
                text.DOFade(0f, 0.5f).OnComplete(
                    () =>
                    {
                        callback();
                        text.color = startColor;
                    });
            });
    }
    [ContextMenu("Wrong")]
    public void WrongFeedback(Action callback)
    {
        text.DOColor(wrongColor, 0.75f);
        text.transform.DOShakePosition(0.75f, 3.5f, 50).OnComplete(
            () =>
            {
                callback();
                text.color = startColor;
            });
    }
}
