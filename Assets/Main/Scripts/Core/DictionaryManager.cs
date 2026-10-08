using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DictionaryManager : MonoBehaviour
{
    // Singleton pattern
    public static DictionaryManager Instance { get; private set; }
    
    // Store all dictionaries
    private Dictionary<string, List<DictionaryEntry>> dictionaries = new Dictionary<string, List<DictionaryEntry>>();
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadAllDictionaries()
    {
        LoadDictionary("tagalog");
        LoadDictionary("cebuano");
        LoadDictionary("ilocano");
        
        Debug.Log("All dictionaries loaded!");
    }
    
    void LoadDictionary(string language)
    {
        // Load JSON file from Resources
        TextAsset jsonFile = Resources.Load<TextAsset>($"Dictionaries/{language}");
        
        if (jsonFile == null)
        {
            Debug.LogError($"Failed to load {language}.json from Resources/Dictionaries/");
            return;
        }
        
        // Parse JSON
        // Note: Unity's JsonUtility requires wrapper for arrays
        string jsonData = "{ \"entries\": " + jsonFile.text + " }";
        DictionaryData data = JsonUtility.FromJson<DictionaryData>(jsonData);
        
        // Store in dictionary
        dictionaries[language.ToLower()] = data.entries;
        
        Debug.Log($"Loaded {data.entries.Count} words for {language}");
    }
    
    // Get random words for a game
    public List<DictionaryEntry> GetRandomWords(string language, int count)
    {
        if (!dictionaries.ContainsKey(language.ToLower()))
        {
            Debug.LogError($"Language {language} not found!");
            return new List<DictionaryEntry>();
        }
        
        List<DictionaryEntry> allWords = dictionaries[language.ToLower()];
        
        // Shuffle and take 'count' words
        return allWords.OrderBy(x => Random.value).Take(count).ToList();
    }
    
    // Get words by length (useful for Wordle)
    public List<DictionaryEntry> GetWordsByLength(string language, int length, int count)
    {
        if (!dictionaries.ContainsKey(language.ToLower()))
        {
            Debug.LogError($"Language {language} not found!");
            return new List<DictionaryEntry>();
        }
        
        List<DictionaryEntry> allWords = dictionaries[language.ToLower()];
        
        // Filter by word length, shuffle, and take 'count' words
        return allWords
            .Where(entry => entry.word.Length == length)
            .OrderBy(x => Random.value)
            .Take(count)
            .ToList();
    }
    
    // Get all words for a language
    public List<DictionaryEntry> GetAllWords(string language)
    {
        if (!dictionaries.ContainsKey(language.ToLower()))
        {
            return new List<DictionaryEntry>();
        }
        
        return dictionaries[language.ToLower()];
    }
}
