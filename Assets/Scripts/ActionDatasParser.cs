using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using System.IO;

public class ActionDatasParser : MonoBehaviour
{
    public int howManyActions = 10;
    int actionsCount = 0;
    public string datasetName;
    List<string> narrationStrings;
    Dictionary<string, List<VideoData>> videoDictionary;

    public TMP_InputField inputField;
    public VideoPlayer player;

    public bool local;

    public PostProcessVolume ppvolume;
    public AudioSource bgMusic;

    public void Activate(bool infiniteMode = false)
    {

        inputField.gameObject.SetActive(true);
        inputField.interactable = true;
        inputField.ActivateInputField();
        if(infiniteMode)
        {
            howManyActions = -1;
        }
        actionsCount = 0;
        bgMusic.DOFade(0.5f,2.5f);
        available =true;
        // DOTween.To(()=>ppvolume.weight, x=>ppvolume.weight=x, 1f, 2.5f);

    }

    public void Deactivate()
    {
        inputField.gameObject.SetActive(false);
        bgMusic.DOFade(0f,2.5f);

    }
    float timer;

    public float minimumDuration;
    int minimumFrames
    {
        get
        {
            return (int)(minimumDuration*player.frameRate);
        }
    }

    public Feedback fb;

    AudioSource source;
    public int targetStopFrame;
    double targetStopTime;
    double targetStartTime;
    public void PreparationComplete()
    {
        Debug.Log(Time.time - timer); 

        targetStartTime = currentVideoData.start_time;
        targetStopTime = currentVideoData.stop_time;

        double duration = targetStopTime-targetStartTime;
        if(duration < minimumDuration)
        {

            double difference = minimumDuration-duration;
            // Debug.Log(difference);
            targetStartTime = Mathf.Max(0,(float)(targetStartTime-difference*0.5));
            // Debug.Log(targetStartTime);
            targetStopTime = Mathf.Min(player.frameCount*player.frameRate,(float)(targetStopTime+difference*0.5));
            // Debug.Log(targetStopTime);
        }

        player.time = targetStartTime;
        player.Play();
        
        // targetStartTime = targetStartFrame/player.frameRate;

        // targetStopFrame = currentVideoData.stop_frame;
        // int frameDuration = targetStopFrame-currentVideoData.start_frame; 
        // Debug.Log(string.Format("frame duration : {0} ", frameDuration));
        // if(frameDuration < minimumFrames)
        // {
        //     int difference = minimumFrames-frameDuration;
        //     Debug.Log(string.Format("difference : {0} - {1} = {2}", minimumFrames,frameDuration,difference));
        //     targetStopFrame = (int)Mathf.Min(player.frameCount, targetStopFrame+difference*0.5f); 
        //     Debug.Log(string.Format("new stop frame : {0} (previously {1})", targetStopFrame,currentVideoData.stop_frame));

        // }
        // targetStopTime = targetStopFrame/player.frameRate;
    }
    public List<AudioClip> drumkits;
    GameManager manager;
    public TextAsset dataset;
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
  source = GetComponent<AudioSource>();

        List<TrainActionLabel> actions = TrainActionLabel.RetrieveFromDataset(dataset.text);
        
        
        narrationStrings = new List<string>();
        videoDictionary = new Dictionary<string, List<VideoData>>();
        player.prepareCompleted += (x) => {
            PreparationComplete();};
            
