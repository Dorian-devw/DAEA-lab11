using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly InscripcionRepositorio _inscripcionRepo;

        public InscripcionesController(InscripcionRepositorio inscripcionRepo)
        {
            _inscripcionRepo = inscripcionRepo;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            var inscripciones = await _inscripcionRepo.ObtenerTodasAsync();
            return View(inscripciones);
        }

        // GET: Inscripciones/Create?eventoId=5
        public async Task<IActionResult> Create(int? eventoId)
        {
            await CargarListasDesplegablesAsync(eventoId);
            var inscripcion = new Inscripcion
            {
                EventoId = eventoId ?? 0,
                FechaInscripcion = DateTime.Now
            };
            return View(inscripcion);
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inscripcion inscripcion)
        {
            var evento = await _inscripcionRepo.ObtenerEventoAsync(inscripcion.EventoId);

            if (evento == null)
            {
                ModelState.AddModelError(nameof(inscripcion.EventoId), "El evento seleccionado no existe o no se encuentra activo.");
            }
            else
            {
                // Validación 1: Que el evento no haya pasado
                if (evento.Fecha <= DateTime.Now)
                {
                    ModelState.AddModelError(nameof(inscripcion.EventoId), "No es posible inscribirse a un evento cuya fecha ya ha pasado.");
                }

                // Validación 2: Que el asistente no esté ya inscrito (Confirmada) en ese evento
                bool yaInscrito = await _inscripcionRepo.YaEstaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId);
                if (yaInscrito)
                {
                    ModelState.AddModelError(nameof(inscripcion.AsistenteId), "El asistente ya cuenta con una inscripción 'Confirmada' en este evento.");
                }

                // Validación 3: Que el aforo no esté completo (inscripciones Confirmada menores que el Aforo)
                int confirmadas = await _inscripcionRepo.ContarConfirmadasPorEventoAsync(inscripcion.EventoId);
                if (confirmadas >= evento.Aforo)
                {
                    ModelState.AddModelError(nameof(inscripcion.EventoId), $"El aforo del evento ({evento.Aforo} personas) ya está completo. No hay cupos disponibles.");
                }
            }

            if (ModelState.IsValid)
            {
                await _inscripcionRepo.CrearAsync(inscripcion);
                TempData["Exito"] = "Inscripción registrada con estado Confirmada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListasDesplegablesAsync(inscripcion.EventoId, inscripcion.AsistenteId);
            return View(inscripcion);
        }

        // POST: Inscripciones/Cancelar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            await _inscripcionRepo.CancelarAsync(id);
            TempData["Exito"] = "La inscripción ha sido cancelada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasDesplegablesAsync(int? eventoSeleccionado = null, int? asistenteSeleccionado = null)
        {
            var eventos = await _inscripcionRepo.ObtenerEventosActivosAsync();
            var asistentes = await _inscripcionRepo.ObtenerAsistentesActivosAsync();

            ViewBag.Eventos = new SelectList(
                eventos.Select(e => new
                {
                    Id = e.Id,
                    Texto = $"{e.Titulo} ({e.Fecha:dd/MM/yyyy HH:mm} - {e.Modalidad} - Aforo: {e.Aforo})"
                }),
                "Id",
                "Texto",
                eventoSeleccionado
            );

            ViewBag.Asistentes = new SelectList(
                asistentes.Select(a => new
                {
                    Id = a.Id,
                    Texto = $"{a.Nombres} (DNI: {a.Dni} - Correo: {a.Correo})"
                }),
                "Id",
                "Texto",
                asistenteSeleccionado
            );
        }
    }
}

