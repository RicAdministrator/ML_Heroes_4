using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(HeroDbContext context, ImageFileService imageFileService) : ControllerBase
    {
        private readonly HeroDbContext _context = context;
        private readonly ImageFileService _imageFileService = imageFileService;

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

            var imageValidation = _imageFileService.ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            var role = new Role
            {
                HeroRole = dto.HeroRole,
                    PrimaryFunction = dto.PrimaryFunction,
                KeyAttributes = dto.KeyAttributes
            };

            role.LogoUrl = await _imageFileService.SaveImageAsync(dto.ImageFile, Request, "roles");

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

            var imageValidation = _imageFileService.ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            role.HeroRole = dto.HeroRole;
            role.PrimaryFunction = dto.PrimaryFunction;
            role.KeyAttributes = dto.KeyAttributes;

            var previousImageUrl = role.LogoUrl;
            if (dto.ImageFile is not null)
            {
                role.LogoUrl = await _imageFileService.SaveImageAsync(dto.ImageFile, Request, "roles");
                _imageFileService.DeleteImage(previousImageUrl, "roles");
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

            _imageFileService.DeleteImage(role.LogoUrl, "roles");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
