namespace backend.Models.Entities;

public class Board(string id)
{
    public string Id { get; set; } = id;
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddMinutes(30);
    public ICollection<Stroke> Strokes { get; set; } = [];
}
