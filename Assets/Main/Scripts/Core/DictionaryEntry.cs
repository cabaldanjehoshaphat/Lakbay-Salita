using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]

public class DictionaryEntry : MonoBehaviour
{
    public string word;
    public string definition;
    public string partOfSpeech;
    
    // Constructor
    public DictionaryEntry(string word, string definition, string partOfSpeech = "")
    {
        this.word = word;
        this.definition = definition;
        this.partOfSpeech = partOfSpeech;
    }
    
}

[Serializable]
public class DictionaryData
{
    public List<DictionaryEntry> entries;
}
