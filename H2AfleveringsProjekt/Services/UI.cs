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
            _parking.CheckIn(CarType.BigCar, "sad");
        }
        public void UnregisterCar()
        {

        }
        public void RegisterCar()
        { 

        }

        // Private methods
        protected CarType? ChooseType()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Prices are per hour \n");
                Console.WriteLine($"A: {CarType.Car}           | Price (hour): {(int)CarType.Car} $");
                Console.WriteLine($"B: {CarType.ExtendedCar}   | Price (hour): {(int)CarType.ExtendedCar} $");
                Console.WriteLine($"C: {CarType.BigCar}         | Price (hour): {(int)CarType.BigCar} $");

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
