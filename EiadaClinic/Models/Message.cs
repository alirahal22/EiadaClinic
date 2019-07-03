using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EiadaClinic.Models
{
    public class Message
    {

        public string Id { get; set; }
        public string content { get; set; }
        public EiadaUser sender { get; set; }
    }
}
