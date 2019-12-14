using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;



    static class VideoDataTreeGenerator
    {
        static int IDCounter;

        public struct UniqueActKey
        {
            public int verbClass;
            public int nounClass;
        }

        public static List<VideoDataTreeElement> GenreateVideoDataTree()
        {
            var treeElements = new List<VideoDataTreeElement>();
        //     IDCounter = 0;
        //     IEnumerable<TrainActionLabel> actions = new List<TrainActionLabel>();

        //     StreamReader videosReader = new StreamReader(Application.dataPath + "/Datasets/final.csv");
        //     StreamReader nounsReader = new StreamReader(Application.dataPath + "/Datasets/noun_classes.tsv");
        //     StreamReader verbsReader = new StreamReader(Application.dataPath + "/Datasets/verb_classes.tsv");
        //     // CsvReader videosCsv = new CsvReader(videosReader);
        //     // CsvReader nounsCsv = new CsvReader(nounsReader);
        //     // CsvReader verbsCsv = new CsvReader(verbsReader);
        //     nounsCsv.Configuration.Delimiter = "\t";
        //     verbsCsv.Configuration.Delimiter = "\t";
        //     actions = videosCsv.GetRecords<TrainActionLabel>();
        //     List<NounClasses> nouns = new List<NounClasses>(nounsCsv.GetRecords<NounClasses>());
        //     List<VerbClasses> verbs = new List<VerbClasses>(verbsCsv.GetRecords<VerbClasses>());

        //     Dictionary<UniqueActKey, Dictionary<string, List<TrainActionLabel>>> uniqueActDico = new Dictionary<UniqueActKey, Dictionary<string, List<TrainActionLabel>>>(); 
        //     List<string> narrationStrings = new List<string>();
        
        //     foreach (TrainActionLabel act in actions)
        //     {
        //         if (!narrationStrings.Contains(act.narration))
        //         {
        //             narrationStrings.Add(act.narration);
        //         }
        //         act.start_frame = act.start_frame / 2;
        //         act.stop_frame = act.stop_frame / 2;

        //         UniqueActKey key = new UniqueActKey {verbClass = act.verb_class, nounClass = act.noun_class};

        //     Dictionary<string, List<TrainActionLabel>> narrationDico = null;
        //     if(!uniqueActDico.ContainsKey(key))
        //     {
        //         uniqueActDico[key] = narrationDico = new Dictionary<string, List<TrainActionLabel>>();
        //     }
        //     else
        //     {
        //         narrationDico = uniqueActDico[key];
        //     }

        //     if (!narrationDico.ContainsKey(act.narration))
        //         narrationDico[act.narration] = new List<TrainActionLabel>();
        //     else
        //     {
        //         if(narrationDico[act.narration][0].verb != act.verb)
        //             Debug.LogError(string.Format("Mismatching {0} & {1} - {2} - {3} vs {4} - {5}", narrationDico[act.narration][0].narration, act.narration, narrationDico[act.narration][0].verb, narrationDico[act.narration][0].noun, act.verb, act.noun));       
        //     }

        //     narrationDico[act.narration].Add(act);
        // }

 
        //     var root = new VideoDataTreeElement("Root", -1, IDCounter++, null);
        //     treeElements.Add(root);
        //     Debug.Log(uniqueActDico.Count);
        //     foreach(var keyKVP in uniqueActDico)
        //     {
        //         var uniqueElement = new VideoDataTreeElement(string.Format("{0} {1} [{2},{3}]", verbs[keyKVP.Key.verbClass].class_key, nouns[keyKVP.Key.nounClass].class_key,keyKVP.Key.verbClass, keyKVP.Key.nounClass), 0, IDCounter++, null);
        //          int nb = 0;
        //         foreach(var child in keyKVP.Value)
        //         {
                    
        //                 nb += (child.Value.Count);
        //             }
                
        //         uniqueElement.display_occurences_count = nb;
        //         uniqueElement.display_noun = nouns[keyKVP.Key.nounClass].nouns;
        //         uniqueElement.display_verb = verbs[keyKVP.Key.verbClass].verbs;
        //         treeElements.Add(uniqueElement);

        //         foreach(var narrationKVP in keyKVP.Value)
        //         {
        //             var narrationElement = new VideoDataTreeElement(narrationKVP.Key, 1, IDCounter++, narrationKVP.Value);
        //               string value = narrationKVP.Value[0].noun;
        //             foreach(var d in narrationKVP.Value)
        //             {
        //                 if(d.noun != value)
        //                 {
        //                     value = "Multiple";
        //                     break;
        //                 }
        //             }
        //             narrationElement.display_noun = value;
        //             value = narrationKVP.Value[0].verb;
        //             foreach(var d in narrationKVP.Value)
        //             {
        //                 if(d.verb != value)
        //                 {
        //                     value = "Multiple";
        //                     break;
        //                 }
        //             }
        //             narrationElement.display_verb = value;
        //             narrationElement.display_occurences_count = narrationKVP.Value.Count;

        //             treeElements.Add(narrationElement);
        //             foreach(TrainActionLabel vd in narrationKVP.Value)
        //             {
        //                 List<TrainActionLabel> occ = new List<TrainActionLabel>();
        //                 occ.Add(vd);
        //                 var el = new VideoDataTreeElement(vd.video_id, 2, IDCounter++, occ);
        //                 el.display_noun = vd.noun;
        //                 el.display_verb = vd.verb;
        //                 el.display_occurences_count = -1;
        //                 treeElements.Add(el);
        //             }
        //         }
        //     }

        return treeElements;
        }
        //static void AddChildrenRecursive(UnityEditor.TreeViewExamples.TreeElement element, int numChildren, bool force, int numTotalElements, ref int allowedDepth, List<VideoDataTreeElement> treeElements)
        //{
        //    if (element.depth >= allowedDepth)
        //    {
        //        allowedDepth = 0;
        //        return;
        //    }

        //    for (int i = 0; i < numChildren; ++i)
        //    {
        //        if (IDCounter > numTotalElements)
        //            return;

        //        var child = new VideoDataTreeElement("Element " + IDCounter, element.depth + 1, ++IDCounter);
        //        treeElements.Add(child);

        //        if (!force && Random.value < probabilityOfBeingLeaf)
        //            continue;

        //        AddChildrenRecursive(child, Random.Range(minNumChildren, maxNumChildren), false, numTotalElements, ref allowedDepth, treeElements);
        //    }
        //}
    }

