using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public interface ICars
    {
        Ticket ticket { get; set; }
        int ParkingSpot { get; set; }
    }
}
