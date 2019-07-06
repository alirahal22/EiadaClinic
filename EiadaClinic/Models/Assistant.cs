using EiadaClinic.Models.BindingModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Assistant
    {
        [ForeignKey("User")]
        public string Id { get; set; }
        public EiadaUser User { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public AssistantCreationBindingModel ToAssistantCreationBindingModel()
        {
            return new AssistantCreationBindingModel()
            {
                Id = this.Id,
                FirstName = User.FirstName,
                MiddleName = User.MiddleName,
                LastName = User.LastName,
                UserName = User.UserName,
                Email = User.Email,
                Gender = User.Gender,
                Address = User.Address,
                Birthday = User.Birthday,
                PhoneNumber = User.PhoneNumber,
                DoctorId = this.DoctorId

            };
        }

    }
}
