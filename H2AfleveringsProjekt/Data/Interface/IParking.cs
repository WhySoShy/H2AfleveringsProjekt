using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;

namespace H2AfleveringsProjekt.Data.Interface
{
    public interface IParking
    {
        List<Car> ListOfCars { get; set; }
        List<ExtendedCar> ListOfExtendedCars { get; set; }
        List<BigCar> ListOfBigCars { get; set; }
        List<CarWash> WashHall1 { get; set; }
        List<CarWash> WashHall2 { get; set; }

        /// <returns>The object of the TicketID or numberplate has been found</returns>
        ICar FindCarAsync<T>(string search);
        /// <returns>Et af 3 følgende objekter (BigCar, Car, Extendedcar)</returns>
        Task<int> CheckIn(CarType type, string plate);
        /// <returns>Et af 3 følgende objekter (BigCar, Car, Extendedcar)</returns>
        Task<KeyValuePair<int, int>> CheckOut(string search);
        int WashCar(WashType type, Ticket ticket);
        Task RunCarWash();
        TimeSpan EstimatedTime();
        TimeSpan EstimatedTime(List<CarWash> hall);
    }
}
