using EiadaClinic.Models;
using EiadaClinic.Models.BindingModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Doctor: IEquatable<Doctor>
    {
        [ForeignKey("User")]
        public string Id { get; set; }
        public EiadaUser User { get; set; }

        public string Specialty { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }

        [ForeignKey("Assistant")]
        public string AssistantId { get; set; }
        public Assistant Assistant { get; set; }
  

        public virtual ICollection<PatientDoctor> PatientDoctors { get; set; }
        public virtual ICollection<Consultation> Consultations { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }

        public bool Equals(Doctor other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals((Doctor)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Specialty, OpenTime, CloseTime);
        }

        public DoctorCreationBindingModel ToDoctorCreationBindingModel()
        {
            return new DoctorCreationBindingModel()
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
                Specialty = this.Specialty,
                OpenTime = this.OpenTime,
                CloseTime = this.CloseTime
            };
        }
        
    }
}
