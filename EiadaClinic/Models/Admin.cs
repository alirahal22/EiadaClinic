using Clinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Admin
    {
        public string Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public EiadaUser User { get; set; }

        public virtual ICollection<Reminder> Reminders { get; set; }
        public virtual ICollection<Message> Messages { get; set; }




        /*public List<Reminder> getReminders(ApplicationDbContext context)
        {
            return context.Reminders.Where(r => r.EiadaUser.Id == this.Id).ToList();
        }*/
    }
}
