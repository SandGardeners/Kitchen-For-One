using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class CsvLol
{
    public static List<TrainActionLabel> TALFromText(string text)
    {
        List<TrainActionLabel> results = new List<TrainActionLabel>();
        string[] rows = text.Split('\n');
        foreach(string r in rows)
        {
            string[] datas = r.Split(',');
            TrainActionLabel tal = new TrainActionLabel();
            tal.video_id = datas[0];
            tal.narration = datas[1];
            tal.start_frame = int.Parse(datas[2]);
            tal.stop_frame = int.Parse(datas[3]);
            tal.verb = datas[4];
            tal.verb_class = int.Parse(datas[5]);
            tal.noun = datas[6];
            tal.noun_class = int.Parse(datas[7]);
            results.Add(tal);
        }
        return results;
    }
}
[System.Serializable]
public class TrainActionLabel
{
    public string video_id { get; set; }
    public string narration { get; set; }
    public int start_frame { get; set; }
    public int stop_frame { get; set; }
    public string verb { get; set; }
    public int verb_class { get; set; }
    public string noun { get; set; }
    public int noun_class { get; set; }

    static List<TrainActionLabel> actions;

    public static List<TrainActionLabel> RetrieveFromDataset(string path)
    {
        if(actions == null)
        {
            // StreamReader reader = new StreamReader(path);
            // CsvReader csv = new CsvReader(reader);
            actions = CsvLol.TALFromText(path);
            // actions =  new List<TrainActionLabel>(csv.GetRecords<TrainActionLabel>());
        }
        return actions;
    }
}

[System.Serializable]
public class NounClasses
{
    public int noun_id { get; set; }
    public string class_key { get; set; }
    public string nouns {get; set; }

       static List<NounClasses> instances;

    // public static List<NounClasses> RetrieveFromDataset(string path)
    // {
    //     if(instances == null)
    //     {
    //         StreamReader reader = new StreamReader(path);

    //         CsvReader csv = new CsvReader(reader);
    //         csv.Configuration.Delimiter = "\t";
    //         instances =  new List<NounClasses>(csv.GetRecords<NounClasses>());
    //     }
    //     return instances;
    // }
}

[System.Serializable]
public class VerbClasses
{
    public int verb_id { get; set; }
    public string class_key {get; set;}
    public string verbs {get;set;}
    static List<VerbClasses> instances;

    // public static List<VerbClasses> RetrieveFromDataset(string path)
    // {
    //     if(instances == null)
    //     {
    //         StreamReader reader = new StreamReader(path);

    //         CsvReader csv = new CsvReader(reader);
    //         csv.Configuration.Delimiter = "\t";
    //         instances =  new List<VerbClasses>(csv.GetRecords<VerbClasses>());
    //     }
    //     return instances;
    // }
}