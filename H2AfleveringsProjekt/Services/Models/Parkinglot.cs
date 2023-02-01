using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2AfleveringsProjekt.Services.Models
{
    public class Parkinglot
    {
        public int CarMaxSlots { get; set; } = 20; //Antallet af slots for CarMax
        public int ExtendedCarSlots { get; set; } = 5;
        public int BigCarSlots { get; set; } = 2;

    }
}
