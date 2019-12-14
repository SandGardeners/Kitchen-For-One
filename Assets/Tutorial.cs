using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class Tutorial : MonoBehaviour
{
    VideoPlayer player;
    public float[] timeMarks;
    public GameObject[] tutoImages;
    int index;
    AudioSource source;
    public AudioSource bgMusic;
    public PostProcessVolume volume;
    public TMP_InputField inputField;
    public float[] tutoMarks;
    public string[] tutoLines;
    private int tutoIndex;

    public Feedback fb;
    GameManager gm;
    private void Awake() {
        gm = FindObjectOfType<GameManager>();
        source = GetComponent<AudioSource>();
        player = GetComponent<VideoPlayer>();
        player.loopPointReached += (x) => 
        {
           Kill();
        }; 
    }

    public void Kill()
    {
        player.Stop();
            player.targetTexture.Release();
            volume.weight = 0f;
            bgMusic.DOFade(0f,1.5f).OnComplete( ()=> 
            {
                if(jumpToEnd)
                    gm.TypingStuff(true);
                else
                    gm.PlayNarration(0);
            });
    }


    public void Begin()
    {
        player.url = "https://psycho.sandgardeners.com/kitchen/output.mp4";
        player.Prepare();
        player.prepareCompleted += ((x)=>
        {
            player.Play();
            volume.weight = 1f;
            bgMusic.DOFade(1f,2.5f);
        });
    }
        bool jumpToEnd;
    public GameObject shortcut;

    public void ValidateInput(string s)
    {
        s = s.ToLower();
        s = s.Trim();
        switch(index)
        {
            case 0:
            {

                List<string> tr = new List<string>{ "turn subtitles on", "turn subtitles off"};
                if(tr.Contains(s))
                {
                    gm.subtitles = (s==tr[0]);
                    fb.GoodFeedback(()=>BackResume());
                }
                else
                {
                  fb.WrongFeedback(()=>Retry());
                }
            break;
            }
            case 1:
            {

                List<string> tr = new List<string>{ "set high quality", "set medium quality", "set low quality"};
                
                if(tr.Contains(s))
                {
                    gm.quality = tr.FindIndex(a => a == s);

                  fb.GoodFeedback(()=>BackResume());
                }
                else
                {
                  fb.WrongFeedback(()=>Retry());
                    
                }
                break;
            }
            case 2:
            {

                if(s == "put down instructions" || (s == "skip thinking" && PlayerPrefs.HasKey("finishedGame")))
                {
                    if(s=="skip thinking")
                        jumpToEnd = true;

                    fb.GoodFeedback(()=>BackResume());
                }
                else
                {
                  fb.WrongFeedback(()=>Retry());

                }
            }
            break;
        }
    }
    
    void Retry()
    {
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }
    
    void BackResume()
    {
        Resume();
        tutoImages[index].SetActive(false);
        index++;
    }

    void Resume()
    {
        inputField.text = string.Empty;
        inputField.gameObject.SetActive(false);
        player.Play();
        bgMusic.DOFade(1f,2.5f);
        DOTween.To(() => volume.weight, x => volume.weight = x, 1f, 1f);
    }

    void ResumeTuto()
    {
        Resume();
        tutoIndex++;
    }

    IEnumerator FillInputFieldLol(string input)
    {
        int strIndex = 0;
        while(strIndex < input.Length)
        {
            inputField.text += input[strIndex++]; 
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(0.25f);
        fb.GoodFeedback(()=>ResumeTuto());
        yield return null;
    }


    void FadeToPause(Action callback)
    {
        bgMusic.DOFade(0.5f,2.5f);
        DOTween.To(() => volume.weight, x => volume.weight = x, 0f, 1f).OnComplete( ()=> callback()); 
    }

    private void Update() {
        if(Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.L))
        {
            Kill();
        }
        if(player.isPlaying && index < timeMarks.Length)
        {
            if(tutoIndex < tutoMarks.Length && player.time >= tutoMarks[tutoIndex])
            {
                FadeToPause(()=>{
                    inputField.gameObject.SetActive(true);
                    inputField.interactable =false;
                    StartCoroutine(FillInputFieldLol(tutoLines[tutoIndex]));
                    // EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                    // inputField.Select();
                });
                player.Pause();
            }
            if(player.time >= timeMarks[index])
            {
                 FadeToPause(()=>{
                    tutoImages[index].SetActive(true); 
                    if(index == 2 && PlayerPrefs.HasKey("finishedGame"))
                    {
                        shortcut.SetActive(true);
                    }
                    inputField.gameObject.SetActive(true);
                    inputField.interactable = true;
                    inputField.ActivateInputField();
                    // EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                    // inputField.Select();
                });
                player.Pause();
            }
        }

    }
}
