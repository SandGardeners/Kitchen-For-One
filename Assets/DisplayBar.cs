using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayBar : MonoBehaviour
{
    TMP_Text text;
    Image img;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        img = transform.parent.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(text.text) && text.color.a != 0f)
        {
            img.enabled = true;
        }
        else
        {
            img.enabled = false;
        }
    }
}
