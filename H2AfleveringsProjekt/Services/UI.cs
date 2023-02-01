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
        public void ShowTicketlist()
        {
            Console.WriteLine("List\n");
            Console.WriteLine(_parking.ListOfCars.Count());
            foreach(var item in _parking.ListOfCars)
            {
                Console.WriteLine("TicketID: " + item.ticket.TicketID);
                Console.WriteLine("Plate: " + item.ticket.NumerberPlate);
                Console.WriteLine("Slot: " + item.ParkingSpot);
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
                    KeyValuePair<int, decimal> info = await _parking.CheckOut(plateNumber);
                    Console.WriteLine($"You have paid: {info.Value} $ for {info.Key} hours");
                    break;
                }catch(KeyNotFoundException er)
                {
                    Console.WriteLine(er.Message);
                    break;
                }
            }
        }
        public void RegisterCar()
        { 
            for(int i = 0; i <= 19; i++)
                _parking.CheckIn(CarType.Car, $"asd{i}".ToLower());
            _parking.CheckIn(CarType.BigCar, "das");
            
        }

        // Private methods
        protected CarType? ChooseType(bool showPrices = false)
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
