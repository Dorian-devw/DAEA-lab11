using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models
{
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio.")]
        [Display(Name = "Evento")]
        public int EventoId { get; set; }

        [ForeignKey(nameof(EventoId))]
        public Evento? Evento { get; set; }

        [Required(ErrorMessage = "El asistente es obligatorio.")]
        [Display(Name = "Asistente")]
        public int AsistenteId { get; set; }

        [ForeignKey(nameof(AsistenteId))]
        public Asistente? Asistente { get; set; }

        [Required]
        [Display(Name = "Fecha de Inscripción")]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Confirmada"; // "Confirmada" o "Cancelada"
    }
}

