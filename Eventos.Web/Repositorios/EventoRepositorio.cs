using Microsoft.EntityFrameworkCore;
using Eventos.Web.Data;
using Eventos.Web.Models;

namespace Eventos.Web.Repositorios
{
    public class EventoRepositorio
    {
        private readonly EventosContext _context;

        public EventoRepositorio(EventosContext context)
        {
            _context = context;
        }

        public async Task<List<Evento>> ObtenerTodosActivosAsync(int? categoriaId = null)
        {
            var query = _context.Eventos
                .AsNoTracking()
                .Include(e => e.Categoria)
                .Include(e => e.Inscripciones)
                .Where(e => e.Activo);

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(e => e.CategoriaId == categoriaId.Value);
            }

            return await query.OrderBy(e => e.Fecha).ToListAsync();
        }

        public async Task<Evento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Eventos
                .AsNoTracking()
                .Include(e => e.Categoria)
                .Include(e => e.Inscripciones)
                    .ThenInclude(i => i.Asistente)
                .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        }

        public async Task<Evento?> ObtenerParaEditarAsync(int id)
        {
            return await _context.Eventos
                .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        }

        public async Task CrearAsync(Evento evento)
        {
            evento.Activo = true;
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Evento evento)
        {
            _context.Eventos.Update(evento);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarLogicoAsync(int id)
        {
            var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id);
            if (evento != null)
            {
                evento.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<int> ContarInscritosConfirmadosAsync(int eventoId)
        {
            return await _context.Inscripciones
                .AsNoTracking()
                .CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");
        }
    }
}