        int count = 0;
        foreach(TrainActionLabel act in actions)
        {

                        if (!narrationStrings.Contains(act.narration))
                {
                    narrationStrings.Add(act.narration);
                }
         
                VideoData data = new VideoData();
                data.video_id = act.video_id;
                data.start_time = act.start_frame/60.0;
                data.stop_time = act.stop_frame/60.0;
                data.unique_id = count;
                data.narration = act.narration.ToLower().Trim();
                count++;

            if(act.narration == act.verb || act.verb == "video" || act.noun == "video" || act.narration.Contains("video") || act.narration.Contains("recording"))
            {
               // Debug.Log("SKIPPED " + act.narration);
            }
            else
            {    
                if(!videoDictionary.ContainsKey(data.narration))
                                videoDictionary[data.narration] = new List<VideoData>();
                            
                            videoDictionary[data.narration].Add(data);
            }
             
        }
    }
    

    private void Update() {

        if(currentVideoData != null && player.isPlaying && player.frame != -1)
        {
            // Debug.Log(targetStartTime);
            // Debug.Log(player.time);
            // Debug.Log(targetStopTime);
            if(player.time >= targetStopTime || player.time < targetStartTime-minimumDuration)
            {
                currentVideoData = null;
                player.targetTexture.Release();
                ppvolume.weight = 0;

                if(howManyActions < 0 || actionsCount < howManyActions)
                {
                    bgMusic.DOFade(0.5f,1.5f);
                    player.Stop();
                    inputField.gameObject.SetActive(true);
                    inputField.interactable = true;
                    inputField.ActivateInputField();
                }
                else
                {
                    FindObjectOfType<GameManager>().ResumeNarration();
                }
            }
        }

        // if(available && inputField.isFocused && Input.inputString != string.Empty)
        // {
        //         char key = Input.inputString.ToLower()[Input.inputString.Length - 1];
                
        //         if(key >= 'a' && key <= 'z')
        //         {
        //             source.PlayOneShot(drumkits[(int)key - (int)'a']);
        //         }
        //         else if(key == ' ')
        //     {
        //         source.PlayOneShot(drumkits[drumkits.Count - 1]);
        //     }
            
        // }
    }
    VideoData currentVideoData;
    bool available = true;
    int targetStartFrame;
    public void TryDisplayVideo()
    {
        if(available)
        {
            available = false;
            inputField.interactable = false;
            inputField.text = inputField.text.ToLower().Trim();
            if(videoDictionary.ContainsKey(inputField.text))
            {
                List<VideoData> allDatasAvailable = videoDictionary[inputField.text];
                currentVideoData = allDatasAvailable[Random.Range(0, allDatasAvailable.Count)];
                manager.SendEntry(currentVideoData.unique_id);
              //  Debug.Log(allDatasAvailable.Count);
                string path;
                if (local)
                    path = Application.streamingAssetsPath + "/processed" + currentVideoData.video_id + "_compressed_sobel_bw.mp4";
                else
                {
                    path = "https://psycho.sandgardeners.com/kitchen/processed" + currentVideoData.video_id + "_compressed_sobel_bw";
                    if(manager.quality == 1)
                    {
                        path += "_md";
                    }
                    else if(manager.quality == 2)
                    {
                        path += "_sd";
                    }
                    path += ".mp4";
                }
                player.url = path;
                Debug.Log(path);
                timer = Time.time;
                fb.GoodFeedback(
                    () =>
                    {
                        available = true;
                        inputField.gameObject.SetActive(false);

                      
                     //   player.Play();

                        actionsCount++;
                                bgMusic.DOFade(1f,1.5f);
                        // targetStartFrame = currentVideoData.start_frame;
                        // int frameDuration = currentVideoData.stop_frame-targetStartFrame; 
                        // Debug.Log(string.Format("frame duration : {0} ", frameDuration));
                        // if(frameDuration < minimumFrames)
                        // {
                        //     int difference = minimumFrames-frameDuration;
                        //  Debug.Log(string.Format("difference : {0} - {1} = {2}", minimumFrames,frameDuration,difference));
                        //     targetStartFrame = (int)Mathf.Max(0,targetStartFrame-difference*0.5f); 
                        //   Debug.Log(string.Format("new start frame : {0} (previously {1})", targetStartFrame,currentVideoData.start_frame));
                        // }
                        ppvolume.weight = 1f;
                        player.Prepare();
                        inputField.text = string.Empty;
                    });
            }
            else
            {
                fb.WrongFeedback(
                    () =>
                    {
                        inputField.interactable = true;
                        inputField.ActivateInputField();
                        available = true;
                    });
            }
        }
    }


}
