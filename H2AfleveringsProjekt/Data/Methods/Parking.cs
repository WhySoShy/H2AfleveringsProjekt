using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;
using H2AfleveringsProjekt.Data.Interface;
using System.Threading;

// TODO: Tilføje så de får den næste ledige plads - FIKSED
// TODO: Tilføj GUUID til Tickets istedet for int som ticketid
// TODO: Tilføje en global tilbage knap

namespace H2AfleveringsProjekt.Data.Methods
{
    public class Parking : IParking
    {
        public List<Car> ListOfCars { get; set; } = new List<Car>();
        public List<ExtendedCar> ListOfExtendedCars { get; set; } = new List<ExtendedCar>();
        public List<BigCar> ListOfBigCars { get; set; } = new List<BigCar>();

        private Parkinglot parking = new Parkinglot();
        public async Task<int> CheckIn(CarType type, string plate)
        {
            try
            {
                if (ListOfCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate) ||
                    ListOfExtendedCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate) ||
                    ListOfBigCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate))
                    throw new Exception("There is already a car parked with this plate number!");

                switch (type)
                {   
                    case CarType.Car:
                        return await CreateCarObj(ListOfCars, parking.CarMaxSlots, new Car(), plate);
                    case CarType.ExtendedCar:
                        return await CreateCarObj(ListOfExtendedCars, parking.ExtendedCarSlots, new ExtendedCar(), plate);
                    case CarType.BigCar:
                        return await CreateCarObj(ListOfBigCars, parking.BigCarSlots, new BigCar(), plate);
                }
                return 2;
            }
            catch (OverflowException r){throw r;}

        }
        public async Task<KeyValuePair<int, int>> CheckOut(string search)
        {            
            if (ListOfCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
                return await GetCalculatedCar(ListOfCars, search);

            else if (ListOfExtendedCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
                return await GetCalculatedCar(ListOfExtendedCars, search);
            
            else if (ListOfBigCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
                return await GetCalculatedCar(ListOfBigCars, search);
            
            throw new KeyNotFoundException("Could not find the car matching your TicketID / Numberplate.");
        }   


        #region Private methods
        private async Task<int> CreateCarObj<T>(List<T> listOfCars, int maxSlotCount, ICar car, string plate) where T : ICar
        {
            if (listOfCars.Count(x => x.ticket != null) >= maxSlotCount)
                throw new OverflowException("There is not enough space for you.");

            ICar obj = ListOfCars.FirstOrDefault(x => x.ticket == null);

            car.ticket = new Ticket
            {
                NumerberPlate = plate,
                ParkStart = DateTime.UtcNow,
                TicketID = 1 
            };
            switch(car.GetType().Name)
            {
                case "Car":
                    car.ticket.Type = CarType.Car;
                    break;
                case "ExtendedCar":
                    car.ticket.Type = CarType.ExtendedCar;
                    obj = ListOfExtendedCars.FirstOrDefault(x => x.ticket == null);
                    break;
                case "BigCar":
                    obj = ListOfBigCars.FirstOrDefault(x => x.ticket == null);
                    car.ticket.Type = CarType.BigCar;
                    break;
            }
            obj.ticket = car.ticket;
            return obj.ParkingSpot;
        }
        /// <summary>
        /// Finds the listed car, and gets the price pr hour.
        /// Time raised by 5 hours by default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList"></param>
        /// <param name="search"></param>
        /// <returns>Hours, Cost of parking in $</returns>
        private async Task<KeyValuePair<int, int>> GetCalculatedCar<T>(List<T> thisList, string search) where T : ICar
        {
            var _ = thisList.Find(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search);
            int hours = _.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
            CarType? type = _.ticket.Type;
            _.ticket = null;
            
            return new KeyValuePair<int, int>(hours, hours*(int)type);
        }

        #endregion
    }
}
