using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAcces.Context;
using DataAcces.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace ApiToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoFamiliarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GrupoFamiliarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GrupoFamiliars
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GrupoFamiliar>>> GetGrupoFamiliar()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var user = Jwt.validarToken(identity, _context);
                if (user == null)
                {
                    return Unauthorized("Invalid token");
                }
                var grupoFamiliar = await _context.GrupoFamiliar.ToListAsync();
                return Ok(grupoFamiliar);
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(ex, "An error occurred while executing the GetGrupoFamiliar method");

                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        // GET: api/GrupoFamiliars/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GrupoFamiliar>> GetGrupoFamiliar(int id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var user = Jwt.validarToken(identity, _context);
                if (user == null)
                {
                    return Unauthorized("Invalid token");
                }
                var grupoFamiliar = await _context.GrupoFamiliar.FindAsync(id);

                if (grupoFamiliar == null)
                {
                    return NotFound();
                }

                return grupoFamiliar;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching the grupo familiar with id {id}", id);
                return StatusCode(500, "An error occurred while fetching the grupo familiar");
            }
        }

        // PUT: api/GrupoFamiliars/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrupoFamiliar(int id, GrupoFamiliar grupoFamiliar)
        {
            if (id != grupoFamiliar.Id)
            {
                return BadRequest();
            }

            _context.Entry(grupoFamiliar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrupoFamiliarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GrupoFamiliars
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GrupoFamiliar>> PostGrupoFamiliar(GrupoFamiliar grupoFamiliar)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var user = Jwt.validarToken(identity, _context);
                if (user == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Validar campos requeridos
                if (string.IsNullOrEmpty(grupoFamiliar.Usuario) || string.IsNullOrEmpty(grupoFamiliar.Cedula)
                    || string.IsNullOrEmpty(grupoFamiliar.Nombres) || string.IsNullOrEmpty(grupoFamiliar.Apellidos)
                    || grupoFamiliar.Edad == 0)
                {
                    return BadRequest("Missing required fields");
                }

                // Validar fecha de nacimiento si es menor de edad
                if (grupoFamiliar.Edad < 18 && grupoFamiliar.FechaNacimiento == null)
                {
                    return BadRequest("Missing required fields");
                }

                // Validar si ya existe el registro
                var existente = await _context.GrupoFamiliar
                    .Where(gf => gf.Cedula == grupoFamiliar.Cedula && gf.Usuario == grupoFamiliar.Usuario)
                    .FirstOrDefaultAsync();
                if (existente != null)
                {
                    return BadRequest("El usuario ya se encuentra registrado.");
                }

                // Asignar el valor del campo MenorEdad
                grupoFamiliar.MenorEdad = grupoFamiliar.Edad < 18;
                _context.GrupoFamiliar.Add(grupoFamiliar);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetGrupoFamiliar", new { id = grupoFamiliar.Id }, grupoFamiliar);
            }
            catch (Exception ex)
            {
                // Guardar en el log
                Log.Error($"Error en el método PostGrupoFamiliar: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // DELETE: api/GrupoFamiliars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrupoFamiliar(int id)
        {
            var grupoFamiliar = await _context.GrupoFamiliar.FindAsync(id);
            if (grupoFamiliar == null)
            {
                return NotFound();
            }

            _context.GrupoFamiliar.Remove(grupoFamiliar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrupoFamiliarExists(int id)
        {
            return _context.GrupoFamiliar.Any(e => e.Id == id);
        }
    }
}
