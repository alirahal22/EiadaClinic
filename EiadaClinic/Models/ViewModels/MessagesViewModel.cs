using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models.ViewModels
{
    public class MessagesViewModel
    {
        public List<Message> Messages { get; set; }
        public List<Reminder> Reminders { get; set; }
        public Reminder Reminder { get; set; }
    }
}
