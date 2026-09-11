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
    public class RoleController(HeroDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly HeroDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
        private const long MaxImageSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            return Ok(await _context.Roles.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> AddRole([FromForm] RoleCreateDto dto)
        {
            if (dto is null)
                return BadRequest();

            var imageValidation = ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            var role = new Role
            {
                HeroRole = dto.HeroRole,
                    PrimaryFunction = dto.PrimaryFunction,
                KeyAttributes = dto.KeyAttributes
            };

            role.LogoUrl = await SaveImageAsync(dto.ImageFile);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromForm] RoleCreateDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            var imageValidation = ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            role.HeroRole = dto.HeroRole;
            role.PrimaryFunction = dto.PrimaryFunction;
            role.KeyAttributes = dto.KeyAttributes;

            var previousImageUrl = role.LogoUrl;
            if (dto.ImageFile is not null)
            {
                role.LogoUrl = await SaveImageAsync(dto.ImageFile);
                DeleteImage(previousImageUrl);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            DeleteImage(role.LogoUrl);

            _context.Roles.Remove(role);
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

            var uploadDirectory = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "roles");
            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);

            return $"{Request.Scheme}://{Request.Host}/uploads/roles/{fileName}";
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var fileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var uploadDirectory = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "roles");
            var filePath = Path.Combine(uploadDirectory, fileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
    }
}
