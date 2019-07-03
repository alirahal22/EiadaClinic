using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models.ViewModels
{
    [NotMapped]
    public class PatientViewModel
    {
        public Patient patient { get; set; }
        public ICollection<Reminder> Reminders { get; set; }
        public Dictionary<Doctor, List<Consultation>> ConsultationsDictionary { get; set; }
    
    }
}
