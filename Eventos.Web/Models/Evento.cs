using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El lugar es obligatorio.")]
        [StringLength(100, ErrorMessage = "El lugar no puede superar los 100 caracteres.")]
        [Display(Name = "Lugar")]
        public string Lugar { get; set; } = string.Empty;

        [Required(ErrorMessage = "El aforo es obligatorio.")]
        [Range(1, 5000, ErrorMessage = "El aforo debe ser un número entero entre 1 y 5000.")]
        [Display(Name = "Aforo")]
        public int Aforo { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 99999999.99, ErrorMessage = "El precio debe ser un valor no negativo.")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria.")]
        [StringLength(20, ErrorMessage = "La modalidad no puede superar los 20 caracteres.")]
        [Display(Name = "Modalidad")]
        public string Modalidad { get; set; } = "Presencial";

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        public Categoria? Categoria { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
