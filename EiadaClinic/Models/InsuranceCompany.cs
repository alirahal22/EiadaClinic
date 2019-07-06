using EiadaClinic.Models;
using EiadaClinic.Models.BindingModels;
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
        [ForeignKey("User")]
        public string Id { get; set; }
        public EiadaUser User { get; set; }


        public string Name { get; set; }
        public string Fax { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }


        public InsuranceCompanyCreationBindingModel ToInsuranceCompanyCreationBindingModel()
        {
            return new InsuranceCompanyCreationBindingModel()
            {
                Id = this.Id,
                UserName = User.UserName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                Address = User.Address,
                Name = this.Name,
                Fax = this.Fax,
            };
        }
    }
}
