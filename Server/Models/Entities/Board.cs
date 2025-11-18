namespace backend.Models.Entities;

public class Board(Guid id)
{
    public Guid Id { get; set; } = id;
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddMinutes(30);
    public ICollection<Stroke> Strokes { get; set; } = [];
}
