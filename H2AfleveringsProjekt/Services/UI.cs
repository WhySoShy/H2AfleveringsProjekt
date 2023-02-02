using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2AfleveringsProjekt.Data.Interface;
using H2AfleveringsProjekt.Data.Methods;
using H2AfleveringsProjekt.Services.Models;

namespace H2AfleveringsProjekt.Services
{
    // TODO: Tilføje try catches, til at tage imod Exceptions!
    public class UI
    {
        private readonly IParking _parking;
        public UI()
        {
            ServiceProvider services = new ServiceCollection().AddSingleton<IParking, Parking>().BuildServiceProvider();
            _parking = services.GetRequiredService<IParking>();
        }
        public void InitLists()
        {
            for (int i = 0; i < 20; i++)
                _parking.ListOfCars.Add(new Car() { ParkingSpot = i + 1 }) ;
            for (int i = 0; i < 5; i++)
                _parking.ListOfExtendedCars.Add(new ExtendedCar());
            for (int i = 0; i < 3; i++)
                _parking.ListOfBigCars.Add(new BigCar());
        }
        public void ShowTicketlist()
        {
            Console.WriteLine($"Count: {_parking.ListOfCars.Count(x => x.ticket != null)}");
            Console.WriteLine("\nList\n");
            for (int i = 0; i < _parking.ListOfCars.Count(x => x.ticket != null); i++)
            {
                Console.WriteLine("TicketID: " + _parking.ListOfCars[i].ticket.TicketID);
                Console.WriteLine("Plate: " + _parking.ListOfCars[i].ticket.NumerberPlate);
                Console.WriteLine("Spot: " + _parking.ListOfCars[i].ParkingSpot + "\n");
            }
        }
        public async Task UnregisterCar()
        {
            while(true)
            {
                Console.Write("Enter your plate number: ");
                string plateNumber = Console.ReadLine().ToLower();
                Console.Clear();
                try
                {
                    KeyValuePair<int, int> info = await _parking.CheckOut(plateNumber);
                    Console.WriteLine($"You have paid: {info.Value} $ for {info.Key} hours");
                    break;
                }catch(OverflowException er)
                {
                    Console.WriteLine(er.Message);
                    break;
                }
                catch (Exception er)
                {
                    Console.WriteLine(er.Message);
                }
            }
        }
        public async Task RegisterCar()
        { 
            try
            {
                //_parking.CheckIn(CarType.BigCar, "das");
                if (_parking.ListOfCars.Count(x => x.ticket == null) > 3) { 
                    for (int i = 0; i <= 3; i++)
                        _parking.CheckIn(CarType.Car, $"asd{i}".ToLower());
                    return;
                }

                Console.Clear();
                CarType type = await ChooseType();

                Console.Write("Enter your numberplate: ");
                string plate = Console.ReadLine();

                Console.Clear();
                Console.WriteLine($"You have gotten parking slot {_parking.CheckIn(type, plate)}");
            }
            catch(Exception er) 
                { Console.WriteLine(er.Message); }
            
        }

        // Private methods
        protected async Task<CarType> ChooseType()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Prices are per hour \n");
                Console.WriteLine($"A: {CarType.Car}           | Price (hour): {(int)CarType.Car} $");
                Console.WriteLine($"B: {CarType.ExtendedCar}   | Price (hour): {(int)CarType.ExtendedCar} $");
                Console.WriteLine($"C: {CarType.BigCar}        | Price (hour): {(int)CarType.BigCar} $");
                

                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        return CarType.Car;
                    case ConsoleKey.B:
                        return CarType.ExtendedCar;
                    case ConsoleKey.C:
                        return CarType.BigCar;
                }
            }
        }
    }
}
