using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gma.DataStructures.StringSearch;
using TMPro;

public class InputValidator : MonoBehaviour
{
    
    public GameObject cluePrefab;
    public Transform cluesTransform;
    UkkonenTrie<int> narrationTrie;
    UkkonenTrie<string> verbsTrie;
    UkkonenTrie<string> nounsTrie;

    List<string> narrationStrings;

    Dictionary<string, VerbClasses> verbClassesDico;
    Dictionary<string, NounClasses> nounClassesDico;

    private void Start() {

    }

    private void Update() {
           for(int i = cluesTransform.childCount; i >= 0; i--)
        {
            if(i < cluesTransform.childCount)
            {
     
            }
        }
    }

    public bool CheckForExactItem(string start)
    {
        
        return narrationStrings.Contains(start); 
        
        // IEnumerable<int> results = narrationTrie.Retrieve(start);
        //     foreach(int i in results)
        //     {
        //         string toAdd = string.Empty;

        //         if(narrationStrings[i].StartsWith(start))
        //         {
        //             toAdd = narrationStrings[i];
        //         }
        //         // if(start.StartsWith(splittedNarration[0]))
        //         // {
        //         //     toAdd = (narrationStrings[i]);
        //         // }
        //         // else if(splittedNarration[0].StartsWith(start))
        //         // {
        //         //     toAdd = (splittedNarration[0]);
        //         // }

        //         if(!string.IsNullOrEmpty(toAdd) && !options.Contains(toAdd))
        //         {
        //             options.Add(toAdd);
        //         }
        //     }
    }

    public List<string> CheckForItems<T>(string start, UkkonenTrie<string> trie, Dictionary<string, T> dico, char separator)
    {
        List<string> options = new List<string>();
        
        string[] splittedNarration = start.Split(' ');
        foreach(string s in splittedNarration)
        {
                IEnumerable<string> verbResults = trie.Retrieve(s);
                foreach(string i in verbResults)
                {
                    if(i.StartsWith(s))
                    {
                        string[] trySplit = i.Split(separator);
                        string toAdd = string.Empty;
                        if(trySplit.Length > 1)
                        {
                            bool containsAllParts = true;
                            foreach(string splitted in trySplit)
                            {
                                if(!start.Contains(splitted))
                                {
                                    containsAllParts = false;
                                }
                            }
                            if(containsAllParts)
                            {
                                if(typeof(T) == typeof(VerbClasses))
                                {
                                    toAdd = string.Format("<color=red>{0}</color>",(dico as Dictionary<string, VerbClasses>)[i].class_key);
                                }
                                if(typeof(T) == typeof(NounClasses))
                                {
                                    toAdd = string.Format("<color=blue>{0}</color>",(dico as Dictionary<string, NounClasses>)[i].class_key);
                                }
                            }
                        }
                        else if(start.Contains(i))
                        {
                               if(typeof(T) == typeof(VerbClasses))
                                {
                                    toAdd = string.Format("<color=red>{0}</color>",(dico as Dictionary<string, VerbClasses>)[i].class_key);
                                }
                                if(typeof(T) == typeof(NounClasses))
                                {
                                    toAdd = string.Format("<color=blue>{0}</color>",(dico as Dictionary<string, NounClasses>)[i].class_key);
                                }
                        }

                        if(!string.IsNullOrEmpty(toAdd) && !options.Contains(toAdd))
                        {
                            options.Add(toAdd);
                        }
                    }
                }
            }

            return options;


    }

    public void DisplayAllNarration(string start)
    {
        List<string> options = new List<string>();

        for(int i = cluesTransform.childCount; i >= 0; i--)
        {
            if(i < cluesTransform.childCount)
            {
                Destroy(cluesTransform.GetChild(i).gameObject);
            }
        }

        if(CheckForExactItem(start))
        {
            options.Add(string.Format("<color=green>{0}</color>",start));
        }
        else
        {
            options.AddRange(CheckForItems<VerbClasses>(start, verbsTrie,verbClassesDico,'-'));
            options.AddRange(CheckForItems<NounClasses>(start, nounsTrie, nounClassesDico, ':'));    
        }
            
        

        foreach(string option in options)
        {
            GameObject go = Instantiate(cluePrefab, Vector3.zero, Quaternion.identity, cluesTransform);
                RectTransform tr =go.GetComponent<RectTransform>();
                Vector3 p = tr.localPosition;
                p.z = 0f;
                tr.localPosition = p;
            go.GetComponent<TMP_Text>().text = option;

        }
    }
}
