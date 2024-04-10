using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Models
{
    public class LocacaoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "IdMoto must be declared")]
        public int IdMoto { get; set; }
        [Required(ErrorMessage = "IdEntregador must be declared")]
        public int IdEntregador { get; set; }
        [Required(ErrorMessage = "Rent days must be declared")]
        public int DiasLocacao { get; set; }
        [Required(ErrorMessage = "Data de inicio must be declared")]
        public DateTime DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        [Required(ErrorMessage = "Data de previsao de termino must be declared")]
        public DateTime DataPrevisaoTermino { get; set; }
        [Required(ErrorMessage = "Valor locação must be declared")]
        public decimal ValorLocacao { get; set; }
        [Required(ErrorMessage = "Ativo must be declared")]
        public bool Ativo { get; set; }
    }
}
