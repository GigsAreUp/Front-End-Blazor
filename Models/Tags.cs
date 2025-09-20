namespace MusicHFE2.Models
{
    // Models/Tag.cs
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<UserTag> UserTags { get; set; } = new List<UserTag>();
    }

    // Models/UserTag.cs
    public class UserTag
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = new User();
        public Tag Tag { get; set; } = new Tag();
    }
}
