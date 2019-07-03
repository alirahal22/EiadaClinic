using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Assistant
    {
        public string Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public EiadaUser User { get; set; }
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
