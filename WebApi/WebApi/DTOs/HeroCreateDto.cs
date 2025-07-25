namespace WebApi.DTOs
{
    public class HeroCreateDto
    {
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public List<int>? RoleIds { get; set; }
    }
}
