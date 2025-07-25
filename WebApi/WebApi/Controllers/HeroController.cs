using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController(HeroDbContext context) : ControllerBase
    {
        private readonly HeroDbContext _context = context;

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
        public async Task<ActionResult<Hero>> AddHero(HeroCreateDto dto)
        {
            if (dto is null)
                return BadRequest();

            var hero = new Hero
            {
                Name = dto.Name,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description
            };

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
        public async Task<IActionResult> UpdateHero(int id, HeroCreateDto dto)
        {
            var hero = await _context.Heroes.Include(h => h.Roles).FirstOrDefaultAsync(h => h.Id == id);
            if (hero is null)
                return NotFound();

            hero.Name = dto.Name;
            hero.ImageUrl = dto.ImageUrl;
            hero.Description = dto.Description;

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

            // Remove all roles from the hero (clears join table entries)
            hero.Roles?.Clear();

            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
