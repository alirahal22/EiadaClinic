using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.BindingModels
{
    public class AssistantCreationBindingModel: UserCreationBindingModel
    {
        public string DoctorId { get; set; }
    }
}
