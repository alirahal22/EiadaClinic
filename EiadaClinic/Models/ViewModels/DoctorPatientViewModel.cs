using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.ViewModels
{
    public class DoctorPatientViewModel
    {
        public Patient Patient { get; set; }
        public List<Consultation> Consultations { get; set; }
    }
}
