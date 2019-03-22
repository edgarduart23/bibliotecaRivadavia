using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Biblioteca.Models
{
    public class BibliotecaContexto: DbContext

    {
        public BibliotecaContexto()
            :base("DefaultConnection")
        {

        }
        public DbSet<Libro> Libros { get; set; }
    }
}