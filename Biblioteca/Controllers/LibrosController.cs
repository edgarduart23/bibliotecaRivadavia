using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Biblioteca.Models;
using OfficeOpenXml;

namespace Biblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private readonly int _RegistrosPorPagina = 10;

        
        private List<Libro> _Libros;
        private PaginadorGenerico<Libro> _PaginadorLibros;
        private BibliotecaContexto db = new BibliotecaContexto();
        private BibliotecaContexto _DbContext;

        public void ExportToExcel()
        {
            using (_DbContext = new BibliotecaContexto())
            {
                // Recuperamos el 'DbSet' completo
                _Libros = _DbContext.Libros.ToList();
            }
                _Libros.Select(x => new Libro
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Inventario = x.Inventario,
                    Autor = x.Autor,
                    Titulo = x.Titulo,
                    Procedencia = x.Procedencia,
                }).ToList();

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                ws.Cells["A1"].Value = "Biblioteca Rivadavia";
                ws.Cells["B1"].Value = "Listado de libros";

                ws.Cells["A2"].Value = "Report";
                ws.Cells["B2"].Value = "Report1";

                ws.Cells["A3"].Value = "Fecha";
                ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

                ws.Cells["A6"].Value = "Id";
                ws.Cells["B6"].Value = "Fecha";
                ws.Cells["C6"].Value = "Inventario";
                ws.Cells["D6"].Value = "Autor";
                ws.Cells["E6"].Value = "Titulo";
                ws.Cells["F6"].Value = "Procedencia";

                int rowStart = 7;
                foreach (var item in _Libros)
                {
                   

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.Id;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.Fecha.ToString("dd MMMM yyyy hh:mm:ss tt");
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.Inventario;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Autor;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.Titulo;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.Procedencia;
                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();


            }
        
        
        // GET: Libros
        public ActionResult Index(string buscar, int pagina = 1)
        {
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            // FILTRO DE BÚSQUEDA
            using (_DbContext = new BibliotecaContexto())
            {
                // Recuperamos el 'DbSet' completo
                _Libros= _DbContext.Libros.ToList();

                // Filtramos el resultado por el 'texto de búqueda'
                if (!string.IsNullOrEmpty(buscar))
                {
                    foreach (var item in buscar.Split(new char[] { ' ' },
                             StringSplitOptions.RemoveEmptyEntries))
                    {
                        _Libros = _Libros.Where(x => x.Autor.Contains(item) ||
                                                      x.Titulo.Contains(item) ||
                                                      x.Procedencia.Contains(item))
                                                      .ToList();
                    }
                }
            }

            // SISTEMA DE PAGINACIÓN
            using (_DbContext = new BibliotecaContexto())
            {
                // Número total de registros de la tabla Libros
                _TotalRegistros = _Libros.Count();
                // Obtenemos la 'página de registros' de la tabla Libros
                _Libros = _Libros.OrderBy(x => x.Autor)
                                                 .Skip((pagina - 1) * _RegistrosPorPagina)
                                                 .Take(_RegistrosPorPagina)
                                                 .ToList();
                // Número total de páginas de la tabla Libros
                _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

                // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
                _PaginadorLibros = new PaginadorGenerico<Libro>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _TotalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = pagina,
                    BusquedaActual = buscar,
                    Resultado = _Libros
                };
            }

            // Enviamos a la Vista la 'Clase de paginación'
            return View(_PaginadorLibros);
        }
    

           
        

        // GET: Libros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(libro);
        }

        // GET: Libros/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Fecha,Inventario,Ubicacion,Autor,Titulo,Procedencia")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                db.Libros.Add(libro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(libro);
        }

        // GET: Libros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(libro);
        }

        // POST: Libros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Fecha,Inventario,Ubicacion,Autor,Titulo,Procedencia")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(libro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(libro);
        }

        // GET: Libros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libro libro = db.Libros.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Libro libro = db.Libros.Find(id);
            db.Libros.Remove(libro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

   
}
