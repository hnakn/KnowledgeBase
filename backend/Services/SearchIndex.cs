namespace backend.Services;
using backend.Models;
using backend.Data;
using System.Text.RegularExpressions;

public class SearchIndex
{
    private Dictionary<string,Dictionary<int,int>> _index = new();
    
    private string[] Tokenize(string text)
    {
        var words = Regex.Split(text.ToLower(),@"\W+");
        return words.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
    }

    public void AddDocument(Document document)
    {
        var words = Tokenize(document.Content);
        foreach(string s in words.ToList())
        {
            if(!_index.ContainsKey(s))
            {
                _index[s] = new Dictionary<int,int>();
            } 
            if(!_index[s].ContainsKey(document.Id)) _index[s][document.Id] = 0;
            _index[s][document.Id]++;
        }
    }

    public List<SearchResult> SearchDocuments(string text)
    {
        var words = Tokenize(text);
        if(words.Length==0 || !_index.ContainsKey(words[0])) return [];
        HashSet<int> result = new(_index[words[0]].Keys);
        foreach(string s in words.ToHashSet())
        {
            if(!_index.ContainsKey(s)) return [];
            result.IntersectWith(_index[s].Keys);
        }

        List<SearchResult> rankedResult = new List<SearchResult>();
        foreach(int i in result)
        {
            var score = 0;
            foreach(string s in words.ToHashSet())
            {
                score += _index[s][i];
            }
            rankedResult.Add(new SearchResult
            {
                DocumentId = i,
                Score = score,
            });
        }
        
        rankedResult.Sort((a,b)=>b.Score.CompareTo(a.Score));
        return rankedResult;
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