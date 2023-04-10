using DataAcces;
using DataAcces.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces.Models
{
    public class Jwt
    {
        public readonly ApplicationDbContext _context;
        
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public string Subject { get; set; }

        public  static dynamic validarToken(ClaimsIdentity identity , ApplicationDbContext context)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        succes = false,
                        message = "Verificar si estan enviando un token valido",
                        result = ""
                    };

                }
                var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value; 
                GrupoFamiliar usuario = context.GrupoFamiliar.FirstOrDefault(x => x.Id.ToString() == id);

                return new
                {
                      succes = true,
                      message = "Exitoso",
                      result = usuario
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    succes = false,
                    message = "Catch: "+ ex.Message,
                    result = ""
                };

            }
        }
    }
}
