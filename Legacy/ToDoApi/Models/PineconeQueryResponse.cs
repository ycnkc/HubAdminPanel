public class PineconeQueryResponse
{
    public List<PineconeMatch> Matches { get; set; } = new();
}

public class PineconeMatch
{
    public string Id { get; set; } = string.Empty;
    public float Score { get; set; } 
}