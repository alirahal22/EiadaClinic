using Clinic.Models;
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
        public string Id { get; set; }
        [Display(Name = "Blood Type")]
        public string BloodType { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public EiadaUser User { get; set; }
    }
}
