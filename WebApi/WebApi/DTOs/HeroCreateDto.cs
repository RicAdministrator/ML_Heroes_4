using Microsoft.AspNetCore.Http;

namespace WebApi.DTOs
{
    public class HeroCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        public List<int>? RoleIds { get; set; }
    }
}
