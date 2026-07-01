namespace backend.Services;
using backend.Models;
using backend.Data;
using System.Text.RegularExpressions;

public class SearchIndex
{
    private Dictionary<string,WordInfo> _index = new();
    private int IndexedCount { get; set; }
    
    private void UpdateIdf()
    {
        foreach(var word in _index.Keys)
        {
            _index[word].Idf = Math.Log10((double)IndexedCount/_index[word].Frequencies.Count) + 1;
        }

    }
    private string[] Tokenize(string text)
    {
        var words = Regex.Split(text.ToLower(),@"\W+");
        return words.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
    }

    public void AddDocument(Document document)
    {
        IndexedCount++;
        var words = Tokenize(document.Content);
        foreach(string s in words)
        {
            if(!_index.ContainsKey(s))
            {
                _index[s] = new WordInfo();
            } 
            if(!_index[s].Frequencies.ContainsKey(document.Id)) _index[s].Frequencies[document.Id] = 0;
            _index[s].Frequencies[document.Id]++;
        }

        UpdateIdf();
    }


    public List<SearchResult> SearchDocuments(string text)
    {
        var words = Tokenize(text);
        if(words.Length==0 || !_index.ContainsKey(words[0])) return [];
        HashSet<int> result = new(_index[words[0]].Frequencies.Keys);

        var uniqueWords = words.ToHashSet();
        foreach(string s in uniqueWords)
        {
            if(!_index.ContainsKey(s)) return [];
            result.IntersectWith(_index[s].Frequencies.Keys);
        }

        List<SearchResult> rankedResult = new List<SearchResult>();
        foreach(int i in result)
        {
            double score = 0;
            foreach(string s in uniqueWords)
            {
                score += _index[s].Frequencies[i] * _index[s].Idf;
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
        IndexedCount--;
        var words = Tokenize(document.Content).ToHashSet();
        foreach(var word in words)
        {
            _index[word].Frequencies.Remove(document.Id);
            if(_index[word].Frequencies.Count==0)
            {
                _index.Remove(word);
            }
        }

        UpdateIdf();
    }
}