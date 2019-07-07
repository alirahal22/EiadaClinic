using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public class Appointment
    {

        public int Id { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
