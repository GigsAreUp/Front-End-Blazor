namespace MusicHFE2.Models
{
    // Shared/Order.cs
    public class Order
    {
        public int Id { get; set; }
        public string Seller { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}
