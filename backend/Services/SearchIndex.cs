namespace backend.Services;
using backend.Models;
using backend.Data;
using System.Text.RegularExpressions;

public class SearchIndex
{
    private Dictionary<string,HashSet<int>> _index = new();
    
    private string[] Tokenize(string text)
    {
        var words = Regex.Split(text.ToLower(),@"\W+");
        return words.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
    }

    public void AddDocument(Document document)
    {
        var words = Tokenize(document.Content);
        foreach(string s in words.ToHashSet())
        {
            if(!_index.ContainsKey(s)) _index[s] = new HashSet<int>();
            _index[s].Add(document.Id);
        }
    }

    public HashSet<int> SearchDocuments(string word)
    {
        var words = Tokenize(word);
        if(!_index.ContainsKey(words[0])) return [];
        HashSet<int> result = new(_index[words[0]]);
        foreach(string s in words.ToHashSet())
        {
            if(!_index.ContainsKey(s)) return [];
            result.IntersectWith(_index[s]);
        }
        
        return result;
    }

    public void RemoveDocument(Document document)
    {
        var words = Tokenize(document.Content);
        foreach(var word in words.ToHashSet())
        {
            _index[word].Remove(document.Id);
            if(_index[word].Count==0)
            {
                _index.Remove(word);
            }
        }
    }
}