using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;
using H2AfleveringsProjekt.Data.Interface;

namespace H2AfleveringsProjekt.Data.Methods
{
    public class Parking : IParking
    {
        public List<Car> ListOfCars { get; set; } = new List<Car>();
        public List<ExtendedCar> ListOfExtendedCars { get; set; } = new List<ExtendedCar>();
        public List<BigCar> ListOfBigCars { get; set; } = new List<BigCar>();

        private Parkinglot parking = new Parkinglot();
        public Parkinglot CheckIn(CarType type, string plate)
        {
            if (ListOfCars.Any(x => x.ticket.NumerberPlate.ToLower() == plate) ||
                ListOfExtendedCars.Any(x => x.ticket.NumerberPlate.ToLower() == plate) ||
                ListOfBigCars.Any(x => x.ticket.NumerberPlate.ToLower() == plate))
                    throw new Exception("There is already a car parked with this plate number!");

            switch (type)
            {
                case CarType.Car:
                    if (ListOfCars.Count() >= parking.CarMaxSlots)
                        throw new OverflowException("There is not enough space for you.");

                    parking = (Car)CreateCarObj(new Car(), plate);
                    ListOfCars.Add((Car)parking);
                    break;
                case CarType.ExtendedCar:
                    if (ListOfExtendedCars.Count() >= parking.ExtendedCarSlots)
                        throw new OverflowException("There is not enough space for you.");

                    parking = (ExtendedCar)CreateCarObj(new ExtendedCar(), plate);
                    ListOfExtendedCars.Add((ExtendedCar)parking);

                    break;
                case CarType.BigCar:
                    if (ListOfBigCars.Count() >= parking.BigCarSlots)
                        throw new OverflowException("There is not enough space for you.");

                    parking = (BigCar)CreateCarObj(new BigCar(), plate);
                    ListOfBigCars.Add((BigCar)parking); 
                    break;
            }

            return parking;
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
        private ICars CreateCarObj(ICars car, string plate)
        {
            car.ticket = new Ticket
            {
                NumerberPlate = plate,
                ParkStart = DateTime.UtcNow
            };
            switch(car.GetType().Name)
            {
                case "Car":
                    car.ticket.TicketID = ListOfCars.Count() + 1;
                    car.ParkingSpot = ListOfCars.Count() + 1;
                    car.ticket.Type = CarType.Car;
                    break;
                case "ExtendedCar":
                    car.ticket.TicketID = ListOfExtendedCars.Count() + 1;
                    car.ParkingSpot = ListOfExtendedCars.Count() + 1;
                    car.ticket.Type = CarType.ExtendedCar;
                    break;
                case "BigCar":
                    car.ticket.TicketID = ListOfBigCars.Count() + 1;
                    car.ParkingSpot = ListOfBigCars.Count() + 1;
                    car.ticket.Type = CarType.BigCar;
                    break;
            }
            return car;
        } 
        /// <summary>
        /// Finds the listed car, and gets the price pr hour.
        /// Time raised by 5 hours by default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList"></param>
        /// <param name="search"></param>
        /// <returns>Hours, Cost of parking in $</returns>
        private async Task<KeyValuePair<int, int>> GetCalculatedCar<T>(List<T> thisList, string search) where T : ICars
        {
            var _ = thisList.Find(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search);
            int hours = _.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
            thisList.Remove(_);
            return new KeyValuePair<int, int>(hours, hours*(int)_.ticket.Type);
        }

        public List<Parkinglot> GetListofCars(CarType type)
        {
            return null;
        }
    }
}
