namespace backend.Models;

public class WordInfo
{
    public Dictionary<int,int> Frequencies { get; } = new();
    public double Idf { get; set; }
}