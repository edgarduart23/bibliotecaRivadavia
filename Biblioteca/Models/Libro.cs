using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int Inventario { get; set; }
        public string Ubicacion { get; set; }
        public string Autor { get; set; }
        public string Titulo { get; set; }
        public string Procedencia { get; set; }
    }
}