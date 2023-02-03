using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public class CarWash
    {
        public Ticket Ticket { get; set; }
        public int CarWashID { get; set; }
        public DateTime WashEnd { get; set; }
        public WashType WashType { get; set; }
    }
}
