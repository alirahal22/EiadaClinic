using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Patient
    {
        [ForeignKey("User")]
        public string Id { get; set; }
        public EiadaUser User { get; set; }
        [Display(Name = "Blood Type")]
        public string BloodType { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }
        

        public virtual ICollection<PatientDoctor> PatientDoctors { get; set; }
        public virtual ICollection<Consultation> Consultations { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
