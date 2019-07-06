using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models.ViewModels
{
    public class DoctorViewModel
    {

        public Doctor Doctor { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
