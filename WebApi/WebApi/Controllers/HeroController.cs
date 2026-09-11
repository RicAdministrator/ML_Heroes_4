using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class HeroController(HeroDbContext context, ImageFileService imageFileService) : ControllerBase
    {
        private readonly HeroDbContext _context = context;
        private readonly ImageFileService _imageFileService = imageFileService;

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

            var imageValidation = _imageFileService.ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            var hero = new Hero
            {
                Name = dto.Name,
                Description = dto.Description
            };

            hero.ImageUrl = await _imageFileService.SaveImageAsync(dto.ImageFile, Request, "heroes");

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

            var imageValidation = _imageFileService.ValidateImage(dto.ImageFile);
            if (imageValidation is not null)
                return BadRequest(imageValidation);

            hero.Name = dto.Name;
            hero.Description = dto.Description;

            var previousImageUrl = hero.ImageUrl;
            if (dto.ImageFile is not null)
            {
                hero.ImageUrl = await _imageFileService.SaveImageAsync(dto.ImageFile, Request, "heroes");
                _imageFileService.DeleteImage(previousImageUrl, "heroes");
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

            _imageFileService.DeleteImage(hero.ImageUrl, "heroes");

            // Remove all roles from the hero (clears join table entries)
            hero.Roles?.Clear();

            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
