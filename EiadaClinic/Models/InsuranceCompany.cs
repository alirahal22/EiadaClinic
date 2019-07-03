using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public class InsuranceCompany
    {
        public string Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public EiadaUser User { get; set; }


        public string Name { get; set; }
        public string Fax { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
        
    }
}
