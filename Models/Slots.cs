// Shared/Slot.cs
public class Slot
{
    public Guid Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Available { get; set; }
}