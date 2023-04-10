using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces.Models
{
    public class GrupoFamiliar
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Genero { get; set; }
        public string Parentesco { get; set; }
        public int Edad { get; set; }
        public bool MenorEdad { get; set; }
        public DateTime? FechaNacimiento { get; set; }

    }


}
