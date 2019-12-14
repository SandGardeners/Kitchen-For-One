using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using System.IO;
using System;

public class VideoPlayerWindow : EditorWindow
{

    bool isInit;
    void Init()
    {
        texture = new RenderTexture(1280, 720, 16, RenderTextureFormat.ARGB32);
        go = new GameObject("VideoPlayer", typeof(VideoPlayer));
        go.hideFlags = HideFlags.HideAndDontSave;
        player = go.GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.isLooping = false;
        player.targetTexture = texture;
        isInit = true;
    }

    [MenuItem("Kitchen/Video Player")]
    public static VideoPlayerWindow GetWindow()
    {
        var window = GetWindow<VideoPlayerWindow>();
        window.Init();
        window.Show();
        window.Focus();
        window.Repaint();
        return window;
    }

    
    bool multipleClips;
    List<TrainActionLabel> currentClips;
    int clipIndex;
    TrainActionLabel current;
    public void PlayClips(List<TrainActionLabel> occurences)
    {
        multipleClips = true;
        currentClips = occurences;
        clipIndex = 0;
        PlayClip(currentClips[clipIndex++]);
    }
    public void PlayClip(TrainActionLabel d)
    {
        if (!isInit)
            Init();

        string path = Application.streamingAssetsPath + "/processed" + d.video_id + "_compressed_sobel_bw.mp4";
        if (!System.IO.File.Exists(path))
        {
            path = "http://psycho.sandgardeners.com/kitchen/processed" + d.video_id + "_compressed_sobel_bw.mp4";
        }
        current = d;
        player.url = path;
        player.Play();
        targetFrame = d.stop_frame;
        player.frame = d.start_frame;
    }

    VideoPlayer player;
    RenderTexture texture;
    GameObject go;
    float targetFrame;
    void OnGUI()
    {
        if(!isInit)
        {
            Init();
        }
        else
        {
            GUILayout.BeginArea(new Rect(20, 10, position.width-20, 80));
            if(current != null)
                GUILayout.Label("Currently playing : " + current.video_id);
            else
                GUILayout.Label("None playing");
            GUILayout.EndArea();

            EditorGUI.DrawPreviewTexture(new Rect(0, 90,Mathf.Min(position.width,1280), Mathf.Min(position.height,1080)), texture, null, ScaleMode.ScaleToFit);
            if(player != null && player.frame >= targetFrame)
            {
                player.Stop();
                if(multipleClips && clipIndex < currentClips.Count)
                {
                    PlayClip(currentClips[clipIndex++]);
                }
                else
                {    
                    current = null;
                    texture.Release();
                }
            }
            Repaint();
        }
    }


    void OnDestroy()
    {
        DestroyImmediate(go);
        texture.Release();
        DestroyImmediate(texture);
    }

}
