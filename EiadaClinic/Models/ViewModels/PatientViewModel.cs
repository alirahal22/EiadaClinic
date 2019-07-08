using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.ViewModels
{
    [NotMapped]
    public class PatientViewModel
    {
        public Patient Patient { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
