using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NarrationReader : MonoBehaviour
{
    public VideoPlayer player;
    public List<AudioClip> narrationClips;
    public List<TextAsset> narrationSubs;
    public List<AudioClip> interludeClips;
    public List<TextAsset> interludeSubs;
    public AudioSource source;
    public List<string> urls;
    int currentIndex;
    int currentInterlude;
    public SubtitleDisplayer subtitleDisplayer;
    GameManager gm;
    // Start is called before the first frame update
    private void Awake() 
    {
        player.targetTexture.Release();
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        player.prepareCompleted += (x)=> PlayCorrespondingAudio();
        player.loopPointReached += (x)=> PlayCorrespondingInterlude();
    }

    public void Resume()
    {
        Play(currentIndex);
    }

    public void Play(int index)
    {
        currentIndex = index;
        player.url = "https://psycho.sandgardeners.com/kitchen/" + urls[currentIndex];
        player.Play();
    }

    public void PlayCorrespondingInterlude()
    {
        source.Stop();
        player.Stop();
        PlayInterlude(currentIndex);
    }

    public void PlayInterlude(int index)
    {
        player.targetTexture.Release();
        currentInterlude = index;
        source.clip = interludeClips[currentInterlude];
        if(gm.subtitles)
            StartCoroutine(subtitleDisplayer.Begin(interludeSubs[currentInterlude]));
        Invoke("TypeStuff", source.clip.length);
        source.Play();
    }

    void PlayCorrespondingAudio()
    {
        if(gm.subtitles)
            StartCoroutine(subtitleDisplayer.Begin(narrationSubs[currentIndex]));
        source.clip = narrationClips[currentIndex];
        source.Play();
    }

    void TypeStuff()
    {
        currentIndex++;
        gm.TypingStuff(currentIndex >= narrationClips.Count);
    }

}
