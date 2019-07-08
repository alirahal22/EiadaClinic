using EiadaClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Time { get; set; }


    }
}
