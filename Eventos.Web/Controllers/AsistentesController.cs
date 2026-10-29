using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers
{
    public class AsistentesController : Controller
    {
        private readonly AsistenteRepositorio _asistenteRepo;

        public AsistentesController(AsistenteRepositorio asistenteRepo)
        {
            _asistenteRepo = asistenteRepo;
        }

        // GET: Asistentes
        public async Task<IActionResult> Index()
        {
            var asistentes = await _asistenteRepo.ObtenerTodosActivosAsync();
            return View(asistentes);
        }

        // GET: Asistentes/Create
        public IActionResult Create()
        {
            return View(new Asistente());
        }

        // POST: Asistentes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asistente asistente)
        {
            if (await _asistenteRepo.ExisteDniAsync(asistente.Dni))
            {
                ModelState.AddModelError(nameof(asistente.Dni), "El DNI ingresado ya se encuentra registrado.");
            }

            if (await _asistenteRepo.ExisteCorreoAsync(asistente.Correo))
            {
                ModelState.AddModelError(nameof(asistente.Correo), "El correo electrónico ya se encuentra registrado.");
            }

            if (ModelState.IsValid)
            {
                await _asistenteRepo.CrearAsync(asistente);
                TempData["Exito"] = $"El asistente '{asistente.Nombres}' fue registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(asistente);
        }

        // POST: Asistentes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _asistenteRepo.EliminarLogicoAsync(id);
            TempData["Exito"] = "Asistente eliminado lógicamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}

