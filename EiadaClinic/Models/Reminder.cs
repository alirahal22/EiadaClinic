using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Models
{
    public class Reminder
    {

        public int Id { get; set; }
        public int Priority { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public bool IsDone { get; set; }
    }
}
