using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Models
{
    public class MotoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Year must be declared")]
        public required int Year { get; set; }
        [Required(ErrorMessage = "Model must be declared")]
        public required string Model { get; set; }
        [Required(ErrorMessage = "License Plate must be declared")]
        public required string LicensePlate { get; set; } //DadoUnico
    }
}
