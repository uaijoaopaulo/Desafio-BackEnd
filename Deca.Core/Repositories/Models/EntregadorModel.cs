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
    public class EntregadorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "Name must be declared")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "CNPJ must be declared")]
        public required string CNPJ { get; set; }
        [Required(ErrorMessage = "Birth date must be declared")]
        public required DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "CNH must be declared")]
        public required string CNH { get; set; }
        [Required(ErrorMessage = "CNH type must be declared")]
        public required CNHTypeEnum CNHType { get; set; }
        [Required(ErrorMessage = "CNH image must be declared")]
        public required string CNHImage { get; set; }
    }
}
