using DataAcces.Context;
using DataAcces.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ApiToken.Controllers
{
    [Route("Usuario")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _Configuration;


        public UsuarioController (ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;

        }
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(IEnumerable<Login>), StatusCodes.Status200OK)]


        public dynamic IniciarSesion([FromBody] object optData)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            string user = data.user.ToString();
            string password = data.password.ToString();

            GrupoFamiliar usuario = _context.GrupoFamiliar.Where(x => x.Usuario == user && x.Cedula == password).FirstOrDefault();

            if (usuario == null)
            {
                return new
                {
                    succes = false,
                    message = "Credenciales incorrectas",
                    result = ""

                };
            }

            var jwt = _Configuration.GetSection("Jwt").Get<Jwt>();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", usuario.Id.ToString()),
                new Claim("usuario", usuario.Usuario)

            };
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(4),
                signingCredentials: singIn
                );
            return new
            {
                succes = true,
                message = "Exitoso",
                result = new JwtSecurityTokenHandler().WriteToken(token)

            };

        }
    }
}
