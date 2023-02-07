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
        ICar FindCar(string search);
        /// <returns>One of the following objects ( Car, ExtendedCar or BigCar )</returns>
        Task<int> CheckIn(CarType type, string plate);
        /// <returns>One of the following objects ( Car, ExtendedCar or BigCar )</returns>
        Task<KeyValuePair<int, int>> CheckOut(string search);
        /// <summary>
        /// Adds the car to the washing queue
        /// </summary>
        void WashCar(WashType type, Ticket ticket);
        /// <summary>
        /// Runs through the first car in the queue in both lists ( WashHall1, WashHall2 )
        /// </summary>
        Task RunCarWash();
        /// <summary>
        /// Gets the next availeable wash.
        /// </summary>
        /// <returns>Timespan</returns>
        TimeSpan EstimatedTime();
        /// <summary>
        /// Gets the estimated time for the next availeable carwash in both washing halls.
        /// </summary>
        /// <returns>Timespan</returns>
        TimeSpan EstimatedTime(List<CarWash> hall);
    }
}
