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
    public class RoleController(HeroDbContext context) : ControllerBase
    {
        private readonly HeroDbContext _context = context;

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
        public async Task<ActionResult<Role>> AddRole(RoleCreateDto dto)
        {
            if (dto is null)
                return BadRequest();

            var role = new Role
            {
                HeroRole = dto.HeroRole,
                LogoUrl = dto.LogoUrl,
                PrimaryFunction = dto.PrimaryFunction,
                KeyAttributes = dto.KeyAttributes
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleCreateDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            role.HeroRole = dto.HeroRole;
            role.LogoUrl = dto.LogoUrl;
            role.PrimaryFunction = dto.PrimaryFunction;
            role.KeyAttributes = dto.KeyAttributes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
