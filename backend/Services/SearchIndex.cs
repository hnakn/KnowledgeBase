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
        if(!_index.ContainsKey(word)) return [];
        return _index[word];
    }
}