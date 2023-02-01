using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public class Car : Parkinglot, ICars
    {
        public Ticket ticket { get; set; }
        public int ParkingSpot { get; set; }
    }
}
