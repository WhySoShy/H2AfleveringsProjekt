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

                    parking = new Car()
                    {
                        ticket = new Ticket()
                        {
                            NumerberPlate = plate,
                            ParkStart = DateTime.UtcNow,
                            TicketID = ListOfCars.Count() + 1
                        },
                        ParkingSpot = ListOfCars.Count() + 1
                    };
                    ListOfCars.Add((Car)parking);
                    break;
                case CarType.ExtendedCar:
                    if (ListOfExtendedCars.Count() >= parking.ExtendedCarSlots)
                        throw new OverflowException("There is not enough space for you.");

                    parking = new ExtendedCar()
                    {
                        ticket = new Ticket()
                        {
                            NumerberPlate = plate,
                            ParkStart = DateTime.UtcNow,
                            TicketID = ListOfExtendedCars.Count() + 1
                        },
                        ParkingSpot = ListOfExtendedCars.Count()+1

                    };
                    ListOfExtendedCars.Add((ExtendedCar)parking);

                    break;
                case CarType.BigCar:
                    if (ListOfBigCars.Count() >= parking.BigCarSlots)
                        throw new OverflowException("There is not enough space for you.");

                    parking = new BigCar()
                    {
                        ticket = new Ticket()
                        {
                            NumerberPlate = plate,
                            ParkStart = DateTime.UtcNow,
                            TicketID = ListOfBigCars.Count() + 1
                        },
                        ParkingSpot = ListOfBigCars.Count() + 1
                    };

                    ListOfBigCars.Add((BigCar)parking); 
                    break;
            }

            return parking;
        }
        public async Task<KeyValuePair<int, decimal>> CheckOut(string search)
        {            
            if (ListOfCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
            {
                Car _ = ListOfCars.Find(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search);
                int hours = _.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
                test(ListOfCars);
                ListOfCars.Remove(_);
                return new KeyValuePair<int, decimal>(hours,hours*(int)CarType.Car);
            }

            else if (ListOfExtendedCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
            {
                ExtendedCar _ = ListOfExtendedCars.Find(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search);
                int hours = _.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
                test(ListOfExtendedCars);
                ListOfExtendedCars.Remove(_);
                return new KeyValuePair<int, decimal>(hours, hours*(int)CarType.ExtendedCar);
            }
            
            else if (ListOfBigCars.Any(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search))
            {
                BigCar _ = ListOfBigCars.Find(x => x.ticket.NumerberPlate == search || Convert.ToString(x.ticket.TicketID) == search);
                int hours = _.ticket.ParkStart.Value.Subtract(DateTime.UtcNow).Hours + 5;
                test(ListOfBigCars);
                ListOfBigCars.Remove(_);
                return  new KeyValuePair<int, decimal>(hours, hours*(int)CarType.BigCar);
            }
            throw new KeyNotFoundException("Could not find the car matching your TicketID / Numberplate.");
        }   
        
        private void test<T>(List<T> testList) where T : ICars
        {
            foreach(var item in testList)
                Console.WriteLine(item.ticket.NumerberPlate);
        }

        public List<Parkinglot> GetListofCars(CarType type)
        {
            return null;
        }
    }
}
