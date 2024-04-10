using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desa.Models;

namespace Desa.Core.Repositories.Models
{
    public class PedidoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Create date must be declared")]
        public DateTime CreateDate { get; set; }
        [Required(ErrorMessage = "Amount must be declared")]
        public int IdEntregador { get; set; }
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Situation must be declared")]
        public OrderSituationEnum Situation { get; set; }
    }
}
