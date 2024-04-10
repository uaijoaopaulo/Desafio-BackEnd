using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Desa.Core.Repositories.Models
{
    public class NotificacaoPedidoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [BsonId]
        public int Id { get; set; }
        [Required(ErrorMessage = "IdEntregador must be declared")]
        public int IdEntregador { get; set; }
        [Required(ErrorMessage = "IdPedido must be declared")]
        public int IdPedido { get; set; }
        [Required(ErrorMessage = "Notification date must be declared")]
        public DateTime DataNotificacao { get; set; }

        [Required(ErrorMessage = "Valid must be declared")]
        public bool Valida { get; set; }
        [Required(ErrorMessage = "Acepted must be declared")]
        public bool Aceita { get; set; }
    }
}
