using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Consultation
    {
        
        public int Id { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; }
        [Display(Name = "Temperature")]
        public float Temperature { get; set; }
        [Display(Name = "Blood Pressure")]
        public float BloodPressure { get; set; }
        [Display(Name = "Fee")]
        public float Fee { get; set; }
        [Display(Name = "Treatment")]
        public string Treatment { get; set; }
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
