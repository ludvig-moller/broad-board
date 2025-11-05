namespace backend.Models.Entities;

public class Board(string id)
{
    public string Id { get; set; } = id;
    public ICollection<Stroke> Strokes { get; set; } = [];
}
