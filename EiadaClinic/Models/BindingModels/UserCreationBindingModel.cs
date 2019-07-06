using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.BindingModels
{
    public class UserCreationBindingModel
    {


        public string Id { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Display(Name = "Email")]
        public virtual string Email { get; set; }
        [Display(Name = "PhoneNumber")]
        public virtual string PhoneNumber { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }
        [Display(Name = "First Name")]
        public virtual string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public virtual string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public virtual string LastName { get; set; }
        [Display(Name = "Gender")]
        public virtual string Gender { get; set; }
        [Display(Name = "Address")]
        public virtual string Address { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public virtual DateTime Birthday { get; set; }


        public UserCreationBindingModel()
        {

        }
    }
}
