using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.BindingModels
{
    public class PatientCreationBindingModel: UserCreationBindingModel
    {
        public string BloodType { get; set; }
        public string InsuranceCompanyId { get; set; }
    }
}