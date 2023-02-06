using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Services.Models;
using H2AfleveringsProjekt.Data.Interface;
using H2AfleveringsProjekt.Services.Models.Enums;

// TODO: Tilføje så de får den næste ledige plads - FIKSED
// TODO: Tilføje en global tilbage knap

namespace H2AfleveringsProjekt.Data.Methods
{
    public class Parking : IParking
    {
        #region Properties and Fields
        public List<Car> ListOfCars { get; set; } = new List<Car>();
        public List<ExtendedCar> ListOfExtendedCars { get; set; } = new List<ExtendedCar>();
        public List<BigCar> ListOfBigCars { get; set; } = new List<BigCar>();
        public List<CarWash> ListOfCarWashes { get; set; } = new List<CarWash>();

        int _ticketsSold = 0;
        int _carWashSold = 0;

        #endregion


        public async Task<int> CheckIn(CarType type, string plate)
        {
            try
            {
                if (ListOfCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate) ||
                    ListOfExtendedCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate) ||
                    ListOfBigCars.Any(x => x.ticket?.NumerberPlate?.ToLower() == plate))
                    throw new Exception("There is already a car parked with this plate number!");

                Parkinglot parking = new Parkinglot();

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
            ICar _;
            _ = ListOfCars.FirstOrDefault(x => x.ticket?.NumerberPlate == search || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return await GetCalculatedCar(_, search);

            _ = ListOfExtendedCars.FirstOrDefault(x => x.ticket?.NumerberPlate == search || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return await GetCalculatedCar(_, search);

            _ = ListOfBigCars.FirstOrDefault(x => x.ticket?.NumerberPlate == search || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return await GetCalculatedCar(_, search);
            
            throw new KeyNotFoundException("Could not find the car matching your TicketID / Numberplate.");
        }   
        public int WashCar(WashType type, Ticket ticket)
        {
            _carWashSold++;
            CarWash _ = new CarWash()
            {
                CarWashID = _carWashSold,
                WashType = type,
                Ticket = ticket,
                Price = GetWashPrice(type)
            };

            if (!ListOfCarWashes.Any())
                _.WashEnd = DateTime.Now.AddSeconds((int)type);
            else
                _.WashEnd = ListOfCarWashes[ListOfCarWashes.Count() - 1].WashEnd.AddSeconds((int)type);

            ListOfCarWashes.Add(_);

            return (int)_.WashType;
        }
        public ICar FindCarAsync<T>(string search)
        {
            ICar _;
            _ = ListOfCars.FirstOrDefault(x => x.ticket?.NumerberPlate.ToLower() == search.ToLower() || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return _;

            _ = ListOfExtendedCars.FirstOrDefault(x => x.ticket?.NumerberPlate.ToLower() == search.ToLower() || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return _;

            _ = ListOfBigCars.FirstOrDefault(x => x.ticket?.NumerberPlate.ToLower() == search.ToLower() || Convert.ToString(x.ticket?.TicketID) == search);
            if (_ != null)
                return _;

            return null;
        }

        #region Tasks
        /// <summary>
        /// Task for running the Carwash
        /// </summary>
        /// <returns></returns>
        public async Task RunCarWash()
        {
            while(true)
            {
                await Task.Delay(250);
                try { CheckForWash(); }
                catch { }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates the types of cars that is needed and being called from CheckIn()
        /// </summary>
        /// <typeparam name="T">Can either be Car, ExtendedCar, BigCar</typeparam>
        /// <returns></returns>
        /// <exception cref="OverflowException"></exception>
        private async Task<int> CreateCarObj<T>(List<T> listOfCars, int maxSlotCount, ICar car, string plate) where T : ICar
        {
            if (listOfCars.Count(x => x.ticket != null) >= maxSlotCount)
                throw new OverflowException("There is not enough space for you.");

            ICar obj = ListOfCars.FirstOrDefault(x => x.ticket == null);
            _ticketsSold++;
            car.ticket = new Ticket
            {
                NumerberPlate = plate,
                ParkStart = DateTime.UtcNow,
                TicketID = _ticketsSold
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
        private async Task<KeyValuePair<int, int>> GetCalculatedCar(ICar car, string search)
        {
            int hours = car.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
            CarType? type = car.ticket.Type;
            car.ticket = null;
            
            return new KeyValuePair<int, int>(hours, hours*(int)type + (car.ticket?.CarWash.Price ?? 0));
        }
        private void CheckForWash()
        {
            if (!ListOfCarWashes.Any())
                return;

            CarWash end = ListOfCarWashes[0];
            if (end.WashEnd.CompareTo(DateTime.Now) < 0)
            {
                ListOfCarWashes.Remove(end);
                switch(end.Ticket.Type)
                {
                    case CarType.Car:
                        ListOfCars.FirstOrDefault(x => x.ticket.TicketID == end.Ticket.TicketID).ticket.CarWash = end;
                        break;
                    case CarType.ExtendedCar:
                        ListOfExtendedCars.FirstOrDefault(x => x.ticket.TicketID == end.Ticket.TicketID).ticket.CarWash = end;
                        break;
                    case CarType.BigCar:
                        ListOfBigCars.FirstOrDefault(x => x.ticket.TicketID == end.Ticket.TicketID).ticket.CarWash = end;
                        break;
                }
                Console.WriteLine("Done");
            }
        }
        private int GetWashPrice(WashType type)
        {
            switch (type)
            {
                case WashType.Economic:
                    return (int)WashPrice.Economic;
                case WashType.Basic:
                    return (int)WashPrice.Basic;
                case WashType.Premium:
                    return (int)WashPrice.Premium;
            }
            return 0;
        }
        #endregion
    }
}
