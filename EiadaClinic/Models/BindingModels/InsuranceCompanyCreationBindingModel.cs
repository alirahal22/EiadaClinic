using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.BindingModels
{
    public class InsuranceCompanyCreationBindingModel: UserCreationBindingModel
    {
        public string Name { get; set; }
        public string Fax { get; set; }
    }
}
