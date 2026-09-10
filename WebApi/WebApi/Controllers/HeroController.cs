using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Models;
using System.Security.Cryptography;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController(HeroDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly HeroDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
        private const long MaxImageSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        [HttpGet]
        public async Task<ActionResult<List<Hero>>> GetHeroes()
        {
            return Ok(await _context.Heroes
                .Include(h => h.Roles)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hero>> GetHeroById(int id)
        {
            var hero = await _context.Heroes.FindAsync(id);
            if (hero is null)
                return NotFound();

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<Hero>> AddHero([FromForm] HeroCreateDto dto)
        {
            if (dto is null)
                return BadRequest();

            var imageValidation = ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            var hero = new Hero
            {
                Name = dto.Name,
                Description = dto.Description
            };

            hero.ImageUrl = await SaveImageAsync(dto.ImageFile);

            if (dto.RoleIds != null && dto.RoleIds.Count > 0)
            {
                var roles = await _context.Roles.Where(r => dto.RoleIds.Contains(r.Id)).ToListAsync();
                hero.Roles = roles;
            }

            _context.Heroes.Add(hero);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHeroById), new { id = hero.Id }, hero);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHero(int id, [FromForm] HeroCreateDto dto)
        {
            var hero = await _context.Heroes.Include(h => h.Roles).FirstOrDefaultAsync(h => h.Id == id);
            if (hero is null)
                return NotFound();

            var imageValidation = ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            hero.Name = dto.Name;
            hero.Description = dto.Description;

            var previousImageUrl = hero.ImageUrl;
            if (dto.ImageFile is not null)
            {
                hero.ImageUrl = await SaveImageAsync(dto.ImageFile);
                DeleteImage(previousImageUrl);
            }

            // Remove all existing roles
            hero.Roles?.Clear();

            // Add new roles from payload
            if (dto.RoleIds != null && dto.RoleIds.Count > 0)
            {
                var roles = await _context.Roles.Where(r => dto.RoleIds.Contains(r.Id)).ToListAsync();
                hero.Roles = roles;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHero(int id)
        {
            var hero = await _context.Heroes.FindAsync(id);
            if (hero is null)
                return NotFound();

            DeleteImage(hero.ImageUrl);

            // Remove all roles from the hero (clears join table entries)
            hero.Roles?.Clear();

            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string? ValidateImage(IFormFile? imageFile)
        {
            if (imageFile is null)
                return null;

            var extension = Path.GetExtension(imageFile.FileName);
            if (!AllowedImageExtensions.Contains(extension))
                return "Only JPG, JPEG, PNG, GIF, and WEBP images are allowed.";

            if (!imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return "The uploaded file must be an image.";

            if (imageFile.Length == 0 || imageFile.Length > MaxImageSize)
                return "The image must be between 1 byte and 5 MB.";

            return null;
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile is null)
                return null;

            var uploadDirectory = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "heroes");
            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);

            return $"{Request.Scheme}://{Request.Host}/uploads/heroes/{fileName}";
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var fileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var uploadDirectory = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "heroes");
            var filePath = Path.Combine(uploadDirectory, fileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
    }
}
