using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.ViewModels
{
    public class PatientHomeViewModel
    {

        public List<Doctor> Doctors { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
