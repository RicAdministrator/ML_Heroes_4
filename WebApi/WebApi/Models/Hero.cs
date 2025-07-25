namespace WebApi.Models
{
    public class Hero
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public List<Role>? Roles { get; set; }
    }
}
