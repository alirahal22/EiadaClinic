using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.BindingModels
{
    public class DoctorCreationBindingModel: UserCreationBindingModel
    {
        [Display(Name = "Specialty")]
        public string Specialty { get; set; }
        [Display(Name = "OpenTime")]
        public string OpenTime { get; set; }
        [Display(Name = "CloseTime")]
        public string CloseTime { get; set; }
        [Display(Name = "Assistant")]
        public string AssistantId { get; set; }


        public DoctorCreationBindingModel()
        {

        }
    }
}
