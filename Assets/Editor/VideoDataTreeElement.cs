using System.Collections;
using System.Collections.Generic;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]

public class VideoDataTreeElement : TreeElement
{
    [SerializeField]
    public List<TrainActionLabel> occurences;

    public int display_occurences_count;
    public string display_noun;
    public string display_verb;

    public VideoDataTreeElement(string name, int depth, int id, List<TrainActionLabel> _occurences) : base(name, depth, id)
    {
        occurences = _occurences;
    }
}