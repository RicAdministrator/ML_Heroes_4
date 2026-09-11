using Microsoft.AspNetCore.Http;

namespace WebApi.DTOs
{
    public class RoleCreateDto
    {
        public string HeroRole { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public string? PrimaryFunction { get; set; }
        public string? KeyAttributes { get; set; }
    }
}
