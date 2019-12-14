using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entry
{
    public int player_id;
    public int source_id;
}

public class PlayerID
{
    public int player_id;
}

public class GameManager : MonoBehaviour
{
    public NarrationReader reader;
    public ActionDatasParser parser;
    public Tutorial tutorial;

    public GameObject panel;

    public bool subtitles;
    public int quality;

    void DisableAll(GameObject but)
    {
        panel.gameObject.SetActive(false);

        if(parser.gameObject != but)
        {
            parser.Deactivate();
            parser.gameObject.SetActive(false);
        }

        if(tutorial.gameObject != but)
        {
            tutorial.gameObject.SetActive(false);
        }
    }

    private void Awake() {
  
    }

    bool triggered = false;
    private void Update() {
        if(!triggered && !Application.isEditor)
        {    
            triggered = true;
            PlayTutorial();
        }
    }

    public void SendEntry(int sourceID)
    {
  
    }

    public void PlayTutorial()
    {
        DisableAll(tutorial.gameObject);
        tutorial.gameObject.SetActive(true);
        tutorial.Begin();
    }

    public void ResumeNarration()
    {
        DisableAll(reader.gameObject);
        reader.gameObject.SetActive(true);
        reader.Resume();
    }

    public void PlayNarration(int index)
    {
        DisableAll(reader.gameObject);
        reader.gameObject.SetActive(true);
        reader.Play(index);
    }

    public void PlayInterlude(int index)
    {
        DisableAll(reader.gameObject);
        reader.gameObject.SetActive(true);
        reader.PlayInterlude(index);
    }

    public void TypingStuff(bool infinite = false)
    {
        if(infinite)
        {
            PlayerPrefs.SetInt("finishedGame",1);
        }
        DisableAll(parser.gameObject);
        parser.gameObject.SetActive(true);
        parser.Activate(infinite);
    }
}
