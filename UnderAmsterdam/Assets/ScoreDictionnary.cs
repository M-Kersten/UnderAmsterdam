using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDictionnary", menuName = "ScriptableObjects/ScoreDictionnary", order = 1)]
public class ScoreDictionnary : ScriptableObject
{
    //public Dictionary<string, int> Dict = new Dictionary<string, int>();
    public string[] companies = new string[5];
    public int[] scores = new int[5];
    public int Count = 0;

    public void Add(string company, int score)
    {
        companies[Count] = company;
        scores[Count++] = score;
    }

    public void Clear()
    {
        companies = new string[5];
        scores = new int[5];
    }

    public void Sort()
    {
        int i = 0;
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (i = 0; i < Count; i++) dict.Add(companies[i], scores[i]);
        dict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        i = 0;
        foreach (var data in dict)
        {
            companies[i] = data.Key;
            scores[i++] = data.Value;
        }
    }

}
