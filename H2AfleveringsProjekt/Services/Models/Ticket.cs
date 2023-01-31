using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public class Ticket
    {
        public int? TicketID { get; set; }
        public string NumerberPlate { get; set; }
        public DateTime? ParkStart { get; set; }
    }
}
