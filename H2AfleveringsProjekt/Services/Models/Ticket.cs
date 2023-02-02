using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public class Ticket
    {
        public int? TicketID { get; set; } = null;
        public string NumerberPlate { get; set; } = null;
        public DateTime? ParkStart { get; set; } = null;
        public CarType? Type { get; set; } = null;
    }
}
