using DataAcces.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAcces.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<GrupoFamiliar> GrupoFamiliar { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
