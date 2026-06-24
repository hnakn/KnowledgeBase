namespace backend.Services;
using backend.Models;
using backend.Data;

public class SearchIndex
{
    private Dictionary<string,HashSet<int>> _index = new();

    public void AddDocument(Document document)
    {
        foreach(string s in document.Content.ToLower().Split())
        {
            if(!_index.ContainsKey(s)) _index[s] = new HashSet<int>();
            _index[s].Add(document.Id);
        }
    }

    public HashSet<int> SearchDocuments(string word)
    {
        string[] words = word.Split();
        if(!_index.ContainsKey(words[0])) return [];
        HashSet<int> result = new(_index[words[0]]);
        foreach(string s in words)
        {
            if(!_index.ContainsKey(s)) return [];
            result.IntersectWith(_index[s]);
        }
        
        return result;
    }

    public void RemoveDocument(Document document)
    {
        foreach(var word in document.Content.ToLower().Split().ToHashSet())
        {
            _index[word].Remove(document.Id);
            if(_index[word].Count==0)
            {
                _index.Remove(word);
            }
        }
    }
}