using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoRepositorio _eventoRepo;

        public EventosController(EventoRepositorio eventoRepo)
        {
            _eventoRepo = eventoRepo;
        }

        // GET: Eventos
        public async Task<IActionResult> Index(int? categoriaId)
        {
            var eventos = await _eventoRepo.ObtenerTodosActivosAsync(categoriaId);
            var categorias = await _eventoRepo.ObtenerCategoriasActivasAsync();

            ViewBag.Categorias = categorias;
            ViewBag.CategoriaSeleccionadaId = categoriaId;

            return View(eventos);
        }

        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var evento = await _eventoRepo.ObtenerPorIdAsync(id);
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.InscritosConfirmados = await _eventoRepo.ContarInscritosConfirmadosAsync(id);
            return View(evento);
        }

        // GET: Eventos/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasDesplegablesAsync();
            var modelo = new Evento
            {
                Fecha = DateTime.Now.AddDays(7),
                Modalidad = "Presencial",
                Aforo = 50,
                Precio = 0.00m
            };
            return View(modelo);
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evento evento)
        {
            if (evento.Fecha <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(evento.Fecha), "La fecha del evento debe ser futura.");
            }

            if (ModelState.IsValid)
            {
                await _eventoRepo.CrearAsync(evento);
                TempData["Exito"] = $"El evento '{evento.Titulo}' fue creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
            return View(evento);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var evento = await _eventoRepo.ObtenerParaEditarAsync(id);
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
            return View(evento);
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _eventoRepo.ActualizarAsync(evento);
                TempData["Exito"] = $"El evento '{evento.Titulo}' fue actualizado con éxito.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
            return View(evento);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _eventoRepo.ObtenerPorIdAsync(id);
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.InscritosConfirmados = await _eventoRepo.ContarInscritosConfirmadosAsync(id);
            return View(evento);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventoRepo.EliminarLogicoAsync(id);
            TempData["Exito"] = "El evento fue eliminado lógicamente del sistema.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasDesplegablesAsync(int? categoriaSeleccionada = null, string? modalidadSeleccionada = null)
        {
            var categorias = await _eventoRepo.ObtenerCategoriasActivasAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);
            ViewBag.Modalidades = new SelectList(new[] { "Presencial", "Virtual" }, modalidadSeleccionada ?? "Presencial");
        }
    }
}

