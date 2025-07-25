using System.Text.Json.Serialization;

namespace WebApi.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string HeroRole { get; set; }
        public string? LogoUrl { get; set; }
        public string? PrimaryFunction { get; set; }
        public string? KeyAttributes { get; set; }

        [JsonIgnore]
        public List<Hero>? Heroes { get; set; }
    }
}
